using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
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
  [SerializeField, Range(1, 12)] private int NumberOfSubLevels = 1;

  private Vector2 HorizontalLimits = new Vector2(-4, 4);
  private Vector2 VerticalLimits = new Vector2(1, 18);
  private Transform[] Levels;

  public void CleanLevels() {
    var tempArray = new GameObject[LevelContainer.childCount];

    for (int i = 0; i < tempArray.Length; i++) {
      tempArray[i] = LevelContainer.GetChild(i).gameObject;
    }

    foreach (var child in tempArray) {
      DestroyImmediate(child);
    }
  }

  public void Generate() {
    SetupLevels();

    // Generate
    Node endNode = null;

    foreach (var level in Levels) {
      Node startNode;
      (startNode, endNode) = GenerateEndPoints(endNode);

      PlotNode(startNode, PrefabPlayerEnter, level);
      PlotNode(endNode, PrefabPlayerExit, level);

      var obstacles = GenerateObstacles(startNode, endNode, TotalObstacles, level);
      // var obstacles = new List<Node>();
      var newEndNode = FindPath(startNode, endNode, obstacles, level);

      if (newEndNode != null) {
        var current = newEndNode;
        while (current != null) {
          // PlotNode(current, PrefabVisited, level);

          var dir1 = GetMoveVector(current);
          var dir2 = GetMoveVector(current.Parent);
          var dot = Vector2.Dot(dir1, dir2);

          if (dot == 0 && current.Parent != null) {

            var newPos = current.Parent.Position - dir2;
            var obstacle = new Node(null, newPos);

            if (obstacles.Contains(obstacle) == false) {

              PlotNode(obstacle, PrefabBlock, level);
            }
          }

          current = current.Parent;
        }
      }
    }
  }

  private Vector2 GetMoveVector(Node node) {

    if (node != null) {
      var parentPosition = node.Parent != null ? node.Parent.Position : node.Position + Vector2.left;
      return (parentPosition - node.Position).normalized;
    }

    return Vector2.right.normalized;
  }

  private void SetupLevels() {
    CleanLevels();

    Levels = new Transform[NumberOfSubLevels];

    var diff = 20;

    for (int i = 0; i < NumberOfSubLevels; i++) {
      var level = new GameObject($"Level_{i}");
      var levelContainer = new GameObject($"Level_Container_{i}");

      level.transform.SetParent(LevelContainer);
      level.transform.localPosition = new Vector3(0, 0, diff);
      level.transform.localRotation = Quaternion.AngleAxis(-30 * i, Vector3.up);

      levelContainer.transform.SetParent(level.transform);
      levelContainer.transform.localPosition = new Vector3(0, 0, -diff);
      levelContainer.transform.localRotation = Quaternion.identity;

      Levels[i] = levelContainer.transform;
    }
  }

  private (Node, Node) GenerateEndPoints(Node oldEnd) {
    // Generate Start Node

    Node startNode;
    var startY = oldEnd != null ? oldEnd.Position.y : Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));
    var startX = HorizontalLimits.x;

    startNode = new Node(null, new Vector2(startX, startY));

    // Generate End Node
    Node endNode;
    do {
      var endY = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));
      var endX = HorizontalLimits.y;

      endNode = new Node(null, new Vector2(endX, endY));

    } while (endNode.Distance(startNode) < 15);

    return (startNode, endNode);
  }

  private List<Node> GenerateObstacles(Node startNode, Node endNode, int total, Transform level) {
    var obstacles = new List<Node>();
    var attempts = 5;

    while (obstacles.Count < total && attempts > 0) {
      var x = Random.Range(Mathf.RoundToInt(HorizontalLimits.x), Mathf.RoundToInt(HorizontalLimits.y));
      var y = Random.Range(Mathf.RoundToInt(VerticalLimits.x), Mathf.RoundToInt(VerticalLimits.y));

      var obstacle = new Node(null, new Vector2(x, y));

      if (obstacles.Contains(obstacle) == false && obstacle.Distance(startNode) > 3 && obstacle.Distance(endNode) > 3) {
        obstacles.Add(obstacle);
        attempts = 5;

        PlotNode(obstacle, PrefabBlock, level);
      } else {
        attempts--;
      }
    }

    return obstacles;
  }

  private Node FindPath(Node start, Node end, List<Node> obstables, Transform level) {
    var searchNodes = new List<Node>();

    start.GScore = 0;
    searchNodes.Add(start);

    while (searchNodes.Count > 0) {
      var currentNode = searchNodes[0];

      foreach (var node in searchNodes) {
        if (node.TotalScore(end) < currentNode.TotalScore(end)) {
          currentNode = node;
        }
      }

      if (currentNode.Equals(end)) {
        return currentNode;
      }

      searchNodes.Remove(currentNode);

      foreach (var neighboor in currentNode.Neighboors()) {
        if (neighboor.CheckBound(HorizontalLimits, VerticalLimits) == false) {
          continue;
        }

        if (obstables.Contains(neighboor)) {
          continue;
        }

        var newGScore = currentNode.GScore + 1;

        if (newGScore < neighboor.GScore) {
          neighboor.Parent = currentNode;
          neighboor.GScore = newGScore;

          if (searchNodes.Contains(neighboor) == false) {
            searchNodes.Add(neighboor);
          }
        }
      }
    }

    return null;
  }

  private void PlotNode(Node node, GameObject prefab, Transform level) {
    var item = Instantiate(prefab);

    item.transform.SetParent(level);
    item.transform.localPosition = node.Position;
    item.transform.localRotation = level.localRotation;
  }

  public class Node {
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

    public Node(Node parent, Vector2 position) {
      this.Parent = parent;
      this.Position = position;
    }

    public float TotalScore(Node targetNode) => GScore + Distance(targetNode);
    public float Distance(Node otherNode) => Mathf.Abs(this.Position.x - otherNode.Position.x) + Mathf.Abs(this.Position.y - otherNode.Position.y);

    public IEnumerable<Node> Neighboors() {
      foreach (var step in steps) {
        yield return new Node(this, this.Position + step);
      }
    }

    public bool CheckBound(Vector2 horizontalBounds, Vector2 verticalBounds) {
      if (this.Position.x < horizontalBounds.x || this.Position.x > horizontalBounds.y) {
        return false;
      }

      if (this.Position.y < verticalBounds.x || this.Position.y > verticalBounds.y) {
        return false;
      }

      return true;
    }

    public override bool Equals(object obj) {
      if (obj is Node otherNode) {
        return otherNode.Position.Equals(this.Position);
      }

      return false;
    }

    public Node Clone() {
      return new Node(null, this.Position);
    }

    public override string ToString() {
      return $"[{nameof(Node)}: {nameof(Position)}={Position}, {nameof(Parent)}={Parent?.Position}]";
    }

    public override int GetHashCode() {
      int hashCode = 1998321615;
      hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(Parent);
      hashCode = hashCode * -1521134295 + Position.GetHashCode();
      return hashCode;
    }
  }
}
