using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class WaveManager : MonoBehaviour
{
    [SerializeField, Header("ウェーブ数")]
    int waveCount = 1;

    [SerializeField, Header("ターン数")]
    int turnCount = 0;

    [SerializeField, Header("総敵数")]
    int allEnemyCount = 3;

    [SerializeField, Header("現在出現敵数")]
    int nowEnemyCount = 0;

    [SerializeField, Header("ターンごとの出現敵数")]
    int spawnEnemyCount = 1;

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

    void StartTurn()
    {
        turnCount++;

        for (int i = 0; i < spawnEnemyCount + (int)waveCount / 5; i++)
        {
            if (allEnemyCount <= 0 && nowEnemyCount < 10)
            {
                if (turnManagerSC.EnemySpown())
                {
                    nowEnemyCount++;
                }
            }
            else
            {
                break;
            }
        }
        StartCoroutine(turnManagerSC.TurnStart());
    }

    void FinishTurn()
    {
        if (playerCount <= 0)
        {
            GameOver();
            return;
        }
        else if (allEnemyCount <= 0)
        {
            WaveClear();
            return;
        }
        else
        {
            StartTurn();
        }
    }


    void PlayerDead()
    {
        playerCount--;
    }

    void EnemyDead()
    {
        allEnemyCount--;
        nowEnemyCount--;
    }

    void WaveClear()
    {
        isClear = true;
        waveCount++;
        turnCount--;
        allEnemyCount = 3 + 2 * waveCount;

    }

    void GameOver()
    {
        isGameOver = true;
    }
}
