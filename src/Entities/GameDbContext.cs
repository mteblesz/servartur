using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace servartur.Entities;

public class GameDbContext : DbContext
{
    private readonly string _connectionString =
        "Data Source=.\\SQLEXPRESS;Initial Catalog=DatarturDb;Integrated Security=True;TrustServerCertificate=true";

    public GameDbContext(DbContextOptions options) : base(options) { }

    public virtual DbSet<Room> Rooms { get; set; } = null!;
    public virtual DbSet<Player> Players { get; set; } = null!;
    public virtual DbSet<Squad> Squads { get; set; } = null!;
    public virtual DbSet<Membership> Memberships { get; set; } = null!;
    public virtual DbSet<SquadVote> SquadVotes { get; set; } = null!;
    public virtual DbSet<QuestVote> QuestVotes { get; set; } = null!;
    public virtual DbSet<Assassination> Assassinations { get; set; } = null!;




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        addPrimaryKeys(modelBuilder);

        addRoomOneToMany(modelBuilder);
        addLeaderOneToMany(modelBuilder);
        addCurrentSquadOneToOne(modelBuilder);
        addAllVotesRelations(modelBuilder);
        addMembershipManyToMany(modelBuilder);
        addAssassinationOneToOne(modelBuilder);

        addEnumFieldsConvertions(modelBuilder);
        addPlayerNickLengthLimit(modelBuilder);
    }

    private static void addEnumFieldsConvertions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .Property(p => p.Team)
            .HasConversion<string>();

        modelBuilder.Entity<Player>()
            .Property(p => p.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Room>()
            .Property(r => r.Status) 
            .HasConversion<string>();

        modelBuilder.Entity<Squad>()
            .Property(r => r.Status)
            .HasConversion<string>();
    }

    private static void addPlayerNickLengthLimit(ModelBuilder modelBuilder)
    {
        //only field in database for players to directly enter
        modelBuilder.Entity<Player>()
                .Property(p => p.Nick)
                .HasMaxLength(20);
    }

    private static void addPrimaryKeys(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>().HasKey(e => e.RoomId);
        modelBuilder.Entity<Player>().HasKey(e => e.PlayerId);
        modelBuilder.Entity<Squad>().HasKey(e => e.SquadId);
        modelBuilder.Entity<SquadVote>().HasKey(e => e.SquadVoteId);
        modelBuilder.Entity<QuestVote>().HasKey(e => e.QuestVoteId);
        modelBuilder.Entity<Assassination>().HasKey(e => e.AssassinationId);
        modelBuilder.Entity<Membership>()
            .HasKey(m => new { m.SquadId, m.PlayerId });
    }

    private static void addRoomOneToMany(ModelBuilder modelBuilder)
    {
        // One-to-many without navigation to principal
        modelBuilder.Entity<Room>()
        .HasMany(e => e.Players)
        .WithOne()
        .HasForeignKey(e => e.RoomId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        // One-to-many without navigation to principal
        modelBuilder.Entity<Squad>()
        .HasOne<Room>()
        .WithMany(e => e.Squads)
        .HasForeignKey(e => e.RoomId)
        .IsRequired();
    }
    private static void addLeaderOneToMany(ModelBuilder modelBuilder)
    {

        // One-to-many without navigation to dependents
        modelBuilder.Entity<Squad>()
        .HasOne(e => e.Leader)
        .WithMany()
        .HasForeignKey(e => e.LeaderId)
        .OnDelete(DeleteBehavior.Restrict);
    }
    private static void addCurrentSquadOneToOne(ModelBuilder modelBuilder)
    {
        // One-to-one without navigation to dependent
        modelBuilder.Entity<Room>()
        .HasOne(e => e.CurrentSquad)
        .WithOne()
        .HasForeignKey<Room>(e => e.CurrentSquadId)
        .OnDelete(DeleteBehavior.Restrict);
    }

    private static void addAllVotesRelations(ModelBuilder modelBuilder)
    {

        //Required one-to-many
        modelBuilder.Entity<Squad>()
        .HasMany(e => e.SquadVotes)
        .WithOne(e => e.Squad)
        .HasForeignKey(e => e.SquadId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

        //Required one-to-many
        modelBuilder.Entity<Squad>()
        .HasMany(e => e.QuestVotes)
        .WithOne(e => e.Squad)
        .HasForeignKey(e => e.SquadId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

        //Required one-to-many
        modelBuilder.Entity<Player>()
        .HasMany(e => e.SquadVotes)
        .WithOne(e => e.Voter)
        .HasForeignKey(e => e.VoterId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

        //Required one-to-many
        modelBuilder.Entity<Player>()
        .HasMany(e => e.QuestVotes)
        .WithOne(e => e.Voter)
        .HasForeignKey(e => e.VoterId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);
    }
    private static void addMembershipManyToMany(ModelBuilder modelBuilder)
    {
        // Many to many
        modelBuilder.Entity<Membership>()
            .HasOne(m => m.Squad)
            .WithMany(s => s.Memberships)
            .HasForeignKey(m => m.SquadId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.Player)
            .WithMany(p => p.Memberships)
            .HasForeignKey(m => m.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    private static void addAssassinationOneToOne(ModelBuilder modelBuilder)
    {

        // One-to-one without navigation to dependent
        modelBuilder.Entity<Assassination>()
        .HasOne(e => e.Assassin)
        .WithOne()
        .HasForeignKey<Assassination>(e => e.AssassinId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

        // One-to-one without navigation to dependent
        modelBuilder.Entity<Assassination>()
        .HasOne(e => e.Target)
        .WithOne()
        .HasForeignKey<Assassination>(e => e.TargetId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);

        // One-to-one without navigation to principal
        modelBuilder.Entity<Assassination>()
        .HasOne<Room>()
        .WithOne(e => e.Assassination)
        .HasForeignKey<Assassination>(e => e.RoomId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Restrict);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }
}

