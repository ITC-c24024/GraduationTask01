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

    void Start()
    {
        cellSC = new CellScript[width, height];//配列初期化

        //マップ生成＆各マススクリプト取得
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cellObj = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cellSC[y, x] = cellObj.GetComponent<CellScript>();
                cellSC[y, x].SetPosition(new Vector2Int(y, x));
                cellSC[y, x].gridManagerSC = this;
            }
        }
    }

    /// <summary>
    /// 指定したマスに移動予約をする
    /// </summary>
    /// <param name="y">y座標</param>
    /// <param name="x">x座標</param>
    /// <param name="unitSC">呼びだす側のユニットスクリプト</param>
    public void ReserveCell(int y, int x, CharaScript unitSC)
    {
        cellSC[y, x].ReserveState(unitSC);
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
        return cellSC[y, x].TryEnter(state, unitSC,direction);
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
    /// 指定したマスの予約の有無を確認する
    /// </summary>
    /// <param name="y">y座標</param>
    /// <param name="x">x座標</param>
    /// <returns>予約あり=true, 予約なし=false</returns>
    public bool CheckCellReserve(int y, int x) => cellSC[y, x].CheckReserve();

    /// <summary>
    /// すべてのマスの予約をリセット(1行動ごとの終了時に呼ぶ)
    /// </summary>
    public void ResetReserveListAll()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cellSC[y, x].ResetList();
            }
        }
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
    public void SendDamage(int y,int x, int damage)
    {
        cellSC[y, x].ReciveAttack(damage);
    }
}
