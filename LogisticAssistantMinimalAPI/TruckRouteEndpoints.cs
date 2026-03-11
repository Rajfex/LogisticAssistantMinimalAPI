using LogisticAssistantMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticAssistantMinimalAPI
{
    public static class TruckRouteEndpoints
    {
        public static void MapTruckRouteEndpoints(this WebApplication app)
        {
            app.MapGet("/", async (AppDbContext context) =>
            {
                var routes = await context.Routes
                    .Include(r => r.Truck)
                    .ToListAsync();

                return Results.Ok(routes);
            });

            app.MapGet("/{id:int}", async (int id, AppDbContext context) =>
            {
                var route = await context.Routes
                    .Include(r => r.Truck)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (route == null)
                    return Results.NotFound();

                return Results.Ok(route);
            });

            app.MapGet("/trucks", async (AppDbContext context) =>
            {
                var trucks = await context.Trucks
                    .ToDictionaryAsync(t => t.Id, t => t.LicensePlate);

                return Results.Ok(trucks);
            });

            app.MapPost("/", async (TruckRouteViewModel truckRouteView, AppDbContext context) =>
            {
                var truck = await context.Trucks
                    .FirstOrDefaultAsync(t => t.Id == truckRouteView.TruckId);

                if (truck == null)
                    return Results.BadRequest("Truck not found");

                var lastRoute = await context.Routes
                    .Where(r => r.TruckId == truckRouteView.TruckId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefaultAsync();

                double remainingToBreak = truckRouteView.BreakFrequency;

                if (lastRoute != null)
                {
                    if (truck.Vmax <= 0 || truckRouteView.Distance <= 0)
                        return Results.BadRequest("Invalid data");

                    double lastTravelMinutes = ((double)lastRoute.Distance / lastRoute.TruckVmax) * 60;

                    int lastBreaks = (int)(lastTravelMinutes / lastRoute.BreakFrequency);
                    double lastBreakMinutes = lastBreaks * lastRoute.TruckDriverBreak;

                    double lastSegmentMinutes = lastTravelMinutes % lastRoute.BreakFrequency;

                    if (lastSegmentMinutes == 0)
                        lastSegmentMinutes = lastRoute.BreakFrequency;

                    remainingToBreak = lastRoute.BreakFrequency - lastSegmentMinutes;

                    double lastTotalMinutes = lastTravelMinutes + lastBreakMinutes;
                    DateTime lastRouteEnd = lastRoute.Date.AddMinutes(lastTotalMinutes);

                    if (truckRouteView.Date < lastRouteEnd)
                        return Results.BadRequest("Truck is still on previous route");
                }

                var newRoute = new TruckRoute
                {
                    TruckId = truck.Id,
                    Distance = truckRouteView.Distance,
                    BreakFrequency = truckRouteView.BreakFrequency,
                    Date = truckRouteView.Date,
                    TruckVmax = truck.Vmax,
                    TruckDriverBreak = truck.DriverBreak,
                    RemainingToBreak = remainingToBreak
                };

                context.Routes.Add(newRoute);
                await context.SaveChangesAsync();

                return Results.Created($"/truckroutes/{newRoute.Id}", newRoute);
            });
        }
    }
}