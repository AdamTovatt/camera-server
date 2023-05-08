using DbUp;
using DbUp.Engine;
using System.Reflection;

namespace CameraServer.Helpers
{
    public class Migrator
    {
        public static void PerformDatabaseMigrations()
        {
            string connectionString = ConnectionStringProvider.GetConnectionString();

            UpgradeEngine upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => { return s.Contains("DatabaseMigrations") && s.Split(".").Last() == "sql"; })
                    .LogToConsole()
                    .Build();

            DatabaseUpgradeResult result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Error when performing database upgrade: " + result.Error);
            }
        }
    }
}
