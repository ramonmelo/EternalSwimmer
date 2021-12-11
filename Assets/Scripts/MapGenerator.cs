using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject PrefabPlayerEnter;
    [SerializeField] private GameObject PrefabPlayerExit;
    [SerializeField] private GameObject PrefabPoint;
    [SerializeField] private GameObject PrefabBlock;
    [SerializeField] private GameObject PrefabVisited;

    [SerializeField] private Transform LevelContainer;

    [SerializeField] private Vector2 ForwardChance;
    [SerializeField] private Vector2 TurnChance;
    [SerializeField] private Vector2 TerminateChance;

    [SerializeField] private int TotalObstacles = 10;

    private Vector2 HorizontalLimits = new Vector2(-4, 4);
    private Vector2 VerticalLimits = new Vector2(1, 18);

    public void Generate()
    {
        Debug.Log("Generating...");

        // Clean UP
        var tempArray = new GameObject[LevelContainer.transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = LevelContainer.transform.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            DestroyImmediate(child);
        }

        // Generate

        var (startNode, endNode) = GenerateEndPoints();

        PlotNode(startNode, PrefabPlayerEnter);
        PlotNode(endNode, PrefabPlayerExit);

        var obstacles = GenerateObstacles(TotalObstacles);

        var newEndNode = FindPath(startNode, endNode, obstacles);
    }

    private (Node, Node) GenerateEndPoints()
    {
        var startX = Random.Range(Mathf.RoundToInt(HorizontalLimits.x), Mathf.RoundToInt(HorizontalLimits.y));
        var startY = 1;

        var startNode = new Node(null, new Vector2(startX, startY));

        Node endNode;
        do
        {
            var endY = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));
            var endX = Random.value > 0.5f ? HorizontalLimits.x : HorizontalLimits.y;

            endNode = new Node(null, new Vector2(endX, endY));

        } while (endNode.Distance(startNode) < 15);

        return (startNode, endNode);
    }

    private List<Node> GenerateObstacles(int total)
    {
        var obstacles = new List<Node>();

        while (obstacles.Count < total)
        {
            var x = Random.Range(Mathf.RoundToInt(HorizontalLimits.x), Mathf.RoundToInt(HorizontalLimits.y));
            var y = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));

            var obstacle = new Node(null, new Vector2(x, y));

            if (obstacles.Contains(obstacle) == false)
            {
                obstacles.Add(obstacle);

                PlotNode(obstacle, PrefabBlock);
            }
        }

        return obstacles;
    }

    private Node FindPath(Node start, Node end, List<Node> obstables)
    {
        var searchNodes = new List<Node>();

        start.GScore = 0;
        searchNodes.Add(start);

        while (searchNodes.Count > 0)
        {
            var currentNode = searchNodes[0];

            foreach (var node in searchNodes)
            {
                if (node.TotalScore(end) < currentNode.TotalScore(end))
                {
                    currentNode = node;
                }
            }

            if (currentNode.Equals(end))
            {
                return end;
            }

            searchNodes.Remove(currentNode);

            PlotNode(currentNode, PrefabVisited);

            foreach (var neighboor in currentNode.Neighboors())
            {
                if (neighboor.CheckBound(HorizontalLimits, VerticalLimits) == false)
                {
                    continue;
                }

                if (obstables.Contains(neighboor))
                {
                    continue;
                }

                var newGScore = currentNode.GScore + 1;

                if (newGScore < neighboor.GScore)
                {
                    neighboor.Parent = currentNode;
                    neighboor.GScore = newGScore;

                    if (searchNodes.Contains(neighboor) == false)
                    {
                        searchNodes.Add(neighboor);
                    }
                }
            }
        }

        return null;
    }

    private void PlotNode(Node node, GameObject prefab)
    {
        var item = Instantiate(prefab);

        Debug.Log(node.Position);

        item.transform.localPosition = node.Position;
        item.transform.SetParent(LevelContainer);
    }

    public class Node
    {
        public float GScore { get; set; } = float.MaxValue;
        public Node Parent { get; set; }
        public Vector2 Position { get; }

        private static Vector2[] steps = new Vector2[]
        {
            Vector2.up,
            Vector2.left,
            Vector2.down,
            Vector2.right
        };

        public Node(Node parent, Vector2 position)
        {
            this.Parent = parent;
            this.Position = position;
        }

        public float TotalScore(Node targetNode) => GScore + Distance(targetNode);
        public float Distance(Node otherNode) => Mathf.Abs(this.Position.x - otherNode.Position.x) + Mathf.Abs(this.Position.y - otherNode.Position.y);

        public IEnumerable<Node> Neighboors()
        {
            foreach (var step in steps)
            {
                yield return new Node(this, this.Position + step);
            }
        }

        public bool CheckBound(Vector2 horizontalBounds, Vector2 verticalBounds)
        {
            if (this.Position.x < horizontalBounds.x || this.Position.x > horizontalBounds.y)
            {
                return false;
            }

            if (this.Position.y < verticalBounds.x || this.Position.y > verticalBounds.y)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node otherNode)
            {
                return otherNode.Position.Equals(this.Position);
            }

            return false;
        }

        public override string ToString()
        {
            return $"[{nameof(Node)}: {nameof(Position)}={Position}, {nameof(Parent)}={Parent?.Position}]";
        }

        public override int GetHashCode()
        {
            int hashCode = 1998321615;
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }
}
