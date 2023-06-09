﻿using CameraServer.Controllers;
using CameraServer.Models;
using CameraServerTests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraControllerTests
    {
        private MockedCameraRepository cameraRepository = new MockedCameraRepository();

        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public async Task Hello()
        {
            CameraController controller = new CameraController(cameraRepository);
            ObjectResult result = await controller.Hello();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.That.ObjectsHaveSameProperties(new { message = "Hello World!" }, result.Value);
        }

        [TestMethod]
        public async Task GetCameraImage()
        {
            CameraInformation? camera1  = await cameraRepository.GetCameraInformationByIdAsync(1);
            CameraInformation? camera2 = await cameraRepository.GetCameraInformationByIdAsync(2);

            Assert.IsNotNull(camera1);
            Assert.IsNotNull(camera2);
            Assert.IsNotNull(camera1.Token);
            Assert.IsNotNull(camera2.Token);

            CameraController controller = new CameraController(cameraRepository);
            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                await controller.UpdateCameraImage(image, 1, camera1.Token);
                await controller.UpdateCameraImage(image, 2, camera2.Token);
            }

            IActionResult imageFromController = await controller.GetCameraImage(1);

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (((FileContentResult)(imageFromController)).FileContents).Length);
            Assert.AreEqual("image/jpeg", ((FileContentResult)imageFromController).ContentType);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);

            IActionResult imageFromController2 = await controller.GetCameraImage(2);

            Assert.AreEqual(239172, ((FileContentResult)imageFromController2).FileContents.Length);
            Assert.AreEqual("image/jpeg", ((FileContentResult)imageFromController2).ContentType);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }

        [TestMethod]
        public async Task GetCameraImageOverwrite()
        {
            CameraInformation? camera1 = await cameraRepository.GetCameraInformationByIdAsync(1);
            CameraInformation? camera2 = await cameraRepository.GetCameraInformationByIdAsync(2);

            Assert.IsNotNull(camera1);
            Assert.IsNotNull(camera2);
            Assert.IsNotNull(camera1.Token);
            Assert.IsNotNull(camera2.Token);

            CameraController controller = new CameraController(cameraRepository);
            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                await controller.UpdateCameraImage(image, 1, camera1.Token);
                await controller.UpdateCameraImage(image, 2, camera2.Token);
            }

            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage2"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                await controller.UpdateCameraImage(image, 1, camera1.Token);
            }

            IActionResult imageFromController = await controller.GetCameraImage(1);

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(51431, ((FileContentResult)imageFromController).FileContents.Length);
            Assert.AreEqual("image/jpeg", ((FileContentResult)imageFromController).ContentType);
            Assert.AreEqual(51431, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);

            IActionResult imageFromController2 = await controller.GetCameraImage(2);

            Assert.AreEqual(239172, ((FileContentResult)imageFromController2).FileContents.Length);
            Assert.AreEqual("image/jpeg", ((FileContentResult)imageFromController2).ContentType);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }

        [TestMethod]
        public async Task UpdateCameraImageFirstTime()
        {
            CameraInformation? camera1 = await cameraRepository.GetCameraInformationByIdAsync(1);
            CameraInformation? camera2 = await cameraRepository.GetCameraInformationByIdAsync(2);

            Assert.IsNotNull(camera1);
            Assert.IsNotNull(camera2);
            Assert.IsNotNull(camera1.Token);
            Assert.IsNotNull(camera2.Token);

            CameraController controller = new CameraController(cameraRepository);
            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                ObjectResult result1 = await controller.UpdateCameraImage(image, 1, camera1.Token);
                ObjectResult result2 = await controller.UpdateCameraImage(image, 2, camera2.Token);

                Assert.IsNotNull(result1);
                Assert.IsNotNull(result2);
                Assert.AreEqual(200, result1.StatusCode);
                Assert.AreEqual(200, result2.StatusCode);
            }

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }

        [TestMethod]
        public async Task UpdateCameraImageOverwrite()
        {
            CameraInformation? camera1 = await cameraRepository.GetCameraInformationByIdAsync(1);
            CameraInformation? camera2 = await cameraRepository.GetCameraInformationByIdAsync(2);

            Assert.IsNotNull(camera1);
            Assert.IsNotNull(camera2);
            Assert.IsNotNull(camera1.Token);
            Assert.IsNotNull(camera2.Token);

            CameraController controller = new CameraController(cameraRepository);
            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                ObjectResult result1 = await controller.UpdateCameraImage(image, 1, camera1.Token);
                ObjectResult result2 = await controller.UpdateCameraImage(image, 2, camera2.Token);

                Assert.IsNotNull(result1);
                Assert.IsNotNull(result2);
                Assert.AreEqual(200, result1.StatusCode);
                Assert.AreEqual(200, result2.StatusCode);
            }

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);

            using (MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage2"))
            {
                if (stream == null)
                    Assert.Fail("Could not load MockedCameraImage");

                IFormFile image = TestUtilities.GetIFormFile(stream);
                ObjectResult result1 = await controller.UpdateCameraImage(image, 1, camera1.Token);

                Assert.IsNotNull(result1);
                Assert.AreEqual(200, result1.StatusCode);
            }

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(51431, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }

        [TestMethod]
        public async Task GetCameraList()
        {
            CameraController controller = new CameraController(cameraRepository);
            ObjectResult result = await controller.GetCameraList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(200, result.StatusCode);

            List<CameraInformation> cameraInformation = (List<CameraInformation>)result.Value;
           
            Assert.AreEqual(2, cameraInformation.Count);
            Assert.AreEqual("Mocked Camera 1", cameraInformation.First().Name);
            Assert.AreEqual("Second Camera", cameraInformation.Last().Name);
            Assert.AreEqual("This is the first mocked camera", cameraInformation.First().Description);
            Assert.AreEqual("This is the second mocked camera", cameraInformation.Last().Description);
        }
    }
}
