using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver.V1;
using StackExchange.Redis;
using System.Collections.Generic;
using TravellerSpot.Contexts;
using TravellerSpot.Models;

namespace TravellerSpot.Services
{
    public class TripService
    {
        private readonly DatabaseContext _database;
        private readonly RedisService _redisService;

        public TripService(DatabaseContext database, RedisService redisService)
        {
            _database = database;
            _redisService = redisService;
        }

        public ActionResult<string> CreateEmpty(Trip t, string personName)       //pozwala na dodawanie duplikatow
        {
            var createTripQuery = $"MATCH (os: Person {{name: '{personName}'}}) CREATE (co: Trip {{name: '{t.Name}', stars: '{t.Stars}'}}), (os)-[:CREATED]->(co)";
            var existingNodeStatement = "MATCH (v: Trip {name: '" + t.Name + "'}) return v";

            using (var s = _database.Driver.Session())
            {
                bool dupFound = true;
                s.ReadTransaction(tx =>
                {
                    var res = tx.Run(existingNodeStatement);
                    if (res.Peek() == null)  //jesli nie ma duplikatu
                    {
                        dupFound = false; //brak dup
                    }
                });
                if (dupFound)
                {
                    t.Name = "DUP";
                }
                if (!dupFound)
                {
                    t.Stars = 5;
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createTripQuery);
                    });

                    _redisService.RedisConnection.GetDatabase().SetAdd($"trips:{personName}:tripset:temporary", t.Name.ToString());
                }
            }

            return $"Wycieczka {t.Name} została utworzona oraz powiązana z użytkownikiem {personName}";
        }

        public ActionResult<List<Trip>> GetTripsInProgress(string p)
        {
            var data = _redisService.RedisConnection.GetDatabase().SetMembers($"trips:{p}:tripset:temporary");
            List<Trip> wycieczkiTemp = new List<Trip>();
            foreach (string d in data)
            {
                wycieczkiTemp.Add(new Trip { Name = d });
            }
            return wycieczkiTemp;
        }

        public ActionResult<List<Trip>> GetTripsFromObservedPersons(string personName)
        {
            List<Trip> trips = new List<Trip>();
            string query = $"match (:Person {{name: '{personName}'}})-[:FOLLOW]->(Person)-[:CREATED]->(Trip) return Trip  ";
            using (var session = _database.Driver.Session())
            {
                using var tx = session.BeginTransaction();
                IStatementResult results = tx.Run(query);
                foreach (IRecord result in results)
                {
                    var node = result["Trip"].As<INode>();
                    trips.Add(
                        new Trip
                        {
                            Name = node.Properties["name"].As<string>(),
                            Stars = node.Properties["stars"].As<int>()
                        });
                }
                //}
                return trips;
            }
        }

        public ActionResult<List<Trip>> GetTripsFrom(string personName, string followedName)
        {
            List<Trip> trips = new List<Trip>();
            string query = $"match (:Person {{name: '{followedName}'}})-[:CREATED]->(Trip) return Trip  ";
            using (var session = _database.Driver.Session())
            {
                using var tx = session.BeginTransaction();
                IStatementResult results = tx.Run(query);
                foreach (IRecord result in results)
                {
                    var node = result["Trip"].As<INode>();
                    trips.Add(
                        new Trip
                        {
                            Name = node.Properties["name"].As<string>(),    
                            Stars = node.Properties["stars"].As<int>()
                        });
                }
                //}
                return trips;
            }
        }

        public ActionResult<List<Trip>> GetRandomTrips()
        {
            List<Trip> trips = new List<Trip>();
            var redisdata = _redisService.RedisConnection.GetDatabase().SetRandomMembers($"trips:tripset",5);
            foreach(RedisValue rv in redisdata)
            {
                trips.Add(new Trip
                {
                    Name = rv.ToString()
                });
            }
            return trips;
        }
    } 
}
