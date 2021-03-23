using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Match3Game : MonoBehaviour
{
    [SerializeField] private TMP_Text timeDisplay;
    [SerializeField] private Slider progress;
    [SerializeField] private GameObject resultsPanel;

    [SerializeField] private int winCondition;
    [SerializeField] private float timeLimit;

    [SerializeField] private bool startGame = true;

    private void Start()
    {
        resultsPanel.SetActive(false);

        float camY = Board.Instance.rows;
        float camX = Board.Instance.columns;

        Camera.main.transform.position = new Vector3(camX * 0.5f, camY * 0.5f - 1.0f, -camY);
    }

    private void Update()
    {
        if (!startGame) return;

        progress.value = (float)Board.Instance.score / (float)winCondition;
        timeLimit -= Time.deltaTime;

        timeDisplay.text = ((int)timeLimit).ToString();

        if (progress.value >= 1.0f)
        {
            resultsPanel.SetActive(true);
            resultsPanel.GetComponentInChildren<TMP_Text>().text = "YOU WIN!";
            startGame = false;
        }

        if (timeLimit <= 0.0f)
        {
            resultsPanel.SetActive(true);
            resultsPanel.GetComponentInChildren<TMP_Text>().text = "YOU LOSE...";
            startGame = false;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
