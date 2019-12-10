using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver.V1;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravellerSpot.Contexts;
using TravellerSpot.Models;
using TravellerSpot.QueryHelper;

namespace TravellerSpot.Services
{
    public class SpotService
    {

        private readonly DatabaseContext _database;
        private readonly RedisService _redisService;

        public SpotService(DatabaseContext database, RedisService redisService)
        {
            _database = database;
            _redisService = redisService;
        }
        public ActionResult<Spot> PostSpotToTrip(Spot spot, string personName, string tripName)       //pozwala na dodawanie duplikatow
        {
            string createQuery = $"MATCH(trip: Trip {{name: '{tripName}'}}) CREATE(spot: Spot {{ name: '{spot.Name}'}}), (trip)-[:INCLUDE]->(spot)";
            string duplicateCheckQuery = $"MATCH(spot: Spot {{ name: '{spot.Name}'}}) return spot";

            using (var s = _database.Driver.Session())
            {
                bool dupFound = true;
                s.ReadTransaction(tx =>
                {
                    var res = tx.Run(duplicateCheckQuery);
                    if (res.Peek() == null)  //jesli nie ma duplikatu
                    {
                        dupFound = false; //brak dup
                    }
                });
                if (dupFound) spot.Name = "DUPLIKAT";
                if (!dupFound)
                {       //tworzymy jesli nie ma duplikatu
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createQuery);
                    });
                    HashEntry[] p1 = {
                        new HashEntry("seeingCost", spot.SeeingCost),
                        new HashEntry("stars", spot.Stars)
                    };
                    if(_redisService.RedisConnection.GetDatabase().SetContains($"trips:{personName}:tripset:temporary", tripName))
                    {//transakcje redis xd
                        _redisService.RedisConnection.GetDatabase().SetRemove($"trips:{personName}:tripset:temporary", tripName);
                        _redisService.RedisConnection.GetDatabase().SetAdd($"trips:tripset", tripName);
                    }
                    _redisService.RedisConnection.GetDatabase().HashSet($"spots:{spot.Name}", p1);
                }
            }
            return spot;  //zmienic zwrot
        }

        public ActionResult<List<Spot>> GetTripSpots(string tripName)
        {
            List<Spot> spotList = new List<Spot>();
            string getQuery = $"Match(trip: Trip {{name: '{tripName}}})-[:INCLUDE]-> (Spot) return Spot";

            using (var session = _database.Driver.Session())
            {
                using var tx = session.BeginTransaction();
                IStatementResult results = tx.Run(getQuery);
                foreach (IRecord result in results)
                {
                    var node = result["Trip"].As<INode>();
                    spotList.Add(
                        new Spot
                        {
                            Name = node.Properties["Name"].As<string>(),
                            Stars = node.Properties["stars"].As<int>()
                        });
                }
                //}
                return spotList;
            }
        }
    }
}
