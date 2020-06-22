using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctreeDS;
using UnityEngine;

namespace OctreeTests
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void TestNearestPoint()
        {
            var point = Vector3.one;

            //vertical line
            var testRay = new Ray(Vector3.zero, new Vector3(0f, 1f));

            var closestPoint = Utils.ClosestPointOnLine(testRay, point);
            var expectedClosestPoint = new Vector3(0f, 1f);

            Assert.AreEqual(closestPoint, expectedClosestPoint);

            //line angled at 45 degrees to x axis
            testRay = new Ray(Vector3.zero, new Vector3(1f, 1f));
            point = new Vector3(1f, 0f);

            closestPoint = Utils.ClosestPointOnLine(testRay, point);
            expectedClosestPoint = new Vector3(0.5f, 0.5f);

            Assert.AreEqual(closestPoint, expectedClosestPoint);
        }
    }
}
