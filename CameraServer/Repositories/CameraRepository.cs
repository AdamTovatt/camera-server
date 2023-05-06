using CameraServer.Helpers;

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

        public async Task<CameraInformation> GetCameraInformationByIdAsync(int id)
        {
            await Task.CompletedTask;

            if (id == 1337)
                return new CameraInformation(id, "WebCam", "The webcam on my computer", DateTime.MinValue);

            return new CameraInformation(id, "Unnamed camera", "Not implemented yet", DateTime.UtcNow);
        }

        public async Task<List<CameraInformation>> GetAllCameraInformationsAsync()
        {
            await Task.CompletedTask;
            return new List<CameraInformation>()
            {
                new CameraInformation(1337, "CoolCam", "The camera for people who are certified Cool (TM)", DateTime.MinValue)
            };
        }
    }
}
