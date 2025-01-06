using System;
using System.Collections.Generic;
using UnityEngine;

public class BagaSinge : MonkeyBase
{
    List<Vector3Int> _path = new List<Vector3Int>();
    [SerializeField] List<Vector3Int> Path = new List<Vector3Int>();
    [SerializeField] Vector3Int _currentNode;

    void Start()
    {
        InitializePath();
    }

    void Update()
    {
        if (_path == null || _path.Count == 0)
        {
            InitializePath();
        }

        if (_path != null && _path.Count > 0)
        {
            Move();
        }
    }

    void InitializePath()
    {
        _path = new List<Vector3Int>(BuildingCreator.GetInstance().Path);
        Path = new List<Vector3Int>(_path);
        if (_path.Count > 0)
        {
            _currentNode = _path[0];
            _path.RemoveAt(0);
        }
        else
        {
            Debug.LogWarning("Path is empty for " + transform.name);
        }
    }

    protected override void Move()
    {
        base.Move();
        if (_path.Count > 0 && Vector3.Distance(transform.position, _currentNode + new Vector3(0.5f, 0.5f, 0)) < 0.01f)
        {
            _currentNode = _path[0];
            _path.RemoveAt(0);
        }

        if (_path.Count > 0)
        {
            Vector3 direction = (_currentNode + new Vector3(0.5f, 0.5f, 0) - transform.position).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }
        else
        {
            Debug.LogWarning("No more nodes to follow for " + transform.name);
        }
    }

    public override void SpecialAbility()
    {
        throw new NotImplementedException();
    }
}