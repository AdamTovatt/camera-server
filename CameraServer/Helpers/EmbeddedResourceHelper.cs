using System.Reflection;

namespace CameraServer.Helpers
{
    public class EmbeddedResourceHelper
    {
        public static string GetFullResourceName(string resourceName, Assembly? assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceNames().Single(str => str.EndsWith(resourceName));
        }

        public static async Task<byte[]> ReadResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = GetFullResourceName(resourceName, assembly);

            using (Stream? stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Could not find the resource with the full resource name: {fullResourceName}");

                using(MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
