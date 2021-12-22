using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField] private LevelManager _levelContainer;
  [SerializeField] private PlayerController _playerPrefab;

  private MapGenerator _mapGenerator;
  private PlayerController _localPlayer;

  void Awake() {
    _mapGenerator = _levelContainer.GetComponent<MapGenerator>();
  }

  void Start() {
    StartLevel();
  }

  void Update() {
  }

  private void StartLevel() {

    _mapGenerator.CleanLevels();
    _mapGenerator.Setup(25, 1);

    while (_mapGenerator.Generate() == false) {
      Debug.LogError("Regenerating map...");
    }

    var startPosition = _mapGenerator.StartingPoint;

    _localPlayer = GameObject.Instantiate(_playerPrefab, startPosition.Position, Quaternion.identity);
  }
}
