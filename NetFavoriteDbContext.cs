using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetFavorite.Models;

namespace NetFavorite;

public partial class NetFavoriteDbContext : DbContext
{
    public NetFavoriteDbContext(DbContextOptions<NetFavoriteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bookmark> Bookmark { get; set; }

    public virtual DbSet<LoginUser> LoginUser { get; set; }

    public virtual DbSet<RolePermission> RolePermission { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.Bookmark_Id).HasName("PK__Bookmark__6869B5C86EF09158");

            entity.Property(e => e.Bookmark_Id).ValueGeneratedNever();
            entity.Property(e => e.Bookmark_Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Bookmark_CreateTime).HasColumnType("datetime");
            entity.Property(e => e.Bookmark_Title)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LoginUser>(entity =>
        {
            entity.HasKey(e => e.LoginUser_Id);

            entity.Property(e => e.LoginUser_Id).ValueGeneratedNever();
            entity.Property(e => e.LoginUser_Account)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser_Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser_Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoginUser_Salt).HasMaxLength(500);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RolePermission_Role, e.RolePermission_Permission });

            entity.Property(e => e.RolePermission_Role).HasMaxLength(50);
            entity.Property(e => e.RolePermission_Permission).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
