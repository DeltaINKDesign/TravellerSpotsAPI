namespace TravellerSpot.Configuration.Neo4jDatabaseSettings
{
    public interface INeo4jDatabaseSettings
    {
         string Uri { get; set; }
         string User { get; set; }
         string Password { get; set; }
    }
}