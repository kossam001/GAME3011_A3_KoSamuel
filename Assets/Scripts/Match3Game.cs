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

    private void Start()
    {
        float camY = Board.Instance.rows;
        float camX = Board.Instance.columns;

        Camera.main.transform.position = new Vector3(camX * 0.5f, camY * 0.5f - 1.0f, -camY);
    }

    private void Update()
    {
        progress.value = (float)Board.Instance.score / (float)winCondition;
    }
}
