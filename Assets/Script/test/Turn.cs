#region

using UnityEngine;

#endregion

public class Turn : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] int range;
    public int Segments = 50;
    public Color CircleColor = Color.red;
    public Material DrawMaterial;
    Collider2D _hit;


    void Update()
    {
        GetObjectInRange();
    }

    void GetObjectInRange()
    {
        _hit = Physics2D.OverlapCircle(transform.position, range, layerMask);
        if (_hit != null)
        {
            var directionToHave = _hit.transform.position - transform.position;
            var angle = Vector3.Angle(transform.right, directionToHave.normalized);
            Debug.Log(angle);
            transform.Rotate(transform.forward, angle);

        }
    }
}