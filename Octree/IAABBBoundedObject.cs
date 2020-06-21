using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctreeDS
{
    public interface IAABBBoundedObject
    {
        AABB AABB { get; }
    }
}
