using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DriveAPI.Database
{
    public partial class DriveDbContext : DbContext
    {
        public DriveDbContext()
        {
        }

        public DriveDbContext(DbContextOptions<DriveDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Register> Register { get; set; }
        public virtual DbSet<UserDrive> UserDrive { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DriveDatabase");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Register>(entity =>
            {
                entity.Property(e => e.FileExtension).HasMaxLength(100);

                entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PathOrUrl)
                    .IsRequired()
                    .HasColumnName("pathOrURL")
                    .HasMaxLength(255);

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.AuthorNavigation)
                    .WithMany(p => p.Register)
                    .HasForeignKey(d => d.Author)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Register__Author__286302EC");

                entity.HasOne(d => d.ParentFolderNavigation)
                    .WithMany(p => p.InverseParentFolderNavigation)
                    .HasForeignKey(d => d.ParentFolder)
                    .HasConstraintName("FK__Register__Parent__29572725");
            });

            modelBuilder.Entity<UserDrive>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__UserDriv__A9D10534BBA898DB")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("UQ__UserDriv__536C85E4BC8685DB")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Lastname).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
