using System;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public bool _isMovement = true;
    public bool _isSpawningType = false;
    public bool _isRotation = false;
    public bool _isLooped = false;
    public bool _isForward = true;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _circleRadius = 2f;
    [SerializeField] private float _timeToRespawn = 3f;

    [SerializeField] private List<Transform> _pathPoints = new List<Transform>();
    [SerializeField] private Transform _movementObject;
    [SerializeField] private Transform _rotationObject;

    private bool _isMoving = false;
    private int _targetPoint = 0;

    private float _angle = 0f;

    private void Start()
    {
        if (_isSpawningType)
        {
            _targetPoint = UnityEngine.Random.Range(0, _pathPoints.Count);
            InvokeRepeating("Respawn", 0f, _timeToRespawn);
            return;
        }
        StartMoving();

    }

    private void FixedUpdate()
    {
        if (_isSpawningType)
            return;

        if (!_isMoving)
            return;

        if (!_isMovement)
            return;

        if (GameState.Instance.CurrentState == GameState.State.Paused)
            return;

        if (_isRotation)
            RotationMove();
        else
            DefaultMove();
    }

    public void StartMoving()
    {
        if (!_isRotation)
        {
            if (_pathPoints.Count != 0)
            {
                int i = UnityEngine.Random.Range(0, _pathPoints.Count);
                _targetPoint = i;
                _movementObject.position = _pathPoints[i].position;
            }
        }
        _isForward = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        _isMoving = true;
    }

    public void StopMoving()
    {
        _isMoving = false;
    }
    private void Respawn()
    {
        if (_pathPoints.Count == 0)
        {
            Debug.Log("Path Points is null");
            return;
        }

        if (_isForward)
        {
            if (_targetPoint == _pathPoints.Count-1)
            {
                if (_isLooped)
                {
                    _targetPoint = 0;
                }
                else
                {
                    ChangeDirection();
                    _targetPoint = _pathPoints.Count - 2;
                }
            }
            else
            {
                _targetPoint++;
            }
        }
        else
        {
            if (_targetPoint == 0)
            {
                if (_isLooped)
                {
                    _targetPoint = _pathPoints.Count - 1;
                }
                else
                {
                    ChangeDirection();
                    _targetPoint = 1;
                }
            }
            else
            {
                _targetPoint--;
            }
        }
        _movementObject.GetComponent<Fruit>().Spawn(_pathPoints[_targetPoint].position);
    }
    private void RotationMove()
    {
        if (_isForward)
        {
            _angle += _speed * Time.fixedDeltaTime;
        }
        else
        {
            _angle -= _speed * Time.fixedDeltaTime;
        }

        float radians = _angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radians) * _circleRadius;
        float y = Mathf.Sin(radians) * _circleRadius;

        Vector3 newPosition = _rotationObject.position + new Vector3(x, y, 0f);
        _movementObject.position = newPosition;
    }
    private void DefaultMove()
    {
        if (_pathPoints.Count == 0)
        {
            Debug.Log("Path Points is null");
            return;
        }
        if(_isForward && _targetPoint > _pathPoints.Count-1)
        {
            CheckMovement();
        }
        else if(!_isForward && _targetPoint < 0)
        {
            CheckMovement();
        }

        _movementObject.position = Vector2.MoveTowards(_movementObject.position, _pathPoints[_targetPoint].position, _speed * Time.fixedDeltaTime);

        if (Vector2.Distance(_movementObject.position, _pathPoints[_targetPoint].position) <= 0.1f)
        {
            if (_isForward)
            {
                if (_targetPoint == _pathPoints.Count)
                {
                    if (_isLooped)
                    {
                        _targetPoint = 0;
                    }
                    else
                    {
                        ChangeDirection();
                        _targetPoint = _pathPoints.Count - 1;
                    }
                }
                else
                    _targetPoint++;
            }
            else
            {
                if (_targetPoint == -1)
                {
                    if (_isLooped)
                    {
                        _targetPoint = _pathPoints.Count - 1;
                    }
                    else
                    {
                        ChangeDirection();
                        _targetPoint = 0;
                    }
                }
                else
                    _targetPoint--;
            }
        }
    }
    private void CheckMovement()
    {
        if (_isForward)
        {
            if (_targetPoint == _pathPoints.Count)
            {
                if (_isLooped)
                {
                    _targetPoint = 0;
                }
                else
                {
                    ChangeDirection();
                    _targetPoint = _pathPoints.Count - 1;
                }
            }
            else
                _targetPoint++;
        }
        else
        {
            if (_targetPoint == -1)
            {
                if (_isLooped)
                {
                    _targetPoint = _pathPoints.Count - 1;
                }
                else
                {
                    ChangeDirection();
                    _targetPoint = 0;
                }
            }
            else
                _targetPoint--;
        }
    }
    private void ChangeDirection()
    {
        _isForward = !_isForward;
    }
}
