using CameraServer.Controllers;
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
    }
}
