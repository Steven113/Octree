using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OctreeDS
{
    public static class Utils
    {
        /// <summary>
        /// Gets the point on a given ray that is closest to the given point
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnLine(Ray ray, Vector3 point)
        {
            var rayOriginToPoint = point - ray.origin;

            /*
             * By definition, Vector3.Project gives us a vector in the same direction as the ray direction,
             * such that the end of the vector is the closest point to our target point
             */
            return ray.origin + Vector3.Project(rayOriginToPoint, ray.direction.normalized);
        }
    }
}
