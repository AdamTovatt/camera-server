using CameraServer.Helpers;

namespace CameraServer.Repositories
{
    public interface ICameraRepository
    {
        public Task<CameraInformation?> GetCameraInformationByIdAsync(int id);

        public Task<List<CameraInformation>> GetAllCameraInformationsAsync();
    }
}
