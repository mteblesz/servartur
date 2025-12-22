using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Servartur.Data.PostgreSQL.Entities;

internal class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
    private const string TableName = "players";

    public void Configure(EntityTypeBuilder<PlayerEntity> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name);

        builder.Property(x => x.Character);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasIndex(p => new { p.RoomId, p.Name })
            .IsUnique();
    }
}
