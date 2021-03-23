﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    private PlayerController player;
    private GameObject minigameScreen;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        player.Interact.AddListener(Use);
    }

    public void Use()
    {
        if (SceneManager.GetSceneByName("Match3Scene").buildIndex == -1)
        {
            SceneManager.LoadSceneAsync("Match3Scene");
        }
    }
}
