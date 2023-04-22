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
    }
}
