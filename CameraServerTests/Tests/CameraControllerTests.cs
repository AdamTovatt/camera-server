using CameraServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraServerTests.Tests
{
    [TestClass]
    internal class CameraControllerTests
    {
        [TestMethod]
        public async Task Hello()
        {
            // Arrange
            CameraController controller = new CameraController();

            // Act
            var result = await controller.Hello();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Hello World!", result);
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
