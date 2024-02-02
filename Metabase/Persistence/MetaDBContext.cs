using Metabase.Models;
using Metabase.Models.Attributes;
using Metabase.Models.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Metabase.Persistence
{
    public class MetaDBContext : DbContext
    {
        public MetaDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DatabaseModel> Databases { get; set; }
        public DbSet<RelationModel> Relations { get; set; }
        public DbSet<AttributeModel> Attributes { get; set; }
        public DbSet<Models.Constraints.ForeignKeyConstraint> ForeignKeyConstraints { get; set; }
        public DbSet<DefaultConstraint> DefaultConstraints { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<DatabaseModel>().HasKey(d => d.Id);
            modelBuilder.Entity<DatabaseModel>().HasIndex(d=>d.Name).IsUnique();

            //modelBuilder.Entity<RelationModel>().HasKey(r => r.Id);
            //modelBuilder.Entity<RelationModel>().HasIndex(r=>r.Name).IsUnique();

            //modelBuilder.Entity<AttributeModel>().HasIndex(a => new{ a.Relation, a.Name }).IsUnique();

            modelBuilder.Entity<Models.Constraints.ForeignKeyConstraint>().HasKey(c=>c.Id);
            modelBuilder.Entity<Models.Constraints.ForeignKeyConstraint>().HasMany(c => c.References).WithOne(r=>r.ForeignKeyConstraint).HasForeignKey(fkr => fkr.ForeignKeyConstraintId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Models.Constraints.ForeignKeyConstraint>().HasOne(c=>c.ReferencedRelation).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FKReference>().HasOne(fkr=>fkr.ReferencingAttribute).WithMany().HasForeignKey(fkr=>fkr.ReferencingAttributeId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FKReference>().HasOne(fkr=>fkr.ReferencedAttribute).WithMany().HasForeignKey(fkr => fkr.ReferencedAttributeId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FKReference>().HasKey(fkr => new {fkr.ForeignKeyConstraintId ,fkr.ReferencingAttributeId, fkr.ReferencedAttributeId });


            //modelBuilder.Entity<Models.Constraints.ForeignKeyConstraint>().HasMany(c=>c.ReferencingAttributes).WithMany().UsingEntity(
            //    l => l.HasOne(typeof(AttributeModel)).WithMany().HasForeignKey("AttributeForeignKey").OnDelete(DeleteBehavior.Cascade),
            //    r => r.HasOne(typeof(Models.Constraints.ForeignKeyConstraint)).WithMany().HasForeignKey("ForeignKeyContraintForeignKey").OnDelete(DeleteBehavior.NoAction)).ToTable("ReferencingAttributes");

            //modelBuilder.Entity<Models.Constraints.ForeignKeyConstraint>().HasMany(c => c.ReferencedAttributes).WithMany().UsingEntity(
            //    l => l.HasOne(typeof(AttributeModel)).WithMany().HasForeignKey("AttributeForeignKey").OnDelete(DeleteBehavior.Cascade),
            //    r => r.HasOne(typeof(Models.Constraints.ForeignKeyConstraint)).WithMany().HasForeignKey("ForeignKeyContraintForeignKey").OnDelete(DeleteBehavior.NoAction)).ToTable("ReferencedAttributes");
            
            //modelBuilder.Entity<ForeignKeyConstraint>().Ignore();


            base.OnModelCreating(modelBuilder);
        }

    }
}
