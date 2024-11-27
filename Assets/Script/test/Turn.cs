using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    Collider2D _hit;
    [SerializeField] LayerMask layerMask;
    [SerializeField] int range;
    public int Segments = 50;
    public Color CircleColor = Color.red;
    public Material DrawMaterial;
    

    void Update()
    {
        GetObjectInRange();
    }

    private void GetObjectInRange()
    {
        _hit = Physics2D.OverlapCircle(transform.position, range, layerMask);
        if (_hit != null)
        {
            Vector3 directionToHave = _hit.transform.position - transform.position;
            var angle = Vector3.Angle(transform.right, directionToHave.normalized);
            Debug.Log(angle);
            transform.Rotate(transform.forward, angle);

        }
    }
}