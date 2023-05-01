using CameraServer.Controllers;
using CameraServer.Helpers;
using CameraServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraControllerTests
    {
        [TestMethod]
        public async Task Hello()
        {
            // Arrange
            CameraController controller = new CameraController();

            // Act
            ObjectResult result = await controller.Hello();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.That.ObjectsHaveSameProperties(new { message = "Hello World!" }, result.Value);
        }

        [TestMethod]
        public async Task GetImage()
        {
            // Arrange
            CameraController controller = new CameraController();

            // Act
            var result = await controller.GetImage();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("image/jpeg", result.ContentType);
            Assert.AreEqual(239172, result.FileContents.Length); // check that the amount of bytes that were read is correct
        }

        [TestMethod]
        public async Task AssertUpdateAndGetCameraImageCorrect()
        {
            // Arrange
            CameraController controller = new CameraController();
            using MemoryStream? stream = await TestUtilities.GetTestFileAsync("MockedCameraImage");

            if (stream == null)
                Assert.Fail("Could not load MockedCameraImage");

            IFormFile image = TestUtilities.GetIFormFile(stream);

            // Act
            await controller.UpdateCameraImage(image, 1);
            FileContentResult imageFromController = await controller.GetCameraImage(1);

            // Assert
            Assert.AreEqual(239172, imageFromController.FileContents.Length);
            Assert.AreEqual("image/jpeg", imageFromController.ContentType);

            // Act2
            await controller.UpdateCameraImage(image, 2);
            FileContentResult imageFromController2 = await controller.GetCameraImage(2);

            // Assert
            Assert.AreEqual(239172, imageFromController2.FileContents.Length);
            Assert.AreEqual("image/jpeg", imageFromController2.ContentType);
        }
    }
}
