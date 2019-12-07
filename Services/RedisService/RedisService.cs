using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TravellerSpot.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private ConnectionMultiplexer _redisConnection;


        public RedisService(IConfiguration config)
        {
            _redisHost = config["Redis:Host"];
            _redisPort = Convert.ToInt32(config["Redis:Port"]);
        }

        public ConnectionMultiplexer RedisConnection { get => _redisConnection; }

        public void Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
                _redisConnection = ConnectionMultiplexer.Connect(configString);
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
