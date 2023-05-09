using CameraServer.Helpers;

namespace CameraServerTests
{
    [TestClass]
    public class Environment
    {
        private static string connectionString = "postgres://postgres:postgres@localhost:5432/camera_server_test";

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            if (File.Exists("connectionString.txt")) // if you add a file called "connectionString.txt" in \CameraServerTests\bin\Debug\net6.0 it will use that
            {
                connectionString = File.ReadAllText("connectionString.txt");
            }

            SetupTestEnvironmentVariables();

            Migrator.PerformDatabaseMigrations();
        }

        private static void SetupTestEnvironmentVariables()
        {
            System.Environment.SetEnvironmentVariable("DATABASE_URL", connectionString);
            System.Environment.SetEnvironmentVariable("CAMERA_TOKEN_SECRET", "NOTsoSECRETsecretUsedFORtESTS1234908");
        }

        [TestMethod]
        public void AssertTestEnvironment()
        {
            Assert.AreEqual(connectionString, EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"));
        }
    }
}
