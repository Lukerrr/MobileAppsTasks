using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject fireButton;

    public Sprite crosshairTex;
    public Sprite crosshairReloadTex;

    private Image crosshairImage;
    private Image fireButtonImage;
    private RectTransform crosshairRect;

    void Start()
    {
        crosshairImage = crosshair.GetComponent<Image>();
        crosshairRect = crosshair.GetComponent<RectTransform>();
        fireButtonImage = fireButton.GetComponent<Image>();
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
        fireButtonImage.color = bIsReloading ? new Color(0.65f, 0.65f, 0.65f, 200f / 255f) : new Color(1f, 1f, 1f, 200f / 255f);
    }
}
