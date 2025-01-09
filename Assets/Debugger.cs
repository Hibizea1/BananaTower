using UnityEngine;
using UnityEngine.InputSystem;

public class Debugger : MonoBehaviour
{
    
    PlayerInput _input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _input = new PlayerInput();
    }

    void OnEnable()
    {
        _input.Player.AddMoney.performed += SetCoin;
    }

    void OnDisable()
    {
        _input.Player.AddMoney.canceled -= SetCoin;
    }

    public void SetCoin(InputAction.CallbackContext ctx)
    {
        EventMaster.GetInstance().InvokeEventInt("AddMoney", 1000);
    }
}
