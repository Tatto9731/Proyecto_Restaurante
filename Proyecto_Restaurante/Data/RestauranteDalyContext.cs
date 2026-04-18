using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Models;

namespace Proyecto_Restaurante.Data;

public partial class RestauranteDalyContext : DbContext
{
    public RestauranteDalyContext()
    {
    }

    public RestauranteDalyContext(DbContextOptions<RestauranteDalyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoOrden> ProductoOrdens { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Salonero> Saloneros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C5C1640831");

            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Cliente__71ABD0A7C8BE5245");

            entity.ToTable("Cliente");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Cliente__531402F3DAC6A575").IsUnique();

            entity.HasIndex(e => e.Identificacion, "UQ__Cliente__D6F931E59FA74AC4").IsUnique();

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.Identificacion).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(15);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.FacturaId).HasName("PK__Factura__5C0248052BBB974E");

            entity.ToTable("Factura");

            entity.HasIndex(e => e.NumeroFactura, "UQ__Factura__CF12F9A6B144E36E").IsUnique();

            entity.Property(e => e.FacturaId).HasColumnName("FacturaID");
            entity.Property(e => e.Impuestos).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MetodoPago).HasMaxLength(50);
            entity.Property(e => e.NumeroFactura).HasMaxLength(50);
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Orden).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__Factura__OrdenID__05D8E0BE");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.MesaId).HasName("PK__Mesa__6A4196C83BEDC1B0");

            entity.ToTable("Mesa");

            entity.Property(e => e.MesaId).HasColumnName("MesaID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Disponible");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.OrdenId).HasName("PK__Orden__C088A4E4C9519D3B");

            entity.ToTable("Orden");

            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.MesaId).HasColumnName("MesaID");
            entity.Property(e => e.SaloneroId).HasColumnName("SaloneroID");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK__Orden__MesaID__01142BA1");

            entity.HasOne(d => d.Salonero).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.SaloneroId)
                .HasConstraintName("FK__Orden__SaloneroI__02084FDA");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83F0E98945");

            entity.ToTable("Producto");

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK__Producto__Catego__7A672E12");
        });

        modelBuilder.Entity<ProductoOrden>(entity =>
        {
            entity.HasKey(e => e.ProductoOrdenId).HasName("PK__Producto__1E0C4908160B3405");

            entity.ToTable("ProductoOrden");

            entity.Property(e => e.ProductoOrdenId).HasColumnName("ProductoOrdenID");
            entity.Property(e => e.OrdenId).HasColumnName("OrdenID");
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.Orden).WithMany(p => p.ProductoOrdens)
                .HasForeignKey(d => d.OrdenId)
                .HasConstraintName("FK__ProductoO__Orden__08B54D69");

            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoOrdens)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__ProductoO__Produ__09A971A2");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PK__Reserva__C3993703EC8E24E9");

            entity.ToTable("Reserva");

            entity.Property(e => e.ReservaId).HasColumnName("ReservaID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.MesaId).HasColumnName("MesaID");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Reserva__Cliente__7D439ABD");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("FK__Reserva__MesaID__7E37BEF6");
        });

        modelBuilder.Entity<Salonero>(entity =>
        {
            entity.HasKey(e => e.SaloneroId).HasName("PK__Salonero__89429A4448497433");

            entity.ToTable("Salonero");

            entity.Property(e => e.SaloneroId).HasColumnName("SaloneroID");
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
