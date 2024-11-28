#region

using UnityEngine;

#endregion

public class OverSelected : MonoBehaviour
{
    Camera _camera;
    GameObject _currentBuilding;
    SpriteRenderer _img;

    void Start()
    {
        _camera = Camera.main;
    }


    void Update()
    {
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var hitCollider = Physics2D.OverlapPoint(mousePos);

        if (hitCollider != null && hitCollider.IsTouchingLayers(7))
        {
            if (_currentBuilding != hitCollider.gameObject)
            {
                ClearCurrentBuilding();
                _currentBuilding = hitCollider.gameObject;
                _img = _currentBuilding.transform.GetChild(0).GetComponent<SpriteRenderer>();
                _img.enabled = true;
                _img.transform.Rotate(0, 0, 5 * Time.deltaTime);
            }
        }
        else
        {
            ClearCurrentBuilding();
        }
    }

    void ClearCurrentBuilding()
    {
        if (_currentBuilding != null)
        {
            _currentBuilding.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            _currentBuilding = null;
        }
    }
}