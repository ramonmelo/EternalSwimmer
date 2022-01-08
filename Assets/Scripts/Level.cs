using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible by creating a new complete Level based on some settings
/// </summary>
public class Level : MonoBehaviour {

  /// <summary>
  /// List of SubLevels
  /// </summary>
  public SubLevel[] SubLevels { get; private set; } = null;

  [SerializeField] private GameObject PrefabPlayerEnter;
  [SerializeField] private GameObject PrefabPlayerExit;
  [SerializeField] private GameObject PrefabPoint;
  [SerializeField] private GameObject PrefabBlock;
  [SerializeField] private GameObject PrefabVisited;

  private readonly Vector2 HorizontalLimits = new Vector2(-4, 4);
  private readonly Vector2 VerticalLimits = new Vector2(1, 18);

  public Node GeneratePath(SubLevel level, Node startNode, Vector2 dir) {

    var currentPos = startNode.Clone();
    var targetPos = currentPos;

    do {
      currentPos = targetPos;
      targetPos = currentPos.Move(dir);

    } while (level.Obstacles.Contains(targetPos) == false);

    return currentPos;
  }

  /// <summary>
  /// Generate a new complete Level
  /// </summary>
  /// <param name="numberOfObstacles">Max number of obstacles per sub-level</param>
  /// <param name="numberOfLevels">Number of sub-levels to be created</param>
  /// <returns>True if a valid level was create, false otherwise</returns>
  public bool GenerateLevels(int numberOfObstacles = GameConstants.MAX_OBSTACLES_PER_LEVEL, int numberOfLevels = GameConstants.MAX_SUB_LEVELS) {

    // Check limits
    numberOfObstacles = Mathf.Clamp(numberOfObstacles, 1, GameConstants.MAX_OBSTACLES_PER_LEVEL);
    numberOfLevels = Mathf.Clamp(numberOfLevels, 1, GameConstants.MAX_SUB_LEVELS);

    // Create sub-levels
    SubLevels = SetupLevels(numberOfLevels);

    // Store last start and end position
    Node endNode = null;
    Node startNode = null;

    // Generate each sub-level
    foreach (var level in SubLevels) {

      // Get start, end positions
      (startNode, endNode) = GenerateEndPoints(HorizontalLimits, VerticalLimits, endNode);

      InstantiateNode(startNode, PrefabPlayerEnter, level.Level);
      InstantiateNode(endNode, PrefabPlayerExit, level.Level);
      
      level.startNode = startNode;
      level.endNode = endNode;
      level.Obstacles = new List<Node>();

      GenerateWalls(level.Obstacles, startNode, endNode, level.Level);
      GenerateObstacles(level.Obstacles, startNode, endNode, numberOfObstacles, level.Level, HorizontalLimits, VerticalLimits);

      var newEndNode = FindPath(startNode, endNode, level.Obstacles, level.Level);

      if (newEndNode != null) {

        var current = newEndNode;
        while (current != null) {
          InstantiateNode(current, PrefabVisited, level.Level);

          var dir1 = GetMoveVector(current);
          var dir2 = GetMoveVector(current.Parent);
          var dot = Vector2.Dot(dir1, dir2);

          if (dot == 0 && current.Parent != null) {

            var newPos = current.Parent.Position - dir2;
            var obstacle = new Node(null, newPos);

            if (level.Obstacles.Contains(obstacle) == false) {

              InstantiateNode(obstacle, PrefabBlock, level.Level);
            }
          }

          current = current.Parent;
        }
      } else {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  /// Create all sub-levels
  /// </summary>
  private SubLevel[] SetupLevels(int numberOfLevels) {
    CleanLevels();

    var levels = new SubLevel[numberOfLevels];

    //parentContainer.localPosition = new Vector3(0, 0, -GameConstants.LEVEL_DIFF);

    for (int i = 0; i < numberOfLevels; i++) {

      var level = new GameObject($"Level_{i}");
      level.transform.SetParent(transform);
      level.transform.localPosition = new Vector3(0, 0, 0);
      level.transform.localRotation = Quaternion.AngleAxis(-GameConstants.LEVEL_ANGLE_DIFF * i, Vector3.up);

      var levelContainer = new GameObject($"Level_Container_{i}");
      levelContainer.transform.SetParent(level.transform);
      levelContainer.transform.localPosition = new Vector3(0, 0, -GameConstants.LEVEL_DIFF);
      levelContainer.transform.localRotation = Quaternion.identity;

      levels[i] = new SubLevel {
        Level = levelContainer.transform
      };
    }

    return levels;
  }

  /// <summary>
  /// Destroy all sub-levels and reset Starting Point
  /// </summary>
  public void CleanLevels() {
    var tempArray = new GameObject[transform.childCount];

    for (int i = 0; i < tempArray.Length; i++) {
      tempArray[i] = transform.GetChild(i).gameObject;
    }

    foreach (var child in tempArray) {
      DestroyImmediate(child);
    }
  }

  /// <summary>
  /// Create at random a new Start and End position for the path.
  /// If an old End Node is not null, the Start Position will keep the X position in order to maintain 
  /// continuity of the path
  /// </summary>
  /// <param name="oldEnd">Optional old End Node used to create the next Start Position</param>
  /// <returns>Tuple with the Start and End Position Node references</returns>
  private (Node, Node) GenerateEndPoints(Vector2 horizontalLimits, Vector2 verticalLimits, Node oldEnd = null) {
    // Generate Start Node

    Node startNode;
    var startY = oldEnd != null ? oldEnd.Position.y : Random.Range(Mathf.RoundToInt(verticalLimits.x), Mathf.RoundToInt(verticalLimits.y));
    var startX = horizontalLimits.x;

    startNode = new Node(null, new Vector2(startX, startY));

    // Generate End Node
    Node endNode;
    do {
      var endY = Random.Range(Mathf.RoundToInt(verticalLimits.x), Mathf.RoundToInt(verticalLimits.y));
      var endX = horizontalLimits.y;

      endNode = new Node(null, new Vector2(endX, endY));

    } while (endNode.Distance(startNode) < 15);

    return (startNode, endNode);
  }

  /// <summary>
  /// Create obstacles randomly scattered around the level
  /// </summary>
  /// <param name="obstacles">Current list of obstacles</param>
  /// <param name="startNode">Start position</param>
  /// <param name="endNode">End position</param>
  /// <param name="total">Max number of obstacles to be created</param>
  /// <param name="level">Parent level</param>
  private void GenerateObstacles(List<Node> obstacles, Node startNode, Node endNode, int total, Transform level, Vector2 horizontalLimits, Vector2 verticalLimits) {
    var attempts = 10;

    var startCount = obstacles.Count;

    while (obstacles.Count < (total + startCount) && attempts > 0) {
      var x = Random.Range(Mathf.RoundToInt(horizontalLimits.x), Mathf.RoundToInt(horizontalLimits.y));
      var y = Random.Range(Mathf.RoundToInt(verticalLimits.x), Mathf.RoundToInt(verticalLimits.y));

      var obstacle = new Node(null, new Vector2(x, y));

      if (obstacles.Contains(obstacle) == false && obstacle.Distance(startNode) > 2 && obstacle.Distance(endNode) > 2) {
        obstacles.Add(obstacle);

        InstantiateNode(obstacle, PrefabBlock, level);
      } else {
        attempts--;
      }
    }
  }

  /// <summary>
  /// Create all obstacles around one sub-level considering the start and end positions
  /// </summary>
  /// <param name="obstacles">Current list of obstacles</param>
  /// <param name="startNode">Start position</param>
  /// <param name="endNode">End position</param>
  /// <param name="level">Parent level</param>
  private void GenerateWalls(List<Node> obstacles, Node startNode, Node endNode, Transform level) {

    // Build start and end blocking walls

    var blockStart = new Node(null, startNode.Position + Vector2.right);
    var blockEnd = new Node(null, endNode.Position + Vector2.left);

    InstantiateNode(blockStart, PrefabBlock, level);
    obstacles.Add(blockStart);

    InstantiateNode(blockEnd, PrefabBlock, level);
    obstacles.Add(blockEnd);

    // Build vertical obstacles

    var passageStart = new Node(null, startNode.Position + Vector2.left);
    var passageEnd = new Node(null, endNode.Position + Vector2.right);

    for (int i = Mathf.RoundToInt(VerticalLimits.x); i <= Mathf.RoundToInt(VerticalLimits.y); i++) {

      var obstacleLeft = new Node(null, new Vector2(Mathf.RoundToInt(HorizontalLimits.x) - 1, i));

      if (obstacleLeft.Equals(passageStart) == false) {

        InstantiateNode(obstacleLeft, PrefabBlock, level);
        obstacles.Add(obstacleLeft);
      }

      var obstacleRight = new Node(null, new Vector2(Mathf.RoundToInt(HorizontalLimits.y) + 1, i));

      if (obstacleRight.Equals(passageEnd) == false) {

        InstantiateNode(obstacleRight, PrefabBlock, level);
        obstacles.Add(obstacleRight);
      }
    }

    // Build horizontal obstacles
    for (int i = Mathf.RoundToInt(HorizontalLimits.x) - 1; i <= Mathf.RoundToInt(HorizontalLimits.y) + 1; i++) {

      var obstacleTop = new Node(null, new Vector2(i, Mathf.RoundToInt(VerticalLimits.y) + 1));
      var obstacleBottom = new Node(null, new Vector2(i, Mathf.RoundToInt(VerticalLimits.x) - 1));

      InstantiateNode(obstacleTop, PrefabBlock, level);
      InstantiateNode(obstacleBottom, PrefabBlock, level);

      obstacles.Add(obstacleTop);
      obstacles.Add(obstacleBottom);
    }
  }

  /// <summary>
  /// Find a valid path between the start and end nodes considering the list of obstacles
  /// This is an implementation of the A* Algorithm
  /// </summary>
  /// <param name="start">Start position</param>
  /// <param name="end">End position</param>
  /// <param name="obstacles">List of obstacles</param>
  /// <param name="level">Parent level</param>
  /// <returns>End position node if a valid path was found, null otherwise</returns>
  private Node FindPath(Node start, Node end, List<Node> obstacles, Transform level) {
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

        if (obstacles.Contains(neighboor)) {
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

  /// <summary>
  /// Get the Move vector direction based on a Note and its parent
  /// </summary>
  /// <param name="node">Node to check the direction</param>
  /// <returns>Vector Direction</returns>
  private Vector2 GetMoveVector(Node node) {

    if (node != null) {
      var parentPosition = node.Parent != null ? node.Parent.Position : node.Position + Vector2.left;
      return (parentPosition - node.Position).normalized;
    }

    return Vector2.right.normalized;
  }

  /// <summary>
  /// Create a new Game Object based on a Node reference
  /// </summary>
  /// <param name="node">Node reference</param>
  /// <param name="prefab">Prefab to be created</param>
  /// <param name="level">Level to parent the new Game Object</param>
  /// <returns>GameObject reference</returns>
  private GameObject InstantiateNode(Node node, GameObject prefab, Transform level) {
    var item = Instantiate(prefab);

    item.transform.SetParent(level);
    item.transform.localPosition = node.Position;
    item.transform.localRotation = level.localRotation;

    node.WorldRepresentation = item.transform;

    return item;
  }

  public class SubLevel {
    public Transform Level;
    public List<Node> Obstacles;
    public Node startNode;
    public Node endNode;
  }

  /// <summary>
  /// Represents a position on a grid/level, and it used to represent all elements of the Level
  /// </summary>
  public class Node {

    /// <summary>
    /// Associated GameObject with this node
    /// </summary>
    public Transform WorldRepresentation;

    /// <summary>
    /// Current GScore of this Node, used by the A* Path Finder
    /// </summary>
    public float GScore { get; set; } = float.MaxValue;

    /// <summary>
    /// Parent Node of this node
    /// </summary>
    public Node Parent { get; set; }

    /// <summary>
    /// Node position
    /// </summary>
    public Vector2 Position { get; }

    // All possible moves 
    private static readonly Vector2[] steps = new Vector2[]
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

    /// <summary>
    /// Calculate the total score of this node considering a Target Node.
    /// Used by the A* path finder
    /// </summary>
    /// <param name="targetNode">Target Node</param>
    /// <returns>A* Total Score</returns>
    public float TotalScore(Node targetNode) => GScore + Distance(targetNode);

    /// <summary>
    /// Calculate the Manhattan Distance between this node and another node
    /// </summary>
    /// <param name="otherNode">Other node to check the distance</param>
    /// <returns>Manhattan Distance</returns>
    public float Distance(Node otherNode) => Mathf.Abs(this.Position.x - otherNode.Position.x) + Mathf.Abs(this.Position.y - otherNode.Position.y);

    /// <summary>
    /// Get the list nodes around this Node
    /// </summary>
    /// <returns>Enumerable list of Nodes in the neighborhood of this Node</returns>
    public IEnumerable<Node> Neighboors() {
      foreach (var step in steps) {
        yield return Move(step);
      }
    }

    /// <summary>
    /// Create a new Node by moving the current Node
    /// </summary>
    /// <param name="dir">Direction to Move</param>
    /// <returns>New Node reference using the current Node as parent</returns>
    public Node Move(Vector2 dir) {
      return new Node(this, this.Position + dir);
    }

    /// <summary>
    /// Check if the Node is out of bounds
    /// </summary>
    /// <param name="horizontalBounds">Horizontal bounds</param>
    /// <param name="verticalBounds">Vertical bounds</param>
    /// <returns>True if not out of bounds, false otherwise</returns>
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
