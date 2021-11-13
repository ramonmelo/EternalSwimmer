using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform Visuals;

    [SerializeField]
    private float Speed;

    private Vector3 _direction = Vector3.zero;
    Vector3 _lastDir = Vector3.zero;

    private Rigidbody _rb;
    private Transform _movingTransform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_movingTransform == null)
        {
            UpdateDirection();
            Move();
        }

        Visuals.rotation = Quaternion.LookRotation(Vector3.forward, _lastDir);
    }

    private void LateUpdate()
    {
        if (_movingTransform != null)
        {
            transform.position = _movingTransform.position;
        }
    }

    private void Move()
    {
        transform.Translate(_direction * Speed * Time.deltaTime, Space.World);
    }

    private void UpdateDirection()
    {
        if (_direction != Vector3.zero)
        {
            _lastDir = _direction;
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _direction = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _direction = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _direction = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _direction = Vector3.right;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        Stop();

        Debug.Log($"OnTriggerEnter: {other}");

        if (other.transform.CompareTag("Mover"))
        {
            Lock(other.transform);
        }
    }

    public void Lock(Transform target)
    {
        Debug.Log($"LOCK: {target}");

        _movingTransform = target;
    }

    public void Unlock()
    {
        _direction = _lastDir;
        _movingTransform = null;
    }

    private void Stop()
    {
        // Reset Position
        transform.position = new Vector3(
                                Mathf.RoundToInt(transform.position.x),
                                Mathf.RoundToInt(transform.position.y),
                                transform.position.z);

        // Reset Direction
        _direction = Vector3.zero;
    }
}
