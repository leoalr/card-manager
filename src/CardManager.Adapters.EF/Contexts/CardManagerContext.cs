using CardManager.Adapters.EF.EntitiesConfigurations;
using CardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Adapters.EF.Contexts;

public class CardManagerContext : DbContext
{
    public CardManagerContext(DbContextOptions<CardManagerContext> options)
        : base(options) { }

    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CardEntityTypeConfiguration());
    }
}
