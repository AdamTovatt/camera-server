namespace CameraServer.Helpers
{
    public class TokenHelper
    {
        private static string? secret = null;

        public static string GetToken(int id)
        {
            if(secret == null)
                secret = EnvironmentHelper.GetEnvironmentVariable("CAMERA_TOKEN_SECRET");

            throw new NotImplementedException();
        }
    }
}
