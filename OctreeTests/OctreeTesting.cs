using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctreeDS;
using UnityEngine;

namespace OctreeTests
{
    [TestClass]
    public class OctreeTesting
    {
        const int dimensionOfOtree = 2;
        const int minNodeWidth = 1;
        private const int distanceOfSideOfTreeFromCentre = dimensionOfOtree / 2;

        [TestMethod]
        public void ItemRetrieval()
        {
            var treeBounds = new AABB(Vector3.zero, new Vector3(distanceOfSideOfTreeFromCentre, distanceOfSideOfTreeFromCentre, distanceOfSideOfTreeFromCentre));
            var octree = new Octree<TestObject>(minNodeWidth, treeBounds);

            var insertedObjects = new List<TestObject>();

            /*
             * This will make it easier to go through all the permutations of AABBs
             */
            var testPositions = new List<Vector3>();

            #region Try putting one item in each node
            for (var x = -distanceOfSideOfTreeFromCentre + distanceOfSideOfTreeFromCentre /2f; x < distanceOfSideOfTreeFromCentre; x += minNodeWidth)
            {
                for (var y = -distanceOfSideOfTreeFromCentre + distanceOfSideOfTreeFromCentre / 2f; y < distanceOfSideOfTreeFromCentre; y += minNodeWidth)
                {
                    for (var z = -distanceOfSideOfTreeFromCentre + distanceOfSideOfTreeFromCentre / 2f; z < distanceOfSideOfTreeFromCentre; z += minNodeWidth)
                    {
                        var testPosition = new Vector3(x, y, z);
                        testPositions.Add(testPosition);

                        var testObj = new TestObject(testPosition);

                        octree.Insert(testObj);

                        insertedObjects.Add(testObj);
                    }
                }
            }
            #endregion

            #region Getting all items in tree returns correct result

            var allItemsInTree = octree.getAllContents().ToHashSet();

            foreach (var testObj in insertedObjects)
            {
                Assert.IsTrue(allItemsInTree.Contains(testObj), $"The object {testObj} was inserted into the octree but was not found when fetching all objects");
            }

            #endregion

            #region Brute force test that any bounds query returns all objects in that bounds

            for (var i = 0; i < testPositions.Count; ++i)
            {
                for (var j = i + 1; j<testPositions.Count; ++j)
                {
                    var bottomCorner = testPositions[i];
                    var upperCorner = testPositions[j];

                    var queryAABB = AABB.Create(new[] { bottomCorner, upperCorner });

                    octree.GetOverlappingItems(queryAABB, out var queryResult);
                }
            }
            #endregion

            #region Edge case: inserting object that exactly touches tree border

            #endregion

        }

        public class TestObject : IAABBBoundedObject
        {
            public AABB AABB { get; private set; }
            private Vector3 Position { get; }

            public TestObject(Vector3 position)
            {
                Position = position;
                AABB = new AABB(position, Vector3.one * minNodeWidth * 0.5f);
            }

            public override bool Equals(object obj)
            {
                return obj is TestObject @object &&
                       EqualityComparer<AABB>.Default.Equals(AABB, @object.AABB);
            }

            public override int GetHashCode()
            {
                return 119604207 + EqualityComparer<AABB>.Default.GetHashCode(AABB);
            }

            public override string ToString()
            {
                return $"Obj: {AABB}";
            }
        }
    }
}
