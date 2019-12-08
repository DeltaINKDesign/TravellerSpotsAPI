using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<Spot> PostSpotToTrip(Spot spot,string personName,string tripName)       //pozwala na dodawanie duplikatow
        {

            string createQuery = $"MATCH(trip: Trip {{Name: '{tripName}'}}) CREATE(spot: Spot {{ Name: '{spot.Name}'}}), (trip)-[:INCLUDE]->(spot)";
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
                if (dupFound) spot.Name= "DUPLIKAT";
                if (!dupFound)
                {       //tworzymy jesli nie ma duplikatu
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createQuery);
                    });
                    HashEntry[] p1 = {
                        new HashEntry("SeeingCost", spot.SeeingCost),
                        new HashEntry("Stars", spot.Stars)
                    };
                    _redisService.RedisConnection.GetDatabase().HashSet($"Travel:{tripName}:Spot:{spot.Name}:Hash", p1);
                }

            }

            return spot;  //zmienic zwrot
        }
    }
}
