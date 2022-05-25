using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    public GameObject crosshair;
    public Sprite crosshairTex;
    public Sprite crosshairReloadTex;

    private Image crosshairImage;
    private RectTransform crosshairRect;

    void Start()
    {
        crosshairImage = crosshair.GetComponent<Image>();
        crosshairRect = crosshair.GetComponent<RectTransform>();
    }

    public void SetCrossairPos(Vector3 pos)
    {
        crosshairRect.position = pos;
    }

    public void SetCrossairVisible(bool bIsVisible)
    {
        crosshairImage.enabled = bIsVisible;
    }

    public void SetReloading(bool bIsReloading)
    {
        crosshairImage.sprite = bIsReloading ? crosshairReloadTex : crosshairTex;
    }
}
