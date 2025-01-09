using System;
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
        _input.Player.DebugModeIncreaseMoney.performed += SetCoin;
    }

    void OnDisable()
    {
        _input.Player.DebugModeIncreaseMoney.canceled -= SetCoin;
    }

    public void SetCoin(InputAction.CallbackContext ctx)
    {
        EventMaster.GetInstance().InvokeEventInt("AddMoney", 100000);
    }
}
