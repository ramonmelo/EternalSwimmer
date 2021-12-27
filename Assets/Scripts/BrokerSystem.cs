using System;
using UnityEngine;

public static class BrokerSystem {

	
	public static event Action OnStartGame;
	
	public static event Action OnEndGame;
	
	
	public static void StartGame() {
		Debug.Log($"Running: {nameof(StartGame)}");
		OnStartGame?.Invoke();
	}
	
	public static void EndGame() {
		Debug.Log($"Running: {nameof(EndGame)}");
		OnEndGame?.Invoke();
	}
	
}