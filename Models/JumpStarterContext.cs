#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace BeltExam.Models;

public class JumpStarterContext : DbContext 
{ 
    public JumpStarterContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; } 
    public DbSet<Project> Projects { get; set; }
    public DbSet<Funder> Funders { get; set; }
}
