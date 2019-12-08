using System.Threading.Tasks;

namespace TravellerSpot.QueryHelper
{
    public static class QueryHelper <T>
    {
        public static string CreateTemplateReturner(T entity, string key, string value) =>
            $"CREATE (v: {entity.GetType().Name} {{ {key}: '{value}' }})";

        public static string MatchTemplateReturner(T entity, string key, string value) =>
            $"MATCH (v: {entity.GetType().Name} {{ {key}: '{value}' }}) return v";
    }
}
