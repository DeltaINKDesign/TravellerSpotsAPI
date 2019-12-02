namespace TravellerSpot.Configuration.Neo4jDatabaseSettings
{
    public class Neo4jDatabaseSettings : INeo4jDatabaseSettings
    {
        public string Uri { get; set; }
        public string User { get;set; }
        public string Password { get;set; }
    }
}