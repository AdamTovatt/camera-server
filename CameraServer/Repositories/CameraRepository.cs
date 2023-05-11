using CameraServer.Helpers;
using CameraServer.Models;
using Npgsql;

namespace CameraServer.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        public static CameraRepository Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CameraRepository();
                return _instance;
            }
        }

        private static CameraRepository? _instance;
        
        private string connectionString;

        public CameraRepository()
        {
            connectionString = ConnectionStringProvider.GetConnectionString();
        }

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<CameraInformation?> GetCameraInformationByIdAsync(int id)
        {
            const string query = @"SELECT id, name, description, token, last_active
                                   FROM camera
                                   WHERE id = @id";

            using (NpgsqlConnection connection = await GetConnectionAsync())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return CameraInformation.FromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<CameraInformation>> GetAllCameraInformationsAsync()
        {
            List<CameraInformation> result = new List<CameraInformation>();

            const string query = @"SELECT id, name, description, token, last_active
                                   FROM camera";

            using (NpgsqlConnection connection = await GetConnectionAsync())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            result.Add(CameraInformation.FromReader(reader));
                        }
                    }
                }
            }

            return result;
        }

        public async Task AddCameraInformationAsync(CameraInformation cameraInformation)
        {
            const string query = @"INSERT INTO camera
                                   (name, description, token)
                                   VALUES
                                   (@name, @description, @token)";

            using (NpgsqlConnection connection = await GetConnectionAsync())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add("@name", NpgsqlTypes.NpgsqlDbType.Varchar).Value = cameraInformation.Name;
                    command.Parameters.Add("@description", NpgsqlTypes.NpgsqlDbType.Varchar).Value = cameraInformation.Description;
                    command.Parameters.Add("@token", NpgsqlTypes.NpgsqlDbType.Varchar).Value = cameraInformation.Token;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
