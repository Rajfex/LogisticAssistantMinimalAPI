using LogisticAssistantMinimalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace LogisticAssistantMinimalAPI
{
    public class TrucksEndpoints
    {
        public static void MapTrucksEndpoints(WebApplication app)
        {
            app.MapPost("/api/trucks/create", async (TruckViewModel truckViewModel, HttpContext http, AppDbContext db) =>
            {
                var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Results.Unauthorized();

                var truck = new Truck
                {
                    LicensePlate = truckViewModel.LicensePlate,
                    Vmax = truckViewModel.Vmax,
                    DriverBreak = truckViewModel.DriverBreak,
                    UserId = userId
                };

                db.Trucks.Add(truck);
                await db.SaveChangesAsync();

                return Results.Ok("Truck created");
            })
            .RequireAuthorization();

            app.MapGet("/api/trucks", async (HttpContext http, AppDbContext db) =>
            {
                var trucks = await db.Trucks
                    .Select(t => new TruckViewModel
                    {
                        Id = t.Id,
                        LicensePlate = t.LicensePlate,
                        Vmax = t.Vmax,
                        DriverBreak = t.DriverBreak
                    })
                    .ToListAsync();
                return Results.Ok(trucks);
            });

            app.MapGet("/api/trucks/{id}", async (int id, HttpContext http, AppDbContext db) =>
            {
                var truck = await db.Trucks
                    .Where(t => t.Id == id)
                    .Select(t => new TruckViewModel
                    {
                        Id = t.Id,
                        LicensePlate = t.LicensePlate,
                        Vmax = t.Vmax,
                        DriverBreak = t.DriverBreak
                    })
                    .FirstOrDefaultAsync();
                if (truck == null)
                    return Results.NotFound();
                return Results.Ok(truck);
            });

            app.MapDelete("/api/trucks/{id}", async (int id, HttpContext http, AppDbContext db) =>
            {
                var truck = await db.Trucks.FindAsync(id);
                if (truck == null)
                    return Results.NotFound();
                db.Trucks.Remove(truck);
                await db.SaveChangesAsync();
                return Results.Ok("Truck deleted");
            });

            app.MapPut("/api/trucks/{id}", async (int id, TruckViewModel truckViewModel, HttpContext http, AppDbContext db) =>
            {
                var truck = await db.Trucks.FindAsync(id);
                if (truck == null)
                    return Results.NotFound();
                truck.LicensePlate = truckViewModel.LicensePlate;
                truck.Vmax = truckViewModel.Vmax;
                truck.DriverBreak = truckViewModel.DriverBreak;
                await db.SaveChangesAsync();
                return Results.Ok("Truck updated");
            });
        }
    }
}
