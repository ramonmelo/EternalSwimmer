using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelManager : MonoBehaviour {

  /// <summary>
  /// Singleton Instance of <see cref="LevelManager"/>
  /// </summary>
  public static LevelManager Instance { get; private set; }

  public int CurrentSubLevel { get; private set; } = 0;

  [SerializeField] private Transform _levelContainer;
  [SerializeField] private Level _levelPrefab;

  // Level references
  private Level _levelOld;
  private Level _levelCurrent;
  private Level _levelNext;

  // Level positions
  private readonly Vector3 LEVEL_POSITION_OLD = new Vector3(0, 20, 0);
  private readonly Vector3 LEVEL_POSITION_CURRENT = new Vector3(0, 0, 0);
  private readonly Vector3 LEVEL_POSITION_NEXT = new Vector3(0, -20, 0);

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged(GameState gameState) {

    switch (gameState) {
      case GameState.GAME_SETUP:
        GenerateWorld();
        break;
      case GameState.GENERATING_WORLD:
        break;
      case GameState.START_GAME:
        Debug.Log($"Level Generated. Starting position: {GetCurrentLevelEntryPoint().position}");
        break;
      case GameState.NEXT_SUBLEVEL:
        break;
      case GameState.NEXT_LEVEL:
        break;
      case GameState.GAME_OVER:
        break;
      default:
        break;
    }
  }

  public Transform GetCurrentLevelEntryPoint() {
    Assert.IsTrue(_levelCurrent.SubLevels.Length > CurrentSubLevel);

    return _levelCurrent.SubLevels[CurrentSubLevel].startNode.WorldRepresentation;
  }

  /// <summary>
  /// Start a New Game with the base Levels.
  /// Also spawns the player into the game world
  /// </summary>
  private async void GenerateWorld() {

    // Update game state
    GameManager.Instance.ChangeGameState(GameState.GENERATING_WORLD);

    // Build first World
    Destroy(_levelOld);
    Destroy(_levelCurrent);
    Destroy(_levelNext);

    _levelOld = null;
    _levelCurrent = await BuildLevel(1, 1, LEVEL_POSITION_CURRENT);
    _levelNext = await BuildLevel(1, 1, LEVEL_POSITION_NEXT);

    // World Generated
    GameManager.Instance.ChangeGameState(GameState.START_GAME);
  }

  /// <summary>
  /// Build a new Level based on configs
  /// </summary>
  /// <param name="numberOfObstacles">Max number of obstacles in the sub-levels</param>
  /// <param name="numberOfLevels">Number of sub-levels</param>
  /// <returns>Complete level reference</returns>
  private async Task<Level> BuildLevel(int numberOfObstacles, int numberOfLevels, Vector3 pos) {

    await Task.Yield();

    var level = Instantiate(_levelPrefab);

    level.transform.SetParent(_levelContainer);
    level.transform.localPosition = pos;

    var map = level.GetComponent<Level>();

    do {
      map.CleanLevels();
      Debug.Log("Generating map...");
    } while (map.GenerateLevels(numberOfObstacles, numberOfLevels) == false);

    return level;
  }

  //public bool IsMoving => _moving;

  //// ----------- PRIVATE MEMBERS --------------------------------------------------------------------------------------

  //[SerializeField] private float Speed;

  //private Quaternion _targetRotation;
  //private Quaternion _fromRotation;

  //private bool _moving;
  //private float _timeCount = 0;
  //private int _targetRotationAngle;

  //private const int ROTATE_AMOUNT = 360 / 12;

  //void Update() {
  //  if (_moving) {
  //    transform.rotation = Quaternion.Slerp(_fromRotation, _targetRotation, _timeCount);
  //    _timeCount += Speed * Time.deltaTime;

  //    if (_timeCount > 1) {
  //      _moving = false;
  //      _timeCount = 0;

  //      transform.rotation = Quaternion.Euler(
  //          transform.rotation.x,
  //          _targetRotationAngle,
  //          transform.rotation.z);
  //    }
  //  }
  //}

  //public void RotateLeft() {
  //  if (_moving == false) {
  //    _fromRotation = transform.rotation;
  //    _targetRotationAngle += ROTATE_AMOUNT;
  //    _targetRotation = Quaternion.AngleAxis(_targetRotationAngle, Vector3.up);
  //    _moving = true;
  //  }
  //}

  //public void RotateRight() {
  //  if (_moving == false) {
  //    _fromRotation = transform.rotation;
  //    _targetRotationAngle -= ROTATE_AMOUNT;
  //    _targetRotation = Quaternion.AngleAxis(_targetRotationAngle, Vector3.up);
  //    _moving = true;
  //  }
  //}
}
