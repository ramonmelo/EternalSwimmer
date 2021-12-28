﻿using System;
using UnityEngine;

public static class BrokerSystem {

	public static event Action OnStartGame;

  public static void StartGame() => OnStartGame?.Invoke();
}