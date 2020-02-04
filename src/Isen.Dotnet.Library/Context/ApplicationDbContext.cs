using System.Diagnostics.CodeAnalysis;
using Isen.Dotnet.Library.Model;
using Microsoft.EntityFrameworkCore;

namespace Isen.Dotnet.Library.Context
{    
    public class ApplicationDbContext : DbContext
    {        
        // Listes des classes modèle / tables
        public DbSet<Person> PersonCollection { get; set; }
        public DbSet<Role> RoleCollection { get; set; }
        public DbSet<Service> ServiceCollection { get; set; }

        public ApplicationDbContext(
            [NotNullAttribute] DbContextOptions options) : 
            base(options) {  }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tables et relations
            modelBuilder
                // Associer la classe Person...
                .Entity<Person>()
                // ... à la table Person
                .ToTable(nameof(Person))
                // Description de la relation Person.Service
                .HasOne(p => p.Service)
                // Relation réciproque (omise)
                .WithMany()
                // Clé étrangère qui porte cette relation
                .HasForeignKey(p => p.ServiceId);

            

            // Et utiliser le champ Id comme clé primaire
            // Déclaration optionnelle, car le nommage
            // Id ou PersonId est reconnu comme convention
            // pour les clés primaires ou étrangères
            modelBuilder.Entity<Person>()
                .HasKey(p => p.Id);

            // Pareil pour Role
            modelBuilder
                .Entity<Role>()
                .ToTable(nameof(Role))
                .HasKey(r => r.Id);
            // Pareil pour Service
            modelBuilder
                .Entity<Service>()
                .ToTable(nameof(Service))
                .HasKey(s => s.Id);

                // Relation many to many PersonRole
            modelBuilder.Entity<PersonRole>()
                .HasKey(pr => new { pr.PersonId, pr.RoleId });  
            modelBuilder.Entity<PersonRole>()
                .HasOne(pr => pr.Person)
                .WithMany(p => p.PersonRoles)
                .HasForeignKey(pr => pr.PersonId);  
            modelBuilder.Entity<PersonRole>()
                .HasOne(pr => pr.Role)
                .WithMany(r => r.PersonRoles)
                .HasForeignKey(pr => pr.RoleId);
        }

    }
}