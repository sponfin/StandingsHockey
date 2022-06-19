namespace StandingsHockey.Entities
{
    public partial class standingsContext : DbContext
    {
        public standingsContext()
        {
        }

        public standingsContext(DbContextOptions<standingsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; } = null!;
        public virtual DbSet<Team> Teams { get; set; } = null!;
        public virtual DbSet<Tourney> Tourneys { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=WIN-2DQ8VAR87JG;Database=standings;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.DateGame).HasColumnType("date");

                entity.Property(e => e.IsSo).HasColumnName("IsSO");

                entity.Property(e => e.Result1)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Result2)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ResultGame)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TimeGame)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.HasOne(d => d.TeamId1Navigation)
                    .WithMany(p => p.GameTeamId1Navigations)
                    .HasForeignKey(d => d.TeamId1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Games_Teams_Team1");

                entity.HasOne(d => d.TeamId2Navigation)
                    .WithMany(p => p.GameTeamId2Navigations)
                    .HasForeignKey(d => d.TeamId2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Games_Teams_Team2");

                entity.HasOne(d => d.Tourney)
                    .WithMany(p => p.Games)
                    .HasForeignKey(d => d.TourneyId)
                    .HasConstraintName("FK_Games_Tourneys");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Logo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TeamName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tourney>(entity =>
            {
                entity.Property(e => e.TourneyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
