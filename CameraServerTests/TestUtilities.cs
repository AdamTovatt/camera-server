using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}
