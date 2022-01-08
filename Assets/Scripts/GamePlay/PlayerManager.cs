using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

  // Player Prefab
  [SerializeField] private PlayerController _playerPrefab;

  private PlayerController _localPlayer;

  private void Start() {
    GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
  }

  private void OnGameStateChanged(GameState gameState) {

    switch (gameState) {
      case GameState.GAME_SETUP:
        break;
      case GameState.GENERATING_WORLD:
        break;
      case GameState.START_GAME:
        SpawnPlayer();
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

  private void SpawnPlayer() {
    var pos = LevelManager.Instance.GetCurrentLevelEntryPoint();

    _localPlayer = Instantiate(_playerPrefab, pos.position, Quaternion.identity);
  }
}
