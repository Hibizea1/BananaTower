using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetInformation : Singleton<SetInformation>
{
    [SerializeField] Image TurretPic;
    [SerializeField] TextMeshProUGUI TurretName;
    [SerializeField] TextMeshProUGUI TurretDamage;
    [SerializeField] TextMeshProUGUI TurretMagazine;
    [SerializeField] TextMeshProUGUI TurretReloadTime;
    [SerializeField] TextMeshProUGUI TurretCost;

    void Start()
    {
        GetComponent<Image>().color = Color.clear;
        TurretPic.enabled = false;
        TurretPic.color = Color.clear;
        TurretMagazine.text = String.Empty;
        TurretReloadTime.text = String.Empty;
        TurretCost.text = String.Empty;
        TurretDamage.text = String.Empty;
        TurretName.text = String.Empty;
    }

    public void SetPicture(Sprite value)
    {
        TurretPic.enabled = true;
        TurretPic.color = Color.white;
        TurretPic.sprite = value;
    }

    public void SetText(String turretName, int turretCost, int turretDamage = 0, int turretMagazine = 0,
        float turretReload = 0)
    {
        TurretName.text = "Item Name : " + turretName;
        if (turretDamage != 0)
            TurretDamage.text = "Turret Damage : " + turretDamage.ToString();
        if (turretMagazine != 0)
            TurretMagazine.text = "Turret Magazine : " + turretMagazine.ToString();
        if (turretReload != 0)
            TurretReloadTime.text = "Turret Reload Time : " + turretReload.ToString();
        TurretCost.text = "Item Cost : " + turretCost.ToString();
    }

    public void Show()
    {
        GetComponent<Image>().color = Color.white;
    }

    public void Hide()
    {
        TurretMagazine.text = String.Empty;
        TurretReloadTime.text = String.Empty;
        TurretCost.text = String.Empty;
        TurretDamage.text = String.Empty;
        TurretName.text = String.Empty;
        TurretPic.sprite = null;
        TurretPic.enabled = false;
        GetComponent<Image>().color = Color.clear;
    }
}