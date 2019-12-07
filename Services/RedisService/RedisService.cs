using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TravellerSpot.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private ConnectionMultiplexer _redis;


        public RedisService(IConfiguration config)
        {
            _redisHost = config["Redis:Host"];
            _redisPort = Convert.ToInt32(config["Redis:Port"]);
        }
        public void Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
                _redis = ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                Console.WriteLine(err.ToString());
                throw err;
            }
            Console.WriteLine("Connected to Redis");
        }
    }
}
