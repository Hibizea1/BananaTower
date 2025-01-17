using UnityEngine;

public class TickManager : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTickAtX1()
    {
        Time.timeScale = 1;
    }
    public void SetTickAtX1Point5()
    {
        Time.timeScale = 1.5f;
    }
    public void SetTickAtX2()
    {
        Time.timeScale = 2;
    }
}
