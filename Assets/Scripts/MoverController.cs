using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverController : MonoBehaviour {

  private LevelManager _levelManager;
  private PlayerController _player;

  public void Setup(LevelManager levelManager) {
    this._levelManager = levelManager;
  }

  private void Update() {
    //if (_player != null && _levelManager.IsMoving == false) {
    //  _player.Unlock();
    //  _player = null;
    //}
  }

  private void OnTriggerEnter(Collider other) {
    if (other.transform.CompareTag("Player")) {
      var pos = Camera.main.WorldToScreenPoint(transform.position);

      if (pos.x < Screen.width / 2) {
        _levelManager.RotateRight();
      } else {
        _levelManager.RotateLeft();
      }

      _player = other.gameObject.GetComponent<PlayerController>();
    }
  }
}
