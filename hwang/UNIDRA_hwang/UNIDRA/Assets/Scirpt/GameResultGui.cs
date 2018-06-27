﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultGui : MonoBehaviour {

    GameRuleCtrl gameRuleCtrl;

    float baseWidth = 854f;
    float baseHeight = 480f;

    public Texture2D gameOverTexture;
    public Texture2D gameClearTexture;

    private void Awake()
    {
        gameRuleCtrl = GameObject.FindObjectOfType(typeof(GameRuleCtrl)) as GameRuleCtrl;

    }
    private void OnGUI()
    {
        Texture2D aTexture;
        if (gameRuleCtrl.gameClear)
        {
            aTexture = gameClearTexture;
        }
        else if (gameRuleCtrl.gameOver)
        {
            aTexture = gameOverTexture;
        }
        else
        {
            return;
        }

        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / baseWidth, Screen.height / baseHeight, 1f));

        GUI.DrawTexture(new Rect(0.0f, 208.0f, 854.0f, 64.0f), aTexture);
    }

}