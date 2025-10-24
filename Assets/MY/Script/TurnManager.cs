using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController[] playerCon;
    //仮
    [SerializeField] CharaScript enemySC;
    //仮
    GridManager gridManager;
    
    [SerializeField] CellScript cellScript;

    //仮
    [SerializeField, Header("敵Prefab")]
    GameObject enemyPrefab;
    [SerializeField, Header("敵のHPバー")]
    Slider hpPrefab;
    [SerializeField] Canvas canvas;

    //実行中コルーチンの数
    int runnning = 0;

    void Start()
    {
        //仮
        Application.targetFrameRate = 30;
        gridManager = gameObject.GetComponent<GridManager>();
        EnemySpown();
        StartCoroutine(TurnStart());
    }

    //仮
    void EnemySpown()
    {
        GameObject enemy = Instantiate(
           enemyPrefab,
           new Vector3(7, 0.5f, 7),
           enemyPrefab.transform.rotation
           );

        Slider slider = Instantiate(hpPrefab);
        slider.transform.SetParent(canvas.transform);

        enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;
        enemySC.hpSlider = slider;
    }

    /// <summary>
    /// コルーチン終了
    /// </summary>
    public void FinCoroutine()
    {
        runnning--;
    }

    /// <summary>
    /// ターン進行管理
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnStart()
    {
        //行動選択
        StartCoroutine(playerCon[0].SelectAction());
        runnning++;
        StartCoroutine(playerCon[1].SelectAction());
        runnning++;

        while (runnning != 0) yield return null;

        yield return new WaitForSeconds(0.5f);

        playerCon[0].PosReset();
        playerCon[1].PosReset();

        yield return new WaitForSeconds(1.0f);

        //実行ターン
        for (int i = 0; i < playerCon[0].actionLimit || i < playerCon[1].actionLimit; i++)
        {
            //先行動敵

            while (runnning != 0) yield return null;

            yield return new WaitForSeconds(0.5f);

            //プレイヤー実行
            if(i < playerCon[0].actionLimit)
            {
                StartCoroutine(playerCon[0].ExecutionAct(i));
                runnning++;
            }
            if(i < playerCon[1].actionLimit)
            {
                StartCoroutine(playerCon[1].ExecutionAct(i));
                runnning++;
            }          

            while (runnning != 0) yield return null;

            yield return new WaitForSeconds(0.5f);

            //後行動敵
            if (i < enemySC.actionLimit)
            {
                StartCoroutine(enemySC.Move());
                runnning++;
            }

            while (runnning != 0) yield return null;

            //予約をすべて削除
            gridManager.ResetReserveListAll();
        }
    }

    public Vector3[] GetPlayerPos()
    {
        Vector3[] playerPos = new Vector3[] { 
            playerCon[0].playerPos, 
            playerCon[1].playerPos 
        };
        return playerPos;
    }
}
