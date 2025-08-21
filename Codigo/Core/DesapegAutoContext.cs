using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class DesapegAutoContext : DbContext
{
    public DesapegAutoContext()
    {
    }

    public DesapegAutoContext(DbContextOptions<DesapegAutoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Concessionaria> Concessionaria { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Veiculo> Veiculos { get; set; }

    public virtual DbSet<VeiculoHasVersao> VeiculoHasVersaos { get; set; }

    public virtual DbSet<Venda> Venda { get; set; }

    public virtual DbSet<Versao> Versaos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("anuncio");

            entity.HasIndex(e => e.IdVeiculo, "fk_anuncio_veiculo_idx");

            entity.HasIndex(e => e.IdVenda, "fk_anuncio_venda1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataPublicacao)
                .HasColumnType("timestamp")
                .HasColumnName("dataPublicacao");
            entity.Property(e => e.Descricao)
                .HasMaxLength(50)
                .HasColumnName("descricao");
            entity.Property(e => e.IdVeiculo).HasColumnName("idVeiculo");
            entity.Property(e => e.IdVenda).HasColumnName("idVenda");
            entity.Property(e => e.Interacoes).HasColumnName("interacoes");
            entity.Property(e => e.Opcionais)
                .HasMaxLength(50)
                .HasColumnName("opcionais");
            entity.Property(e => e.StatusAnuncio)
                .HasMaxLength(20)
                .HasColumnName("statusAnuncio");
            entity.Property(e => e.Visualizacoes).HasColumnName("visualizacoes");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Concessionaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("concessionaria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(14)
                .HasColumnName("cnpj");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IdEndereco).HasColumnName("idEndereco");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Senha)
                .HasMaxLength(8)
                .HasColumnName("senha");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("marca");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("modelo");

            entity.HasIndex(e => e.IdMarca, "fk_modelo_marca1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .HasColumnName("categoria");
            entity.Property(e => e.IdMarca).HasColumnName("idMarca");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Versoes)
                .HasMaxLength(100)
                .HasColumnName("versoes");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Senha)
                .HasMaxLength(8)
                .HasColumnName("senha");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("veiculo");

            entity.HasIndex(e => e.IdConcessionaria, "fk_veiculo_concessionaria_idx");

            entity.HasIndex(e => e.IdMarca, "fk_veiculo_marca1_idx");

            entity.HasIndex(e => e.IdModelo, "fk_veiculo_modelo_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ano).HasColumnName("ano");
            entity.Property(e => e.Cor)
                .HasMaxLength(45)
                .HasColumnName("cor");
            entity.Property(e => e.IdConcessionaria).HasColumnName("idConcessionaria");
            entity.Property(e => e.IdMarca).HasColumnName("idMarca");
            entity.Property(e => e.IdModelo).HasColumnName("idModelo");
            entity.Property(e => e.Placa)
                .HasMaxLength(7)
                .HasColumnName("placa");
            entity.Property(e => e.Preco)
                .HasPrecision(10)
                .HasColumnName("preco");
            entity.Property(e => e.Quilometragem).HasColumnName("quilometragem");
        });

        modelBuilder.Entity<VeiculoHasVersao>(entity =>
        {
            entity.HasKey(e => new { e.IdVeiculo, e.IdVersao }).HasName("PRIMARY");

            entity.ToTable("veiculo_has_versao");

            entity.HasIndex(e => e.IdVeiculo, "fk_veiculo_has_versao_veiculo1_idx");

            entity.HasIndex(e => e.IdVersao, "fk_veiculo_has_versao_versao1_idx");

            entity.Property(e => e.IdVeiculo).HasColumnName("idVeiculo");
            entity.Property(e => e.IdVersao).HasColumnName("idVersao");
        });

        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("venda");

            entity.HasIndex(e => e.IdConcessionaria, "fk_venda_concessionaria1_idx");

            entity.HasIndex(e => e.IdUsuario, "fk_venda_usuario1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataVenda)
                .HasColumnType("date")
                .HasColumnName("dataVenda");
            entity.Property(e => e.FormaPagamento)
                .HasMaxLength(45)
                .HasColumnName("formaPagamento");
            entity.Property(e => e.IdConcessionaria).HasColumnName("idConcessionaria");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.ValorFinal)
                .HasPrecision(10)
                .HasColumnName("valorFinal");
        });

        modelBuilder.Entity<Versao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("versao");

            entity.HasIndex(e => e.IdModelo, "fk_versao_modelo1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdModelo).HasColumnName("idModelo");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
