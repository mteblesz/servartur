using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Servartur.Data.PostgreSQL.Entities;

internal class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
{
    private const string TableName = "rooms";

    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired();
    }
}
