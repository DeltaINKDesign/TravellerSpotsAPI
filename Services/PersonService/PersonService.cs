using System.Collections.Generic;
using TravellerSpot.Models;
using Microsoft.AspNetCore.Mvc;
using TravellerSpot.Contexts;
using Neo4j.Driver.V1;
using System.Linq;
using StackExchange.Redis;
using TravellerSpot.QueryHelper;

namespace TravellerSpot.Services
{
    public class PersonService
    {
        private readonly DatabaseContext _database;
        private readonly RedisService _redisService;

        public PersonService(DatabaseContext database, RedisService redisService)
        {
            _database = database;
            _redisService = redisService;
        }

        public ActionResult<Person> Create(Person p)       //pozwala na dodawanie duplikatow
        {
            string createQuery = QueryHelper<Person>.CreateTemplateReturner(p,"Nick",p.Nick);
            string matchQuery = QueryHelper<Person>.MatchTemplateReturner(p,"Nick",p.Nick);

            using (var s = _database.Driver.Session())
            {
                bool dupFound = true;
                s.ReadTransaction(tx =>
                {
                    var res = tx.Run(matchQuery);
                    if (res.Peek() == null)  //jesli nie ma duplikatu
                    {
                        dupFound = false; //brak dup
                    }
                });
                if (dupFound) p.Nick = "DUPLIKAT";
                if(!dupFound)
                {       //tworzymy jesli nie ma duplikatu
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createQuery);
                    });
                    HashEntry[] p1 = {
                        new HashEntry("Name", p.Name),
                        new HashEntry("From", p.From),
                        new HashEntry("Age", p.Age)
                    };
                    HashEntry[] redisPersonHash = p1;
                    _redisService.RedisConnection.GetDatabase().HashSet("Person:" + p.Nick, redisPersonHash);
                }

            }

           return p ;  //zmienic zwrot
        }

        public ActionResult<List<Person>> GetAll()
        {
            List<Person> persons = new List<Person>();
            var statement = "MATCH (p:Person) return p";
            using (var session = _database.Driver.Session())   
            {
                using var tx = session.BeginTransaction();
                IStatementResult results = tx.Run(statement);
                foreach (IRecord result in results)
                {
                    var node = result["p"].As<INode>();
                    var redisData =_redisService.RedisConnection.GetDatabase().HashGetAll("Person:"+node.Properties["nick"].As<string>());
                    persons.Add(
                        new Person
                        {
                            Nick = node.Properties["nick"].As<string>(),
                            Name = redisData.FirstOrDefault(x => x.Name == "Name").Value,
                            From = redisData.FirstOrDefault(x => x.Name == "From").Value,
                            Age = (int) redisData.FirstOrDefault(x => x.Name == "Age").Value,
                        });
                    ;
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