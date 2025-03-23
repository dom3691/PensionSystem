using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Contribution> Contributions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)  
            .IsUnique();

        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Phone)
            .IsUnique();

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Member)
            .WithMany()
            .HasForeignKey(c => c.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
