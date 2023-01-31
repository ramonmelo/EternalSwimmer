using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

  [SerializeField] private Transform Visuals;
  [SerializeField] private float Speed;
  [SerializeField] private Animator Animator;

  public Level.Node CurrentNodePosition { get; set; }

  //private Vector3 _direction = Vector3.zero;
  //private Vector3 _lastDir = Vector3.zero;
  //private Transform _movingTransform;
  //private float _animationState = 0;
  //private float _animationStateTarget = 0;

  private bool _isMoving = false;

  void Update() {

    if (_isMoving == false) {

      var moveDir = GetInputDir();

      if (moveDir != Vector2.zero) {

        var targetNode = LevelManager.Instance.ComputePath(CurrentNodePosition, moveDir);
        var targetPosition = new Vector3(targetNode.Position.x, targetNode.Position.y, transform.position.z);

        transform.DOMove(targetPosition, 1);
      }
    }

    //if (_movingTransform == null) {
    //  UpdateDirection();
    //  Move();
    //}

    // Update Visual Rotation
    //Visuals.rotation = Quaternion.LookRotation(Vector3.forward, _lastDir);

    // Update Animation
    //_animationState = Mathf.MoveTowards(_animationState, _animationStateTarget, Time.deltaTime * 10f);
    //Animator.SetFloat("SwimmingState", _animationState);
  }

  private void LateUpdate() {
    //if (_movingTransform != null) {
    //  transform.position = _movingTransform.position;
    //}
  }

  //private void Move() {
  //  transform.Translate(_direction * Speed * Time.deltaTime, Space.World);
  //}

  private Vector2 GetInputDir() {

    if (Input.GetKeyDown(KeyCode.UpArrow)) {
      return Vector2.up;
    }
    if (Input.GetKeyDown(KeyCode.DownArrow)) {
      return Vector2.down;
    }
    if (Input.GetKeyDown(KeyCode.LeftArrow)) {
      return Vector2.left;
    }
    if (Input.GetKeyDown(KeyCode.RightArrow)) {
      return Vector2.right;
    }

    return Vector2.zero;
  }

  //private void OnCollisionEnter(Collision collision) {
  //  Stop();
  //}

  //private void OnTriggerEnter(Collider other) {
  //  Stop();

  //  Debug.Log($"OnTriggerEnter: {other}");

  //  if (other.transform.CompareTag("Mover")) {
  //    Lock(other.transform);
  //  }
  //}

  //public void Lock(Transform target) {
  //  Debug.Log($"LOCK: {target}");

  //  _movingTransform = target;
  //}

  //public void Unlock() {
  //  _direction = _lastDir;
  //  _movingTransform = null;
  //}

  //private void Stop() {
  //  // Reset Position
  //  transform.position = new Vector3(
  //                          Mathf.RoundToInt(transform.position.x),
  //                          Mathf.RoundToInt(transform.position.y),
  //                          transform.position.z);

  //  // Reset Direction
  //  _direction = Vector3.zero;
  //}
}
