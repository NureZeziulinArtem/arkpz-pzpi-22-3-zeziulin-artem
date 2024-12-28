using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ApplicationContext : IdentityDbContext<Account>
    {
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceLog> DeviceLog { get; set; }
        public DbSet<Fertilizer> Fertilizers { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<WateringReminder> WateringReminders { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CartItem>()
                .HasKey(ci => new { ci.AccountId, ci.FertilizerId });

            builder.Entity<DeviceLog>()
                .HasOne(dl => dl.Device)
                .WithMany(d => d.Logs)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WateringReminder>()
                .HasOne(wr => wr.Device)
                .WithMany(d => d.Reminders)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Fertilizer)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Device>()
                .HasOne(d => d.Plant)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
