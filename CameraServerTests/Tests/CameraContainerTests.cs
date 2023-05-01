using CameraServer.Controllers;
using CameraServer.Helpers;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraServerTests.Tests
{
    [TestClass]
    public class CameraContainerTests
    {
        [TestMethod]
        public async Task AssertCameraCountWorks()
        {
            // Arrange
            CameraImage image = new CameraImage(await EmbeddedResourceHelper.ReadResource("MockedCameraImage"));
            // Act
            await CameraContainer.Instance.SetImage(1, image);

            // Assert
            Assert.AreEqual(1, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);

            // Act
            await CameraContainer.Instance.SetImage(2, image);

            // Assert
            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }
    }
}
