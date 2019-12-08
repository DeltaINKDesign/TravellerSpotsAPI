using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver.V1;
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

        public ActionResult<Trip> CreateEmpty(Trip t)       //pozwala na dodawanie duplikatow
        {
            //checking for duplicate

            var existingNodeStatement = "MATCH (v: Trip {Name: '" + t.Name + "'}) return v";

            string createNodeStatement = "CREATE (p: Trip {Name: '" + t.Name + "'}) ";
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
                if (dupFound) t.Name = "DUP";
                if(!dupFound)
                {
                    s.WriteTransaction(tx =>
                    {
                        var txresult = tx.Run(createNodeStatement);
                    });
                    _redisService.RedisConnection.GetDatabase().StringSet("Trip:" + t.Name + ":Stars", 5);
                }
            }

            return t;  //zmienic zwrot
        }
    }
}
