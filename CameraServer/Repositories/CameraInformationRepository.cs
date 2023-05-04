using CameraServer.Helpers;

namespace CameraServer.Repositories
{
    public class CameraInformationRepository
    {
        public static CameraInformationRepository Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CameraInformationRepository();
                return _instance;
            }
        }

        private static CameraInformationRepository? _instance;

        public CameraInformation GetCameraInformationById(int id)
        {
            return new CameraInformation(id, "Unnamed camera", "Not implemented yet");
        }
    }
}
