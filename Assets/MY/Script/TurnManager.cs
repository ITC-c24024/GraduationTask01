using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    WaveManager waveManager;
    [SerializeField] PlayerController[] playerCon;
    GridManager gridManager;   
    [SerializeField] CellScript cellScript;
    StartUIScript startUIScript;
    MoneyScript moneyScript;
    SoundManager soundManager;

    [SerializeField, Header("ワールドカメラ")]
    Camera worldCamera;

    //スポーンした敵のList
    public List<CharaScript> enemyList = new List<CharaScript>();
    [SerializeField, Header("敵の攻撃予定イメージ")]
    GameObject attackImage;
    [SerializeField, Header("敵の攻撃方向イメージPrefab")]
    GameObject arrowPrefab;
    [SerializeField, Header("敵Prefab")]
    GameObject[] enemyPrefab;
    [SerializeField, Header("敵のHPバー")]
    Slider hpPrefab;
    [SerializeField] Canvas canvas;

    //実行中コルーチンの数
    //public int runnning = 0;

    void Awake()
    {
        Application.targetFrameRate = 30;

        gridManager = gameObject.GetComponent<GridManager>();
        startUIScript = gameObject.GetComponent<StartUIScript>();
        waveManager = gameObject.GetComponent<WaveManager>();
        moneyScript = gameObject.GetComponent<MoneyScript>();
        soundManager = gameObject.GetComponent<SoundManager>();
    }

    void Start()
    {        
         
    }

    /// <summary>
    /// 敵をスポーン
    /// </summary>
    public bool EnemySpown()
    {
        //gridManagerから座標もらう
        Vector2Int spownPos = gridManager.EnemySpawnCheck(GetPlayerPos());
        if (spownPos == -Vector2.one) return false;

        GameObject enemyObj = SelectEnemy();
        //敵スポーン
        GameObject enemy = Instantiate(
            enemyObj,
            new Vector3(spownPos.y, enemyObj.transform.position.y, spownPos.x),
            enemyObj.transform.rotation
            );

        //必要なコンポーネントを取得
        CharaScript enemySC = enemy.GetComponent<CharaScript>();
        enemySC.waveManager = waveManager;
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;
        enemySC.worldCamera = worldCamera;
        enemySC.canvas = canvas;
        enemySC.moneyScript = moneyScript;
        enemySC.soundManager = soundManager;

        //Listに追加
        enemyList.Add(enemySC);

        //マス状態更新
        gridManager.ChangeCellState(spownPos.x, spownPos.y, CellScript.CellState.enemy, enemySC, Vector2Int.zero);

        return true;
    }

    /// <summary>
    /// 召喚する敵を選ぶ
    /// </summary>
    GameObject SelectEnemy()
    {
        int wave = waveManager.GetNowWave();
        if (wave <= 2)
        {
            return enemyPrefab[0];
        }
        else if(wave<=4)
        {
            int enemyNum = Random.Range(0, enemyPrefab.Length - 1);
            return enemyPrefab[enemyNum];
        }
        else
        {
            int enemyNum = Random.Range(0, enemyPrefab.Length);
            return enemyPrefab[enemyNum];
        }
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
        var sorted = enemyList.OrderBy(x => x.shortDir).ToList<CharaScript>();
        enemyList = sorted;
    }

    /// <summary>
    /// ターン進行管理
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnStart()
    {
        yield return StartCoroutine(startUIScript.SetUI(1));
        //yield return new WaitForSeconds(1.5f);

        //敵の行動予定地を設定
        for (int n = 0; n < enemyList.Count; n++)
        {
            enemyList[n].SetAction();
        }

        //プレイヤーの行動選択
        for (int i = 0; i < playerCon.Length; i++)
        {
            if (playerCon[i].alive)
            {
                yield return StartCoroutine(playerCon[i].SelectAction());
                //runnning++;

                //while (runnning != 0) yield return null;
            }
        }      

        //蘇生できるか確認
        for (int i = 0; i < playerCon.Length; i++)
        {
            if (playerCon[i].alive)
            {
                yield return StartCoroutine(playerCon[i].ResurrectionCheck());
            }
        }

        //敵がいたら
        if (enemyList.Count != 0)
        {
            //距離順並べ替え
            SortEnemy();

            yield return StartCoroutine(startUIScript.SetUI(2));

            //敵の行動
            for (int n = 0; n < enemyList.Count; n++)
            {
                yield return StartCoroutine(enemyList[n].Move());
            }

            //プレイヤーの攻撃
            for (int i = 0; i < playerCon.Length; i++)
            {
                if (playerCon[i].alive)
                {
                    yield return StartCoroutine(playerCon[i].SurrundAttack());
                    //runnning++;
                }
            }

            //while (runnning != 0) yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        //ターン終了
        waveManager.FinishTurn();
    }

    public GameObject[] GetPlayerObj()
    {
        GameObject[] player = new GameObject[]
        {
            playerCon[0].gameObject,
            playerCon[1].gameObject
        };
        return player;
    }

    public Vector3[] GetPlayerPos()
    {
        Vector3[] playerPos = new Vector3[] { 
            playerCon[0].playerPos, 
            playerCon[1].playerPos 
        };
        return playerPos;
    }

    public bool[] GetPlayerAlive()
    {
        bool[] alive = new bool[]
        {
            playerCon[0].alive,
            playerCon[1].alive
        };
        return alive;
    }
}
