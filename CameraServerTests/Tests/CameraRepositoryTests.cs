using CameraServer.Models;
using CameraServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraRepositoryTests
    {
        [TestInitialize]
        public async Task BeforeEach()
        {
            await Cleanup();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await TestUtilities.ExecuteQueryAsync("TRUNCATE camera RESTART IDENTITY CASCADE");
        }

        [TestMethod]
        public async Task AddAndGetTwoCameras()
        {
            List<CameraInformation> cameras = await CameraRepository.Instance.GetAllCameraInformationsAsync();
            Assert.AreEqual(0, cameras.Count);

            CameraInformation camera1 = new CameraInformation(1, "Camera 1", "This is the first camera", DateTime.Now);
            CameraInformation camera2 = new CameraInformation(2, "Camera 2", "This is the second camera", DateTime.Now);

            await CameraRepository.Instance.AddCameraInformationAsync(camera1);
            await CameraRepository.Instance.AddCameraInformationAsync(camera2);

            cameras = await CameraRepository.Instance.GetAllCameraInformationsAsync();
            Assert.AreEqual(2, cameras.Count);

            Assert.AreEqual(camera1.Token, cameras[0].Token);
            Assert.AreEqual(camera2.Token, cameras[1].Token);

            Assert.AreEqual(camera1.Name, cameras[0].Name);
            Assert.AreEqual(camera2.Name, cameras[1].Name);

            Assert.AreEqual(camera1.Description, cameras[0].Description);
            Assert.AreEqual(camera2.Description, cameras[1].Description);
        }

        [TestMethod]
        public async Task AddAndGetSingleCamera()
        {
            List<CameraInformation> cameras = await CameraRepository.Instance.GetAllCameraInformationsAsync();
            Assert.AreEqual(0, cameras.Count);

            CameraInformation originalCamera = new CameraInformation(1, "Camera 1", "This is the first camera", DateTime.Now);

            await CameraRepository.Instance.AddCameraInformationAsync(originalCamera);

            CameraInformation? addedCamera = await CameraRepository.Instance.GetCameraInformationByIdAsync(1);

            Assert.IsNotNull(addedCamera);

            Assert.AreEqual(originalCamera.Token, addedCamera.Token);
            Assert.AreEqual(originalCamera.Token, addedCamera.Token);

            Assert.AreEqual(originalCamera.Name, addedCamera.Name);
            Assert.AreEqual(originalCamera.Name, addedCamera.Name);

            Assert.AreEqual(originalCamera.Description, addedCamera.Description);
            Assert.AreEqual(originalCamera.Description, addedCamera.Description);
        }
    }
}
