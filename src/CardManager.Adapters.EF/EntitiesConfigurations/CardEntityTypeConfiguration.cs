using CardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardManager.Adapters.EF.EntitiesConfigurations;

internal class CardEntityTypeConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey("_id");
        builder.Property("_id");
        builder.Property("_customerId");
        builder.Property("_number");
        builder.Property("_cvv");
        builder.Property("_registrationDate");
    }
}
