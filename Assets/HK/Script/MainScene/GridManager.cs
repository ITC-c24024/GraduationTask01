using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;

    [SerializeField, Header("マスプレハブ")]
    GameObject cellPrefab;

    CellScript[,] cellSC;

    void Awake()
    {
        cellSC = new CellScript[width, height];//配列初期化

        //マップ生成＆各マススクリプト取得
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cellObj = Instantiate(cellPrefab, new Vector3(x, 0.05f, y), Quaternion.identity);
                cellSC[y, x] = cellObj.GetComponent<CellScript>();
                cellSC[y, x].SetPosition(new Vector2Int(y, x));
                cellSC[y, x].gridManagerSC = this;
            }
        }
    }

    /// <summary>
    /// 指定したマスの状態を変更する
    /// </summary>
    /// <param name="y">y座標(縦の列)</param>
    /// <param name="x">x座標(横の列)</param>
    /// <param name="state">状態(empty=空,player=プレイヤー,enemy=敵,damageTile=ダメージ床)</param>
    /// <param name="unitSC">呼びだす側のユニットスクリプト</param>
    /// <param name="direction">エントリの方向</param>
    public CellScript.TryEnterResult ChangeCellState(int y, int x, CellScript.CellState state, CharaScript unitSC, Vector2Int direction)
    {
        return cellSC[y, x].TryEnter(state, unitSC, direction);
    }

    /// <summary>
    /// 指定したマスの状態を確認する
    /// </summary>
    /// <param name="y">y座標(縦の列)</param>
    /// <param name="x">x座標(横の列)</param>
    /// <returns>現在のマスの状態</returns>
    public CellScript.CellState CheckCellState(int y, int x)
    {
        return cellSC[y, x].CheckState();
    }

    /// <summary>
    /// 指定したマスから離れる
    /// </summary>
    /// <param name="y">y座標</param>
    /// <param name="x">x座標</param>
    /// <param name="unitSC">ユニットのスクリプト</param>
    public void LeaveCell(int y, int x, CharaScript unitSC)
    {
        cellSC[y, x].Leave(unitSC);
    }

    /// <summary>
    /// 指定したマスに対して攻撃
    /// </summary>
    /// <param name="y">y座標</param>
    /// <param name="x">x座標</param>
    /// <param name="damage">ダメージ量</param>
    public void SendDamage(int y, int x, int damage,bool isEnemy, Vector2Int direction = default)
    {
        cellSC[y, x].ReciveAttack(damage,isEnemy,direction);
    }

    //勝手に書き換え
    /// <summary>
    /// 指定したマスをダメージマス状態にする
    /// </summary>
    /// <param name="y">y座標</param>
    /// <param name="x">x座標</param>
    public void SetDamageState(int y,int x)
    {
        cellSC[y, x].haveDamage = true;
    }

    /// <summary>
    /// 敵スポーンの位置を決める
    /// </summary>
    /// <param name="playerPos">プレイヤー座標リスト</param>
    /// <returns>スポーン座標</returns>
    public Vector2Int EnemySpawnCheck(Vector3[] playerPos, int minDistance=1)
    {
        List<Vector2Int> playerPosList = new();//プレイヤー座標リスト(Vector2Int変換後用)
        List<Vector2Int> spawnPosList = new();//スポーン可能場所のリスト
        List<Vector2Int> emptyPosList = new();//空マスの座標リスト

        //Vector2Intに変換
        foreach(var pos in playerPos)
        {
            var newPos = new Vector2Int((int)pos.x, (int)pos.z);
            playerPosList.Add(newPos);
        }

        //空マスを登録
        foreach(var cell in cellSC)
        {
            if(cell.CheckState() == CellScript.CellState.empty)
            {
                emptyPosList.Add(cell.GetPosition());
            }
        }

        //空マスリストから条件に合うものをスポーン座標に追加
        foreach(var pos in emptyPosList)
        {
            var ok = true;

            //プレイヤー座標から1マス以上離れていたらスポーン可能
            foreach(var checkPos in playerPosList)
            {
                var distance = new Vector2Int(Mathf.Abs(checkPos.x - pos.y), Mathf.Abs(checkPos.y - pos.x));
                if(distance.x <= minDistance && distance.y <= minDistance)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                spawnPosList.Add(pos);
            }
        }

        //スポーン不可の場合
        if (spawnPosList.Count == 0)
        {
            Debug.LogWarning("敵をスポーンできる位置がありません。");
            return new Vector2Int(-1, -1);
        }

        //スポーン可能座標リストからランダムで返す
        var posNum = Random.Range(0, spawnPosList.Count);
        return spawnPosList[posNum];
    }
}
