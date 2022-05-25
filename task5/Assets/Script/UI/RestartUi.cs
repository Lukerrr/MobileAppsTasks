using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartUi : MonoBehaviour
{
    public GameObject restartScreen;
    public Text restartLabel;

    public string victoryLabel = "You win";
    public string failLabel = "You lose";

    void Start()
    {
        restartScreen.SetActive(false);
    }

    public void ShowRestartScreen(bool bIsVictory)
    {
        //UnityEngine.Cursor.visible = true;
        //UnityEngine.Cursor.lockState = CursorLockMode.None;

        restartLabel.text = bIsVictory ? victoryLabel : failLabel;
        restartScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
}
