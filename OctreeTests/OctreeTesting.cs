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
             * this will give us objects which touch the edge of the Octree
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

            var objectsOutsideTree = new List<TestObject>();

            #region Insert objects which do not fit in the tree
            foreach (var extent in treeBounds.EnumerateExtents())
            {
                var extentDir = new Vector3(Math.Sign(extent.x), Math.Sign(extent.y), Math.Sign(extent.z));
                var offset = extentDir * TestObject.Dimensions;

                var testObj = new TestObject(treeBounds.center + extent + offset);

                octree.Insert(testObj);

                objectsOutsideTree.Add(testObj);
            }
            #endregion

            #region Getting all items in tree returns correct result

            var allItemsInTree = octree.getAllContents().ToHashSet();

            foreach (var testObj in insertedObjects)
            {
                Assert.IsTrue(allItemsInTree.Contains(testObj), $"The object {testObj} was inserted into the octree but was not found when fetching all objects");
            }

            foreach (var testObj in objectsOutsideTree)
            {
                Assert.IsFalse(allItemsInTree.Contains(testObj), $"The object {testObj} falls outside the tree bounds but was successfully inserted into the tree");
            }

            Assert.AreEqual(insertedObjects.Count, allItemsInTree.Count, "Duplicate items in tree");

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

                    var queryResultHashset = queryResult.ToHashSet();

                    foreach (var item in allItemsInTree)
                    {
                        if (queryAABB.Overlaps(item.AABB))
                        {
                            Assert.IsTrue(queryResultHashset.Contains(item), $"{item} overlaps query bounds {queryAABB} but was not found in the results");
                        }
                        else if (queryResultHashset.Contains(item))
                        {
                            Assert.Fail($"Query bounds {queryAABB} does not overlap {item} but it was in the results set");
                        }
                    }
                }
            }
            #endregion

        }

        [TestMethod]
        public void ItemDeletion()
        {
            var treeBounds = new AABB(Vector3.zero, new Vector3(distanceOfSideOfTreeFromCentre, distanceOfSideOfTreeFromCentre, distanceOfSideOfTreeFromCentre));
            var octree = new Octree<TestObject>(minNodeWidth, treeBounds);

            var insertedObjects = new List<TestObject>();

            /*
             * This will make it easier to go through all the permutations of AABBs
             */
            var testPositions = new List<Vector3>();

            #region Try putting one item in each node
            for (var x = -distanceOfSideOfTreeFromCentre + distanceOfSideOfTreeFromCentre / 2f; x < distanceOfSideOfTreeFromCentre; x += minNodeWidth)
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

            #region Delete objects one by one and make sure they are gone

            foreach (var item in insertedObjects)
            {
                Assert.IsTrue(octree.Remove(item), "Item was not successfully removed");

                var allTreeItems = octree.getAllContents();

                Assert.IsFalse(allTreeItems.Contains(item), "Tree contains item despite claiming it was removed");
            }

            Assert.IsFalse(octree.getAllContents().Any(), "Tree should have no items");
            #endregion

        }

        [TestMethod]
        public void EditObjectTest()
        {
            var treeBounds = new AABB(Vector3.zero, new Vector3(distanceOfSideOfTreeFromCentre, distanceOfSideOfTreeFromCentre * 2, distanceOfSideOfTreeFromCentre));
            var octree = new Octree<TestObject>(minNodeWidth, treeBounds);

            var testObject = new TestObject(new Vector3(treeBounds.center.x, treeBounds.center.x + treeBounds.extents.y / 2, treeBounds.center.z));

            Assert.IsTrue(octree.Insert(testObject));

            var testAABBInitial = new AABB(testObject.AABB.center, testObject.AABB.extents);
            var testAABBSecond = new AABB(new Vector3(treeBounds.center.x, treeBounds.center.x - treeBounds.extents.y / 2, treeBounds.center.z), testObject.AABB.extents);

            octree.GetOverlappingItems(testAABBInitial, out var expectInsertedItem);

            Assert.AreEqual(1, expectInsertedItem.Count, "Expected to find item just inserted and nothing else");
            Assert.IsTrue(expectInsertedItem.Contains(testObject), "Expected to find item just inserted");

            octree.GetOverlappingItems(testAABBSecond, out var expectedEmpty);
            Assert.IsTrue(!expectedEmpty.Any(), "Found item in area where there should be no items");

            octree.EditItem(testObject, obj => obj.AABB = new AABB(new Vector3(treeBounds.center.x, treeBounds.center.x - treeBounds.extents.y / 2, treeBounds.center.z), obj.AABB.extents));

            octree.GetOverlappingItems(testAABBSecond, out  expectInsertedItem);

            Assert.AreEqual(1, expectInsertedItem.Count, "Expected to find item just inserted and nothing else");
            Assert.IsTrue(expectInsertedItem.Contains(testObject), "Expected to find item just inserted");

            octree.GetOverlappingItems(testAABBInitial, out expectedEmpty);
            Assert.IsTrue(!expectedEmpty.Any(), "Found item in area where there should be no items");
        }

        public class TestObject : IAABBBoundedObject
        {
            public AABB AABB { get; set; }
            private Vector3 Position { get; }
            public const float Dimensions = 0.5f;

            public TestObject(Vector3 position)
            {
                Position = position;
                AABB = new AABB(position, Vector3.one * minNodeWidth * Dimensions);
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
