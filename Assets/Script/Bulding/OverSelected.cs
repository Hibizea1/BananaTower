using UnityEngine;

public class OverSelected : MonoBehaviour
{
    Camera _camera;
    SpriteRenderer _img;
    Collider2D[] _hitColliders;

    void Start()
    {
        _camera = Camera.main;
        _img = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _img.enabled = false;
    }

    void Update()
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        _hitColliders = Physics2D.OverlapPointAll(mousePos);

        bool isHovering = false;
        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider is BoxCollider2D && hitCollider.gameObject == gameObject)
            {
                _img.enabled = true;
                _img.transform.Rotate(0, 0, 5 * Time.deltaTime);
                isHovering = true;
                break;
            }
        }

        if (!isHovering)
        {
            _img.enabled = false;
        }
    }
}