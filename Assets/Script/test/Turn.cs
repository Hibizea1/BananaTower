using Unity.VisualScripting;
using UnityEngine;

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
    
    void OnRenderObject()
    {
        if (!DrawMaterial)
        {
            Debug.LogWarning("Aucun matériau assigné pour le dessin GL !");
            return;
        }

        DrawMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(CircleColor);

        float angle = 0f;
        Vector3 prevPoint = transform.position + new Vector3(0, 0, range);

        for (int i = 0; i <= Segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * range;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * range;
            Vector3 newPoint = transform.position + new Vector3(x, 0, z);

            Debug.Log($"Point {i}: {newPoint}"); // Vérifier les coordonnées

            GL.Vertex(prevPoint);
            GL.Vertex(newPoint);

            prevPoint = newPoint;
            angle += 360f / Segments;
        }

        GL.End();
    }
    
    
}