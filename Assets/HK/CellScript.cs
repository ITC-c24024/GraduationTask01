using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    /// <summary>
    /// マスの状態
    /// </summary>
    public enum CellState
    {
        empty,//空
        player,//プレイヤー
        enemy,//敵
    }

    [SerializeField, Header("マスの状態")]
    CellState state = CellState.empty;

    [SerializeField, Header("予約数")]
    List<CharaScript> reserveList = new();

    [SerializeField, Header("現在のマス上にいるユニットのスクリプト")]
    CharaScript nowUnitSC;

    /// <summary>
    /// 予約用関数(ステート切り替えの前に必ず呼ぶ)
    /// </summary>
    /// <param name="unitSC">呼びだす側のユニットスクリプト</param>
    public void ReserveState(CharaScript unitSC)
    {
        if (!reserveList.Contains(unitSC))
        {
            reserveList.Add(unitSC);
        }
    }

    /// <summary>
    /// 可能であればマスの状態を変える
    /// </summary>
    /// <param name="newState">状態(empty=空, player=プレイヤー, enemy=敵)</param>
    /// <param name="unitSC">呼びだす側のユニットスクリプト</param>
    public IEnumerator MoveCheck(CellState newState, CharaScript unitSC)
    {
        yield return new WaitForSeconds(0.2f);

        //複数予約がある場合
        if (reserveList.Count > 1)
        {
            unitSC.ReciveDamage(0);
            yield break;
        }
        //予約がない(自身のみ)場合
        else if (reserveList.Count == 1)
        {
            //マスに他ユニットがいる場合
            if (nowUnitSC != null)
            {
                // 相手がその場に留まる場合（＝同じマスを狙う）
                if (nowUnitSC.nextPos == nowUnitSC.curPos)
                {
                    if (unitSC.rank < nowUnitSC.rank)
                    {
                        unitSC.ReciveDamage(nowUnitSC.damage);
                        yield break;
                    }
                }
                // 入れ違い判定
                //同時衝突
                else if (nowUnitSC.nextPos == unitSC.curPos)
                {
                    unitSC.ReciveDamage(0);
                    nowUnitSC.ReciveDamage(0);
                    yield break;
                }
                //衝突無し
                else if(nowUnitSC.nextPos != unitSC.curPos)
                {
                    ChangeState(newState, unitSC);
                    yield break;
                }
            }
            //マスに他ユニットがいない場合
            else if (nowUnitSC == null)
            {
                ChangeState(newState, unitSC);
                yield break;
            }
        }
    }


    /// <summary>
    /// ステート切り替え処理
    /// </summary>
    /// <param name="newState">切り替えるステート指定</param>
    /// <param name="unitSC">ユニットスクリプト</param>
    void ChangeState(CellState newState, CharaScript unitSC)
    {
        if (nowUnitSC != null && nowUnitSC != unitSC)
        {
            nowUnitSC = null;
        }

        state = newState;
        nowUnitSC = unitSC;
    }


    /// <summary>
    /// 各ユニットがマスを離れる際に呼ぶ関数
    /// マスの状態を空にする
    /// </summary>
    public void Leave()
    {
        state = CellState.empty;
        nowUnitSC = null;
    }

    /// <summary>
    /// マスの状態を確認する
    /// </summary>
    /// <returns>現在のマスの状態</returns>
    public CellState CheckState()
    {
        return state;
    }

    /// <summary>
    /// 予約確認用(主に敵の行動判定で使う用)
    /// </summary>
    /// <returns>予約あり=true, 予約なし=false</returns>
    public bool CheckReserve() => reserveList.Count > 0;

    /// <summary>
    /// 予約リストを空にする(行動1手ごとの終了時に呼ぶ)
    /// </summary>
    public void ResetList()
    {
        reserveList.Clear();
    }
}
