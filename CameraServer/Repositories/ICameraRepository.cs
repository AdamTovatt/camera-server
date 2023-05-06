using CameraServer.Helpers;

namespace CameraServer.Repositories
{
    public interface ICameraRepository
    {
        public Task<CameraInformation> GetCameraInformationById(int id);

        public Task<List<CameraInformation>> GetAllCameraInformationsAsync();
    }
}
