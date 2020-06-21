//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace OctreeDS
{
	[Serializable]
	public class AABB
	{
		public Vector3 extents = Vector3.zero;

		public Vector3 center = Vector3.zero;

        public float MinX { get; }
        public float MinY { get; }
        public float MinZ { get; }
        public float MaxX { get; }
        public float MaxY { get; }
        public float MaxZ { get; }

        public Vector3 TopRight { get; }
        public Vector3 BottomLeft { get; }

        public AABB(Vector3 centre, Vector3 extents){

            foreach (var i in Enumerable.Range(0, 3))
            {
                if (extents[i] < 0) throw new ArgumentException($"Component with index {i} in extents is negative");
            }

            this.extents = extents;
			this.center = centre;
            //DrawAABB ();

            MinX = centre.x - extents.x;
            MaxX = centre.x + extents.x;

            MinY = centre.y - extents.y;
            MaxY = centre.y + extents.y;

            MinZ = centre.z - extents.z;
            MaxZ = centre.z + extents.z;

            TopRight = new Vector3(MaxX, MaxY, MaxZ);
            BottomLeft = new Vector3(MinX, MinY, MinZ);
        }

        /// <summary>
        /// Create a AABB that encloses the given points, with the given <paramref name="pointEdgeTolerance"/> padding to prevent floating point errors when considering if one of the points is in the bounds
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pointEdgeTolerance"></param>
        /// <returns></returns>
        public static AABB Create(IEnumerable<Vector3> points, float pointEdgeTolerance = 0)
        {
            var maxCorner = new Vector3(int.MinValue, int.MinValue, int.MinValue);
            var minCorner = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);

            foreach (var vectorIndex in Enumerable.Range(0,3))
            {
                maxCorner[vectorIndex] = points.Max(point => point[vectorIndex]);
                minCorner[vectorIndex] = points.Min(point => point[vectorIndex]);
            }

            var extents = (maxCorner - minCorner)/2;

            var centre = (maxCorner + minCorner) / 2;

            return new AABB(centre, extents*1.1f);
        }

		public bool Overlaps(AABB other){
			for (int i = 0; i<3; ++i)
            {
                if (center[i] < other.center[i])
                {
                    if (TopRight[i] < other.BottomLeft[i])
                    {
                        return false;
                    }
                } else if (center[i] > other.center[i])
                {
                    if (BottomLeft[i] > other.TopRight[i])
                    {
                        return false;
                    }
                }

                if (other.center[i] < center[i])
                {
                    if (other.TopRight[i] < BottomLeft[i])
                    {
                        return false;
                    }
                }
                else if (other.center[i] > center[i])
                {
                    if (other.BottomLeft[i] > TopRight[i])
                    {
                        return false;
                    }
                }
            }

            return true;
		}

        public bool Encloses(AABB other)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (other.BottomLeft[i] < BottomLeft[i]) return false;
                if (other.TopRight[i] > TopRight[i]) return false;
            }

            return true;
        }

        public bool ContainsPoint(Vector3 point)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (point[i] < BottomLeft[i] || point[i] > TopRight[i])
                    return false;
            }
            return true;
        }

		public void DrawAABB(Color color, float debugDrawTime = 1f)
        {
            var extentsToRotate = extents;
            var debugDrawDirections = new[] { -Vector3.up, -Vector3.right, -Vector3.forward }.Select(dir => Vector3.Scale(dir, extentsToRotate)).ToArray();

            foreach (var dir in MultipliersForEnumeration)
            {
                extentsToRotate = Vector3.Scale(extents, dir);
               // Debug.DrawRay(center, extentsToRotate, color, 30f);
                foreach (var boxSideDir in debugDrawDirections.Select(debugDir => Vector3.Scale(debugDir, dir)))
                {
                    Debug.DrawRay(center + extentsToRotate, boxSideDir, color, debugDrawTime);
                }
            }

            Debug.DrawLine(center, BottomLeft, color, debugDrawTime);
            Debug.DrawLine(center, TopRight, color, debugDrawTime);
        }

        private static readonly Vector3[] MultipliersForEnumeration = new[]
        {
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, -1),
            new Vector3(-1, -1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, -1, 1),
        };

        public IEnumerable<Vector3> EnumerateExtents()
        {
            return MultipliersForEnumeration.Select(dir => Vector3.Scale(dir, extents));
        }

        public override bool Equals(object obj)
        {
            return obj is AABB aABB &&
                   extents.Equals(aABB.extents) &&
                   center.Equals(aABB.center);
        }

        public override int GetHashCode()
        {
            var hashCode = -814230510;
            hashCode = hashCode * -1521134295 + extents.GetHashCode();
            hashCode = hashCode * -1521134295 + center.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"AABB: {nameof(center)}={center} {nameof(extents)}={extents}";
        }
    }
}

