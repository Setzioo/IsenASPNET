using System;
using System.Collections.Generic;
using System.Linq;
using Isen.Dotnet.Library.Context;
using Isen.Dotnet.Library.Model;
using Microsoft.Extensions.Logging;

namespace Isen.Dotnet.Library.Services
{
    public class DataInitializer : IDataInitializer
    {
        private List<string> _firstNames => new List<string>
        {
            "Sang", 
            "Anne",
            "Boris",
            "Pierre",
            "Laura",
            "Hadrien",
            "Camille",
            "Louis",
            "Alicia"
        };
        private List<string> _lastNames => new List<string>
        {
            "Schuck",
            "Arbousset",
            "Lopasso",
            "Jubert",
            "Lebrun",
            "Dutaud",
            "Sarrazin",
            "Vu Dinh"
        };
        // Générateur aléatoire
        private readonly Random _random;

        // DI de ApplicationDbContext
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataInitializer> _logger;
        public DataInitializer(
            ILogger<DataInitializer> logger,
            ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            _random = new Random();
        }

        // Générateur de prénom
        private string RandomFirstName => 
            _firstNames[_random.Next(_firstNames.Count)];
        // Générateur de nom
        private string RandomLastName => 
            _lastNames[_random.Next(_lastNames.Count)];
        // Générateur de Service
        private Service RandomService
        {
            get
            {
                var services = _context.ServiceCollection.ToList();
                return services[_random.Next(services.Count)];
            }
        }
         // Générateur de Role
        private Role RandomRole
        {
            get
            {
                var roles = _context.RoleCollection.ToList();
                return roles[_random.Next(roles.Count)];
            }
        }

        // Générateur de date
        private DateTime RandomDate =>
            new DateTime(_random.Next(1980, 2010), 1, 1)
                .AddDays(_random.Next(0, 365));
        //Générateur de numéro de téléphone
        private string RandomPhoneNumber
        {
            get
            {
                var phoneNumber = _random.Next(600000000, 700000000);
                return $"0{phoneNumber}";
            }
        }

        // Générateur de personne
        private Person RandomPerson 
        {
            get
            {
                var first = RandomFirstName;
                var last = RandomLastName;
                var role = RandomRole;
                var person = new Person()
                {
                    FirstName = first,
                    LastName = last,
                    DateOfBirth = RandomDate,
                    PhoneNumber = RandomPhoneNumber,
                    Mail = $"{first}.{last}@mail.com",
                    Service = RandomService,
                    PersonRoles = new List<PersonRole>()
                };

                person.PersonRoles.Add(new PersonRole()
                {
                    Person = person,
                    PersonId = person.Id,
                    Role = role,
                    RoleId = role.Id
                });

                return person;
            }
        }

        // Générateur de personnes
        public List<Person> GetPersons(int size)
        {
            var persons = new List<Person>();
            for(var i = 0 ; i < size ; i++)
            {
                persons.Add(RandomPerson);
            }
            return persons;
        }

        public List<Service> GetServices()
        {
            return new List<Service>
            {
                new Service { Name = "Marketing"},
                new Service { Name = "Production"},
                new Service { Name = "Recherche"},
                new Service { Name = "Developpement"},
                new Service { Name = "Direction"},
                new Service { Name = "Comptabilité"},
                new Service { Name = "Administration"}
            };
        }

        public List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Name = "Utilisateur"},
                new Role { Name = "Manager"},
                new Role { Name = "Administrateur"},
                new Role { Name = "SuperAdministrateur"},
            };
        }

        public void DropDatabase()
        {
            _logger.LogWarning("Dropping database");
            _context.Database.EnsureDeleted();
        } 

        public void CreateDatabase()
        {
            _logger.LogWarning("Creating database");
            _context.Database.EnsureCreated();
        }

        public void AddPersons()
        {
            _logger.LogWarning("Adding persons...");
            // S'il y a déjà des personnes dans la base -> ne rien faire
            if (_context.PersonCollection.Any()) return;
            // Générer des personnes
            var persons = GetPersons(50);
            // Les ajouter au contexte
            _context.AddRange(persons);
            // Sauvegarder le contexte
            _context.SaveChanges();
        }

        public void AddServices()
        {
            _logger.LogWarning("Adding services...");
            if (_context.ServiceCollection.Any()) return;
            var services = GetServices();
            _context.AddRange(services);
            _context.SaveChanges();
        }

        public void AddRoles()
        {
            _logger.LogWarning("Adding roles...");
            if (_context.RoleCollection.Any()) return;
            var roles = GetRoles();
            _context.AddRange(roles);
            _context.SaveChanges();
        }
    }
}