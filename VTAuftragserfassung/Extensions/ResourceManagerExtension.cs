using System.Resources;

namespace VTAuftragserfassung.Extensions
{
    public static class ResourceManagerExtension
    {
        public static string GetQuery(this ResourceManager resM, string queryKey, params object[] args)
        {
            if (resM == null)
            {
                return string.Empty;
            }

            string query = resM.GetString(queryKey) ?? string.Empty;
            return string.Format(query, args);
        }
    }
}