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

        public ActionResult<Trip> CreateEmpty(Trip t, string p)       //pozwala na dodawanie duplikatow
        {
            var createTripQuery = $"MATCH (os: Person {{Nick: '{p}'}}) CREATE (co: Trip {{Name: '{t.Name}'}}), (os)-[:CREATED]->(co)";
            var existingNodeStatement = "MATCH (v: Trip {Name: '" + t.Name + "'}) return v";

            using (var s = _database.Driver.Session())
            {
                bool dupFound = true;
                s.ReadTransaction(tx =>
                {
                    var res = tx.Run(existingNodeStatement);
                    if(res.Peek() == null)  //jesli nie ma duplikatu
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

                    _redisService.RedisConnection.GetDatabase().ListLeftPush($"Trips:{p}:Triplist",t.Name);
                    _redisService.RedisConnection.GetDatabase().StringSet("Trips:" + t.Name + ":Stars", t.Stars);
                }
            }

            return t;  //zmienic zwrot
        }

        public ActionResult<List<Trip>> GetTripsInProgress(string p)
        {
            var data = _redisService.RedisConnection.GetDatabase().ListRange($"Trips:{p}:Triplist", 0, -1);
            List<Trip> wycieczkiTemp = new List<Trip>();
            foreach(string d in data)
            {
                wycieczkiTemp.Add(new Trip {Name = d });
            }
            return wycieczkiTemp;
        }


    }
}
