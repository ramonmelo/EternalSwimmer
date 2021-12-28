using UnityEngine;

/// <summary>
/// Responsible for running the game, building new levels and advancing the game
/// </summary>
public class GameManager : MonoBehaviour {

  // Level Prefab
  [SerializeField] private LevelManager _levelContainer;

  // Player Prefab
  [SerializeField] private PlayerController _playerPrefab;

  private PlayerController _localPlayer;

  // Level references
  private LevelManager _levelOld;
  private LevelManager _levelCurrent;
  private LevelManager _levelNext;

  // Level positions
  private readonly Vector3 LEVEL_POSITION_OLD = new Vector3(0, 20, 20);
  private readonly Vector3 LEVEL_POSITION_CURRENT = new Vector3(0, 0, 20);
  private readonly Vector3 LEVEL_POSITION_NEXT = new Vector3(0, -20, 20);

  private void Start() {
    StartGame();
  }

  /// <summary>
  /// Start a New Game with the base Levels.
  /// Also spawns the player into the game world
  /// </summary>
  private void StartGame() {

    Debug.Log("Starting a new Game.");

    Destroy(_levelOld);
    Destroy(_levelCurrent);
    Destroy(_levelNext);
    Destroy(_localPlayer);

    _levelOld = null;
    _levelCurrent = BuildLevel(25, 12, LEVEL_POSITION_CURRENT);
    _levelNext = BuildLevel(25, 12, LEVEL_POSITION_NEXT);

    var startPos = _levelCurrent.GetComponent<MapGenerator>().StartingPoint;

    _localPlayer = Instantiate(_playerPrefab, startPos.Position, Quaternion.identity);
  }


  /// <summary>
  /// Build a new Level based on configs
  /// </summary>
  /// <param name="numberOfObstacles">Max number of obstacles in the sub-levels</param>
  /// <param name="numberOfLevels">Number of sub-levels</param>
  /// <returns>Complete level reference</returns>
  private LevelManager BuildLevel(int numberOfObstacles, int numberOfLevels, Vector3 pos) {

    var level = Instantiate(_levelContainer);
    level.transform.position = pos;

    var map = level.GetComponent<MapGenerator>();

    do {
      map.CleanLevels();
      Debug.Log("Generating map...");
    } while (map.Generate(numberOfObstacles, numberOfLevels) == false);

    return level;
  }
}
