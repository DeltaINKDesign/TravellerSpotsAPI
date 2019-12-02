using System;
using Neo4j.Driver.V1;
using TravellerSpot.Configuration.Neo4jDatabaseSettings;

namespace TravellerSpot.Contexts
{
    public class DatabaseContext    //this service is supposed to be used only when needed.
        : IDisposable               // https://docs.microsoft.com/pl-pl/dotnet/standard/garbage-collection/implementing-dispose
    {
        public readonly IDriver Driver;

        public DatabaseContext(INeo4jDatabaseSettings databaseSettings)
        {
            Driver = GraphDatabase.Driver(databaseSettings.Uri,AuthTokens.Basic(databaseSettings.User,databaseSettings.Password));
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}