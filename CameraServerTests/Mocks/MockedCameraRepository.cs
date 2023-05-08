using CameraServer.Helpers;
using CameraServer.Repositories;
using Sakur.WebApiUtilities.Models;

namespace CameraServerTests.Mocks
{
    public class MockedCameraRepository : ICameraRepository
    {
        private List<CameraInformation> cameras = new List<CameraInformation>()
        {
            new CameraInformation(1, "Mocked Camera 1", "This is the first mocked camera", DateTime.Now),
            new CameraInformation(2, "Second Camera", "This is the second mocked camera", DateTime.Now),
        };

        public async Task<List<CameraInformation>> GetAllCameraInformationsAsync()
        {
            await Task.CompletedTask;
            return cameras;
        }

        public async Task<CameraInformation> GetCameraInformationByIdAsync(int id)
        {
            await Task.CompletedTask;

            CameraInformation? cameraInformation = cameras.Find(x => x.Id == id);

            if (cameraInformation == null)
                throw new ApiException($"Camera with id {id} not found!", System.Net.HttpStatusCode.BadRequest);
            
            return cameraInformation;
        }
    }
}
