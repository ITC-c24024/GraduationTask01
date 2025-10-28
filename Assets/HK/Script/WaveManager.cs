using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField, Header("ウェーブ数")]
    int waveCount = 0;

    [SerializeField, Header("ターン数")]
    int turnCount = 0;

    [SerializeField, Header("総敵数")]
    int allEnemyCount = 3;

    [SerializeField, Header("現在出現敵数")]
    int nowEnemyCount = 0;

    [SerializeField, Header("プレイヤー数")]
    int playerCount = 0;

    [SerializeField, Header("ウェーブクリア判定")]
    bool isClear = false;

    [SerializeField, Header("ゲームオーバー判定")]
    bool isGameOver = false;

    [SerializeField, Header("ターンマネージャースクリプト")]
    TurnManager turnManagerSC;

    void Start()
    {

    }

    void Update()
    {

    }

    void WaveStart()
    {
        if(allEnemyCount <= 0 && nowEnemyCount < 10)
        {
            //turnManagerSC.EnemySpown();
        }
        //StartCoroutine(turnManagerSC.TurnStart());
    }

    void TurnFinish()
    {
        if (playerCount <= 0)
        {
            GameOver();
            return;
        }
        else if (allEnemyCount <= 0)
        {
            WaveClear();
        }
    }


    void PlayerDead()
    {
        playerCount--;
        if (playerCount <= 0)
        {
            GameOver();
        }
    }

    void EnemyDead()
    {
        allEnemyCount--;
        nowEnemyCount--;
    }

    void WaveClear()
    {
        isClear = true;
    }

    void GameOver()
    {
        isGameOver = true;
    }
}
