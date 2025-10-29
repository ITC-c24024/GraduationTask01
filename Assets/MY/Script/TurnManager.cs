using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController[] playerCon;
    GridManager gridManager;   
    [SerializeField] CellScript cellScript;

    [SerializeField, Header("ワールドカメラ")]
    Camera worldCamera;

    //スポーンした敵のList
    List<CharaScript> enemyList = new List<CharaScript>();
    [SerializeField, Header("敵Prefab")]
    GameObject enemyPrefab;
    [SerializeField, Header("敵のHPバー")]
    Slider hpPrefab;
    [SerializeField] Canvas canvas;

    //実行中コルーチンの数
    int runnning = 0;

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    void Start()
    {        
        gridManager = gameObject.GetComponent<GridManager>();

        //仮
        Invoke("Call",2) ;      
    }
    //仮
    void Call()
    {
        for (int i = 0; i < 5; i++)
        {
            EnemySpown();
        }
        StartCoroutine(TurnStart());
    }

    /// <summary>
    /// 敵をスポーン
    /// </summary>
    /// <param name="enemyNum">スポーンさせたい敵の数</param>
    public bool EnemySpown()
    {
        //gridManagerから座標もらう
        Vector2Int spownPos = gridManager.EnemySpawnCheck(GetPlayerPos());
        if (spownPos == -Vector2.one) return false;

        //敵スポーン
        GameObject enemy = Instantiate(
            enemyPrefab,
            new Vector3(spownPos.y, enemyPrefab.transform.position.y, spownPos.x),
            enemyPrefab.transform.rotation
            );

        //HPスライダーアタッチ
        Slider slider = Instantiate(hpPrefab);
        slider.transform.SetParent(canvas.transform);

        //必要なコンポーネントを取得
        CharaScript enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;
        enemySC.hpSlider = slider;
        enemySC.worldCamera = worldCamera;
        enemySC.canvas = canvas;

        //Listに追加
        enemyList.Add(enemySC);

        //マス状態更新
        gridManager.ChangeCellState(spownPos.x, spownPos.y, CellScript.CellState.enemy, enemySC, Vector2Int.zero);

        return true;
    }

    /// <summary>
    /// コルーチン終了
    /// </summary>
    public void FinCoroutine()
    {
        runnning--;
    }

    /// <summary>
    /// 敵をプレイヤーとの距離が近い順に並べ替え
    /// </summary>
    public void SortEnemy()
    {
        //敵自身のshortDirを更新
        foreach(var enemy in enemyList)
        {
            enemy.SetShortDir();
        }

        //リストを並べ替え
        enemyList.OrderByDescending(x => x.shortDir);
    }

    /// <summary>
    /// ターン進行管理
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnStart()
    {
        //行動選択
        StartCoroutine(playerCon[0].SelectAction());
        runnning++;
        StartCoroutine(playerCon[1].SelectAction());
        runnning++;

        while (runnning != 0) yield return null;

        yield return new WaitForSeconds(0.5f);

        //距離順並べ替え
        SortEnemy();

        //敵行動
        for (int n = 0; n < enemyList.Count; n++)
        {
            //行動回数が足りるなら動く
            if (n < enemyList[n].actionLimit)
            {
                StartCoroutine(enemyList[n].Move());
                runnning++;

                while (runnning != 0) yield return null;
            }
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
