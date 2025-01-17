using UnityEngine;

public class OverSelected : MonoBehaviour
{
    Camera _camera;
    SpriteRenderer _img;
    Collider2D[] _hitColliders;
    Turret _turret;

    void Start()
    {
        _camera = Camera.main;
        _img = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _img.enabled = false;
        _turret = GetComponent<Turret>();

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
                _img.transform.localScale =
                    new Vector3(_turret.Range / 2, _turret.Range / 2, 0);
                isHovering = true;
                _turret.SetCostText();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UpgradeTurret();
                }

                break;
            }
        }

        if (!isHovering)
        {
            _img.enabled = false;
            _turret.ExitCostText();
        }
    }

    void UpgradeTurret()
    {
        _turret.Upgrade();
    }
}