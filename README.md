# Unity Octree

A octree written to be used with the Unity Engine, for objects bounded with a Axis Aligned Bounding Box.

Can generally be used for any "thing" as long as you implement the required interface.

See unit tests for example usage.

Note that this Octree is meant to be used for a static set of data, where you are going to insert everything into the tree before querying it. The insertion will fit the item into the smallest node possible. This means the tree can be quite deep/tall and be slow for just a few items. It was written for pathfinding, where the world is full of spatial objects.

In future I'll make the tree dynamic and I may have to optimize it better to handle having only a few objects better. If objects keep moving around, that means that the tree will probably need to collapse/create nodes and have a way to do that so that queries stay optimized.
