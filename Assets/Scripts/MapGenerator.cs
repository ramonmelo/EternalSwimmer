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

    [SerializeField] private Transform[] LevelContainers;

    [SerializeField] private Vector2 ForwardChance;
    [SerializeField] private Vector2 TurnChance;
    [SerializeField] private Vector2 TerminateChance;

    [SerializeField] private int TotalObstacles = 10;

    private Vector2 HorizontalLimits = new Vector2(-4, 4);
    private Vector2 VerticalLimits = new Vector2(1, 18);

    public void CleanLevels()
    {
        // Clean UP
        foreach (var level in LevelContainers)
        {
            var tempArray = new GameObject[level.transform.childCount];

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = level.transform.GetChild(i).gameObject;
            }

            foreach (var child in tempArray)
            {
                DestroyImmediate(child);
            }
        }
    }

    public void Generate()
    {
        Debug.Log("Generating...");

        // Clean
        CleanLevels();

        // Generate
        Node endNode = null;

        foreach (var level in LevelContainers)
        {
            Node startNode;
            (startNode, endNode) = GenerateEndPoints(endNode);

            PlotNode(startNode, PrefabPlayerEnter, level);
            PlotNode(endNode, PrefabPlayerExit, level);

            var obstacles = GenerateObstacles(TotalObstacles, level);

            var newEndNode = FindPath(startNode, endNode, obstacles, level);
        }
    }

    private (Node, Node) GenerateEndPoints(Node oldEnd)
    {
        // Generate Start Node

        Node startNode;
        if (oldEnd == null)
        {
            var startY = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));
            var startX = HorizontalLimits.x;

            startNode = new Node(null, new Vector2(startX, startY));
        }
        else
        {
            startNode = oldEnd.Clone();
        }

        // Generate End Node
        Node endNode;
        do
        {
            var endY = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));
            var endX = HorizontalLimits.y;

            endNode = new Node(null, new Vector2(endX, endY));

        } while (endNode.Distance(startNode) < 15);

        return (startNode, endNode);
    }

    private List<Node> GenerateObstacles(int total, Transform level)
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

                PlotNode(obstacle, PrefabBlock, level);
            }
        }

        return obstacles;
    }

    private Node FindPath(Node start, Node end, List<Node> obstables, Transform level)
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

            PlotNode(currentNode, PrefabVisited, level);

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

    private void PlotNode(Node node, GameObject prefab, Transform level)
    {
        var item = Instantiate(prefab);

        item.transform.SetParent(level);
        item.transform.localPosition = node.Position;

        Debug.Log(item.transform.localPosition);
        Debug.Log(item.transform.position);
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

        public Node Clone()
        {
            return new Node(null, this.Position);
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
