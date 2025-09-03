using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Domain;

namespace Infrastructure.Data;

public class TooliRentDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();

    public TooliRentDbContext(DbContextOptions<TooliRentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(80);
        });

        b.Entity<Tool>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.HasOne(x => x.Category)
             .WithMany(c => c.Tools)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Booking>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Member)
             .WithMany()
             .HasForeignKey(x => x.MemberId)
             .OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.StartAt).IsRequired();
            e.Property(x => x.EndAt).IsRequired();
        });

        b.Entity<BookingItem>(e =>
        {
            e.HasKey(x => new { x.BookingId, x.ToolId });
            e.HasOne(x => x.Booking).WithMany(bk => bk.Items).HasForeignKey(x => x.BookingId);
            e.HasOne(x => x.Tool).WithMany(t => t.BookingItems).HasForeignKey(x => x.ToolId);
        });
    }
}