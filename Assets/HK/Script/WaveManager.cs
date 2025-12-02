using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    //仮
    GameController gameCon;
    SelectContentScript selectContentSC;
    
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

    //仮(自身でGetComponentするためprivate)
    StartUIScript startUISC;

    void Start()
    {
        //仮
        startUISC = gameObject.GetComponent<StartUIScript>();
        selectContentSC = GetComponent<SelectContentScript>();
    }

    void Update()
    {

    }

    public void StartWave()
    {
        //仮
        StartCoroutine(startUISC.SetUI(0));
        //仮
        startUISC.SetWaveCount(waveCount);
        //仮
        startUISC.SetEnemyCount(allEnemyCount);

        //仮
        Invoke("StartTurn", 3.0f);
    }

    public void StartTurn()
    {
        turnCount++;

        for (int i = 0; i < spawnEnemyCount + (int)waveCount / 5; i++)
        {
            if (allEnemyCount-nowEnemyCount > 0 && nowEnemyCount < 20)
            {
                if (turnManagerSC.EnemySpown()) nowEnemyCount++;
            }
            else break;
        }
        StartCoroutine(turnManagerSC.TurnStart());
    }

    public void FinishTurn()
    {
        if (playerCount <= 0)
        {
            GameOver();
            return;
        }
        else if (allEnemyCount <= 0)
        {
            //仮
            StartCoroutine(WaveClear());        
            return;
        }
        else
        {
            StartTurn();
        }
    }


    public void PlayerDead()
    {
        playerCount--;
    }

    public void EnemyDead()
    {
        allEnemyCount--;
        nowEnemyCount--;

        //仮
        startUISC.SetEnemyCount(allEnemyCount);
    }

    //仮
    IEnumerator WaveClear()
    {
        //仮
        StartCoroutine(startUISC.SetUI(3));

        isClear = true;
        waveCount++;
        //仮
        turnCount = 0;

        spawnEnemyCount++;
        allEnemyCount = 3 + 2 * waveCount;

        //仮(強化内容選択)
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(selectContentSC.SelectContent());

        //仮
        //Invoke("StartWave", 3.5f);
    }

    void GameOver()
    {
        //仮
        StartCoroutine(startUISC.SetUI(4));

        isGameOver = true;
    }
}
