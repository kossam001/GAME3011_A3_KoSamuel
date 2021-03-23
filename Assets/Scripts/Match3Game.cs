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

    }

    private void Update()
    {
        progress.value = (float)Board.Instance.score / (float)winCondition;
    }
}
