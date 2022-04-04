using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ADMRH_API.Models
{
    public partial class ADMRHJQContext : DbContext
    {
        public ADMRHJQContext()
        {
        }

        public ADMRHJQContext(DbContextOptions<ADMRHJQContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Archivo> Archivos { get; set; }
        public virtual DbSet<Candidato> Candidatos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Vacante> Vacantes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("workstation id=ADMRHJQ.mssql.somee.com;packet size=4096;user id=AdmYamc_SQLLogin_1;pwd=kcsbd48khw;data source=ADMRHJQ.mssql.somee.com;persist security info=False;initial catalog=ADMRHJQ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Archivo>(entity =>
            {
                entity.HasKey(e => e.IdArchivos);

                entity.Property(e => e.Cv).IsRequired();

                entity.Property(e => e.FotoFrente)
                    .IsRequired()
                    .HasColumnName("Foto_frente");

                entity.Property(e => e.FotoPerfil).HasColumnName("Foto_perfil");
            });

            modelBuilder.Entity<Candidato>(entity =>
            {
                entity.HasKey(e => e.IdCandidato);

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Cedula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Direccion).IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Fecha_creacion");

                entity.Property(e => e.IdUsuarioCreacion).HasColumnName("IdUsuario_creacion");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Cedula)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Contraseña)
                    .IsRequired()
                    .HasMaxLength(65);

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Departamento)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.Direccion).IsRequired();

                entity.Property(e => e.LoginDate)
                .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rol)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Token);
            });

            modelBuilder.Entity<Vacante>(entity =>
            {
                entity.HasKey(e => e.IdVacante);

                entity.Property(e => e.Cargo)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.Departamento)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Descripcion).IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Fecha_creacion");

                entity.Property(e => e.IdUsuarioCreacion).HasColumnName("IdUsuario_creacion");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Salario)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
