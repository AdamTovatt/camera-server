using System.Security.Cryptography;
using System.Text;

namespace CameraServer.Helpers
{
    public class TokenHelper
    {
        private static string? secret = null;

        public static string GetToken(int id)
        {
            if (secret == null)
                secret = EnvironmentHelper.GetEnvironmentVariable("CAMERA_TOKEN_SECRET");

            byte[] inputBytes = Encoding.UTF8.GetBytes(id.ToString());
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

            byte[] combinedBytes = new byte[inputBytes.Length + secretBytes.Length];
            Buffer.BlockCopy(inputBytes, 0, combinedBytes, 0, inputBytes.Length);
            Buffer.BlockCopy(secretBytes, 0, combinedBytes, inputBytes.Length, secretBytes.Length);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashedBytes = sha256Hash.ComputeHash(combinedBytes);

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                    builder.Append(hashedBytes[i].ToString("x2"));

                return builder.ToString();
            }
        }
    }
}
