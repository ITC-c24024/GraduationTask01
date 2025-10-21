using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController playerCon;
    //仮
    [SerializeField] CharaScript enemySC;
    //仮
    GridManager gridManager;
    //仮
    
    [SerializeField] CellScript cellScript;

    //仮
    [SerializeField, Header("敵Prefab")]
    GameObject enemyPrefab;

    //実行中コルーチン
    Coroutine runnning = null;

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
        runnning = null;
    }

    /// <summary>
    /// ターン進行管理
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnStart()
    {
        //行動選択
        runnning = StartCoroutine(playerCon.SelectAction());

        while (runnning != null) yield return null;

        //実行ターン
        for (int i = 0; i < playerCon.actionLimit; i++)
        {
            //先行動敵

            //プレイヤー実行
            runnning = StartCoroutine(playerCon.ExecutionAct());
            //同時行動敵
            if (i < enemySC.actionLimit) StartCoroutine(enemySC.Move());

            while (runnning != null) yield return null;

            //後行動敵
        }
    }

    public Vector3 GetPlayerPos()
    {
        return playerCon.playerPos;
    }
}
