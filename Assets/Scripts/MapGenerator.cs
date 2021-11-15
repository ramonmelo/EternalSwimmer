using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject MoverPrefab;
    public Transform LevelContainer;

    [SerializeField]
    public Vector2 ForwardChance;

    [SerializeField]
    public Vector2 TurnChance;

    private (int, int) HorizontalLimits = (-4, 4);
    private (int, int) VerticalLimits = (1, 18);

    public void Generate()
    {
        Debug.Log("Generating...");
    }

    public class Node
    {
        public Node Parent;
        public (int, int) Position;
        public List<Node> Nodes = new List<Node>();

        public Node(Node parent, (int, int) position)
        {
            this.Parent = parent;
            this.Position = position;
        }

        public Vector2 GetVectorPosition() => new Vector2(this.Position.Item1, this.Position.Item2);

        public Node GenerateChild(Vector2 step)
        {
            step.Normalize();

            var pos = (this.Position.Item1 + (int)step.x, this.Position.Item2 + (int)step.y);
            var child = new Node(this, pos);

            this.Nodes.Add(child);

            return child;
        }
    }
}
