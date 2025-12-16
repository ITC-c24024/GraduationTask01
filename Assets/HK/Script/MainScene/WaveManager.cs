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
    //勝手に書き換え
    MoneyScript moneyScript;
    [SerializeField] PlayerController[] playerCon;
    
    [SerializeField, Header("ウェーブ数")]
    int waveCount = 1;

    [SerializeField, Header("ターン数")]
    int turnCount = 0;

    //勝手に
    //現在のウェーブで出現する敵の総数
    int totalEnemyCount = 0;

    [SerializeField, Header("残り総敵数")]
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
        moneyScript = GetComponent<MoneyScript>();

        totalEnemyCount = allEnemyCount;
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
        yield return StartCoroutine(startUISC.SetUI(3));

        //プレイヤー蘇生
        for(int i = 0; i < playerCon.Length; i++)
        {
            if (!playerCon[i].alive) yield return StartCoroutine(playerCon[i].Resurrection());
        }

        isClear = true;
        waveCount++;
        /*
        //勝手に書き換え
        //結果を評価してお金を与える
        //与える金額=1ウェーブに出てきた敵の数÷ウェーブクリアにかかったターン数*金額倍率+最低保証
        float money = totalEnemyCount / turnCount * 10000 + 100;
        moneyScript.GetMoney((int)money, Vector3.zero);
        */
        //仮
        turnCount = 0;

        spawnEnemyCount++;
        allEnemyCount = 3 + 2 * waveCount;
        //勝手に
        //総数を保存
        totalEnemyCount = allEnemyCount;

        //仮(強化内容選択)
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(selectContentSC.SelectContent());
    }

    void GameOver()
    {
        //仮
        StartCoroutine(startUISC.SetUI(4));

        isGameOver = true;
    }
}
