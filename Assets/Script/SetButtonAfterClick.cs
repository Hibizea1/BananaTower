using UnityEngine;

public class SetButtonAfterClick : Singleton<SetButtonAfterClick>
{
    [SerializeField] GameObject Button;

    public void ActiveButton()
    {
        Button.SetActive(true);
    }
}
