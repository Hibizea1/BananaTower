#region

using UnityEngine;

#endregion

public class Node
{
    public Node(Vector3Int position)
    {
        Position = position;
    }

    public int G { get; set; }

    public int H { get; set; }

    public int F { get; set; }

    public Node Parent { get; set; }

    public Vector3Int Position { get; set; }
}