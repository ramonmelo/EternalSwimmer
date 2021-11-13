using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    public bool IsMoving => _moving;

    private Quaternion _targetRotation;
    private Quaternion _fromRotation;
    private bool _moving;
    private float _timeCount = 0;
    private int _targetRotationAngle;

    private const int ROTATE_AMOUNT = 90;

    void Update()
    {
        if (_moving)
        {
            transform.rotation = Quaternion.Slerp(_fromRotation, _targetRotation, _timeCount);
            _timeCount += Speed * Time.deltaTime;

            if (_timeCount > 1)
            {
                _moving = false;
                _timeCount = 0;

                transform.rotation = Quaternion.Euler(
                    transform.rotation.x,
                    _targetRotationAngle,
                    transform.rotation.z);
            }
        }
    }

    public void RotateLeft()
    {
        if (_moving == false)
        {
            _fromRotation = transform.rotation;
            _targetRotationAngle += ROTATE_AMOUNT;
            _targetRotation = Quaternion.AngleAxis(_targetRotationAngle, Vector3.up);
            _moving = true;
        }
    }

    public void RotateRight()
    {
        if (_moving == false)
        {
            _fromRotation = transform.rotation;
            _targetRotationAngle -= ROTATE_AMOUNT;
            _targetRotation = Quaternion.AngleAxis(_targetRotationAngle, Vector3.up);
            _moving = true;
        }
    }
}
