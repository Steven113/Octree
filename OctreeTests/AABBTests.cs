﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctreeDS;
using UnityEngine;

namespace OctreeTests
{
    [TestClass]
    public class AABBTests
    {
        public readonly Vector3 TestAABBDimensions = Vector3.one;
        public AABB TestAABB { get; }

        public AABBTests()
        {
            TestAABB = new AABB(Vector3.one * 2, TestAABBDimensions);
        }

        [TestMethod]
        public void ContainsPoint()
        {
            Assert.IsTrue(TestAABB.ContainsPoint(TestAABB.center), "AABB does not contain it's own centre");
            Assert.IsTrue(TestAABB.ContainsPoint(TestAABB.center + TestAABB.extents), "A point right at the corner of the AABB counts as in the AABB");
            Assert.IsFalse(TestAABB.ContainsPoint(TestAABB.center + TestAABB.extents * 2), "The point is outside the AABB");
        }

        [TestMethod]
        public void Overlaps()
        {
            Assert.IsTrue(TestAABB.Overlaps(TestAABB), "A AABB should overlap with itself");

            var overlappingAABB = new AABB(TestAABB.center + TestAABBDimensions, TestAABBDimensions);

            Assert.IsTrue(TestAABB.Overlaps(overlappingAABB), $"{TestAABB} should overlap {overlappingAABB}");

            var nonOverlappingAABB = new AABB(TestAABB.center + TestAABBDimensions * 4, TestAABBDimensions);

            Assert.IsFalse(TestAABB.Overlaps(nonOverlappingAABB), $"{TestAABB} should not overlap {nonOverlappingAABB}");
        }

        [TestMethod]
        public void Encloses()
        {
            Assert.IsTrue(TestAABB.Encloses(TestAABB), "AABB should enclose itself");

            foreach (var extent in TestAABB.EnumerateExtents())
            {
                var definatelyContainedAABB = new AABB(TestAABB.center + extent / 2, TestAABB.extents / 2);

                Assert.IsTrue(TestAABB.Encloses(definatelyContainedAABB), $"{TestAABB} should enclose {definatelyContainedAABB}");

                var definatelyNotContainedAABB = new AABB(TestAABB.center + extent / 2, TestAABB.extents);

                Assert.IsFalse(TestAABB.Encloses(definatelyNotContainedAABB), $"{TestAABB} should not enclose {definatelyNotContainedAABB}");
            }
        }

        [TestMethod]
        public void RayIntersects()
        {
            var ray = new Ray(TestAABB.center - TestAABB.extents * 2, TestAABB.extents);
            Assert.IsTrue(TestAABB.RayIntersects(ray), "Ray passes through centre, it should intersect");

            ray.direction = Vector3.right;

            Assert.IsFalse(TestAABB.RayIntersects(ray), "Ray cannot possibly pass through bounds");

            ray.origin = new Vector3(TestAABB.center.x - TestAABB.extents.x * 4, TestAABB.center.y, TestAABB.center.z);
            ray.direction = TestAABB.extents;

            Assert.IsFalse(TestAABB.RayIntersects(ray), "Ray cannot possibly pass through bounds");

            //"glancing hit"
            ray.origin = new Vector3(TestAABB.center.x - TestAABB.extents.x * 2, TestAABB.center.y, TestAABB.center.z);
            ray.direction = new Vector3(TestAABB.extents.x, 0, 0);
            Assert.IsTrue(TestAABB.RayIntersects(ray), "Ray touches the edge of the bounding box, it should intersect");
        }
    }
}
