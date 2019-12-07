using System.Collections.Generic;
using TravellerSpot.Models;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Contexts;
using Neo4j.Driver.V1;

namespace TravellerSpot.Services
{
    public class PersonService
    {
        private readonly DatabaseContext _database;

        public PersonService(DatabaseContext database)
        {
            _database = database;
        }

        public ActionResult Create(Person p)
        {
            //var statement =


                return null;
        }

        public List<Person> GetAll()
        {
            List<Person> persons = new List<Person>();
            var statement = "MATCH (p:Person) return p";
            using (var session = _database.Driver.Session())   // obczaic co jest 5 z tymi usingami
            {
                using var tx = session.BeginTransaction();
                IStatementResult results = tx.Run(statement);
                foreach (IRecord result in results)
                {
                    var node = result["p"].As<INode>();
                    persons.Add(
                        new Person
                        {
                            Nick = node.Properties["nick"].As<string>(),
                            Name = node.Properties["name"].As<string>(),
                            Age = node.Properties["age"].As<int>(),
                            From = node.Properties["from"].As<string>(),
                        });
                }
            }
            return persons;
        }

        public List<Person> GetPersonsWithNickLike()
        {
            List<Person> persons = new List<Person>();
            var statement = "MATCH (p:person) WHERE ee.name";
            return null;
        }

        //public Person Get(string id) =>
        //_persons.Find<Person>(person => person.Id == id).FirstOrDefault();
    }
}