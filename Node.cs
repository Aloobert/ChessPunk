// Node.cs
using UnityEngine;

public class Node
{
    public Vector3 Position { get; private set; }
    public bool Walkable { get; set; }

    // A* Pathfinding properties
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get { return GCost + HCost; } }
    public Node Parent { get; set; }

    public Node(Vector3 position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
    }
}
