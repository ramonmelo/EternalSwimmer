using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverController : MonoBehaviour {
  [SerializeField]
  private LevelManager LevelManager;

  private PlayerController _player;

  private void Update() {
    if (_player != null && LevelManager.IsMoving == false) {
      _player.Unlock();
      _player = null;
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.transform.CompareTag("Player")) {
      var pos = Camera.main.WorldToScreenPoint(transform.position);

      if (pos.x < Screen.width / 2) {
        LevelManager.RotateRight();
      } else {
        LevelManager.RotateLeft();
      }

      _player = other.gameObject.GetComponent<PlayerController>();
    }
  }
}
