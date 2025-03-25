using Microsoft.EntityFrameworkCore;
using RoomRentalManagerServer.Domain.ModelEntities.Contracts;
using RoomRentalManagerServer.Domain.ModelEntities.Equipments;
using RoomRentalManagerServer.Domain.ModelEntities.ImageDescriptions;
using RoomRentalManagerServer.Domain.ModelEntities.Invoices;
using RoomRentalManagerServer.Domain.ModelEntities.PaymentAmount;
using RoomRentalManagerServer.Domain.ModelEntities.RoomEquipments;
using RoomRentalManagerServer.Domain.ModelEntities.RoomRentals;
using RoomRentalManagerServer.Domain.ModelEntities.User;
namespace RoomRentalManagerServer.Infrastructure.Data;

public class RoomRentalManagerServerDbContext : DbContext
{
    public RoomRentalManagerServerDbContext(DbContextOptions<RoomRentalManagerServerDbContext> options) : base(options) { }

    public DbSet<Users> Users { get; set; }
    public DbSet<RoomRental> RoomRentals { get; set; }  
    public DbSet<ImagesDescription> ImagesDescriptions { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<RoomEquipment> RoomEquipments { get; set; }
    public DbSet<Equipment> Equipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
