﻿using CameraServer.Helpers;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System.Reflection;

namespace CameraServerTests
{
    internal class TestUtilities
    {
        public static async Task<MemoryStream?> GetTestFileAsync(string filename)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

                MemoryStream memoryStream = new MemoryStream();

                using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
                {
                    using (StreamReader reader = new StreamReader(stream!))
                    {
                        await stream.CopyToAsync(memoryStream);
                        return memoryStream;
                    }
                }
            }
            catch { }
            return null;
        }

        public static IFormFile GetIFormFile(MemoryStream stream)
        {
            return new FormFile(stream, 0, stream.Length, "file", "testImage.png");
        }

        public static async Task<byte[]> ReadResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = GetFullResourceName(resourceName, assembly);

            using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Could not find the resource with the full resource name: {fullResourceName}");

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public static string GetFullResourceName(string resourceName, Assembly? assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceNames().Single(str => str.EndsWith(resourceName));
        }

        public static async Task ExecuteQueryAsync(string query)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    await command.ExecuteScalarAsync();
                }
            }
        }
    }
}
