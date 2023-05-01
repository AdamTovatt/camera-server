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
            CameraImage image = new CameraImage(await TestUtilities.ReadResource("MockedCameraImage"));
            CameraImage image2 =  new CameraImage(await TestUtilities.ReadResource("MockedCameraImage2"));
            await CameraContainer.Instance.SetImage(1, image);

            Assert.AreEqual(1, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);

            await CameraContainer.Instance.SetImage(2, image);

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
            
            await CameraContainer.Instance.SetImage(2, image2);

            Assert.AreEqual(2, CameraContainer.Instance.CameraCount);
            Assert.AreEqual(239172, (await (await CameraContainer.Instance.GetCameraAsync(1)).GetImageAsync()).Bytes.Length);
            Assert.AreEqual(51431, (await (await CameraContainer.Instance.GetCameraAsync(2)).GetImageAsync()).Bytes.Length);
        }
    }
}
