using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BagaSinge : MonkeyBase
{
    Stack<Vector3Int> _path = new Stack<Vector3Int>();

    Vector3Int _currentNode;

    void Start()
    {
        _path = BuildingCreator.GetInstance().Path;
    }

    void Update()
    {
        if (_path == null)
        {
            _path = BuildingCreator.GetInstance().Path;
        }

        if (_path != null)
        {
            Move();
        }
    }


    protected override void Move()
    {
        base.Move();
        if (_path.Count > 0 && Vector3.Distance(transform.position, _currentNode) < 0.01f)
        {
            _currentNode = _path.Pop();
        }

        if (_path.Count > 0)
        {
            Vector3 direction = (_currentNode - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;
        }
    }

    public override void SpecialAbility()
    {
        throw new System.NotImplementedException();
    }
}