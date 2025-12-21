using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Servartur.Data.PostgreSQL.Rooms;

internal class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    private const string TableName = "rooms";

    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();
    }
}
