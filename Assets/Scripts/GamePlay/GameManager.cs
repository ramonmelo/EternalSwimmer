using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Responsible for running the game, building new levels and advancing the game
/// </summary>
public class GameManager : MonoBehaviour {

  /// <summary>
  /// Singleton Instance of <see cref="GameManager"/>
  /// </summary>
  public static GameManager Instance { get; private set; }

  /// <summary>
  /// Current Game State
  /// </summary>
  public GameState CurrentGameState { get; private set; }

  /// <summary>
  /// Listener Event fired when the Game State changes
  /// </summary>
  public event Action<GameState> OnGameStateChanged;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    DelayStart();
  }

  private async void DelayStart() {
    Debug.Log($"Starting...");

    await Task.Delay(500);
    Instance.ChangeGameState(GameState.GAME_SETUP);
  }

  public void ChangeGameState(GameState newGameState) {

    // Update game state
    CurrentGameState = newGameState;

    Debug.Log($"New GameState: {CurrentGameState}");

    // Handle game state change
    switch (CurrentGameState) {
      case GameState.GAME_SETUP:
        break;
      case GameState.GENERATING_WORLD:
        break;
      case GameState.START_GAME:
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

    // Notify listeners
    OnGameStateChanged?.Invoke(CurrentGameState);
  }
}

public enum GameState {
  GAME_SETUP,
  GENERATING_WORLD,
  START_GAME,
  NEXT_SUBLEVEL,
  NEXT_LEVEL,
  GAME_OVER,
}