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

        public ActionResult<string> Create(Person p)       //pozwala na dodawanie duplikatow
        {
            string createQuery = QueryHelper<Person>.CreateTemplateReturner(p,"name",p.Name);
            string matchQuery = QueryHelper<Person>.MatchTemplateReturner(p,"name",p.Name);

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
                if (dupFound) p.Name = "DUPLIKAT";
                if(!dupFound)
                {       //tworzymy jesli nie ma duplikatu
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createQuery);
                    });
                    HashEntry[] p1 = {
                        new HashEntry("from", p.From),
                        new HashEntry("age", p.Age)
                    };
                    _redisService.RedisConnection.GetDatabase().HashSet("person:" + p.Name, p1);
                }

            }

            return $"Poprawnie dodałem osobę {p.Name}";
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
                    var redisData =_redisService.RedisConnection.GetDatabase().HashGetAll("person:"+node.Properties["name"].As<string>());
                    persons.Add(
                        new Person
                        {
                            Name = node.Properties["name"].As<string>(),
                            From = redisData.FirstOrDefault(x => x.Name == "from").Value,
                            Age = (int) redisData.FirstOrDefault(x => x.Name == "age").Value,
                        });
                    ;
                }
            }
            return persons;
        }

        public string FollowPerson(string personName,string followedName)
        {
            var statement = $"MATCH (kto:Person {{name:'{personName}'}}), (kogo:Person {{name:'{followedName}'}}) CREATE (kto)-[:FOLLOW]->(kogo)";
            using (var session =_database.Driver.Session())
            {
                session.WriteTransaction(tx =>
                {
                    IStatementResult txresult = tx.Run(statement);
                });
            }
            return $"Utworzyłem powiązanie {personName} obserwuje {followedName}";
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