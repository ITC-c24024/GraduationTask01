using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //実行中コルーチンの数
    int runnning = 0;

    void Start()
    {
        //仮
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

        enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;

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

        playerCon[0].PosReset();
        playerCon[1].PosReset();

        //実行ターン
        for (int i = 0; i < playerCon[0].actionLimit; i++)
        {
            //先行動敵

            //プレイヤー実行
            StartCoroutine(playerCon[0].ExecutionAct());
            runnning++;
            StartCoroutine(playerCon[1].ExecutionAct());
            runnning++;

            //同時行動敵
            if (i < enemySC.actionLimit) StartCoroutine(enemySC.Move());

            while (runnning != 0) yield return null;

            //後行動敵
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
