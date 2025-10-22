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

    /// <summary>
    /// TryEnter関数呼びだしの結果用
    /// </summary>
    public struct TryEnterResult
    {
        public bool canMove;
        public int damage;
        public Vector2Int knockbackDir;
    }

    Vector2Int cellPos = new();

    [SerializeField, Header("マスの状態")]
    CellState state = CellState.empty;

    [SerializeField, Header("予約数")]
    List<CharaScript> reserveList = new();

    [SerializeField, Header("プレイヤー数")]
    List<CharaScript> playerList = new();

    [SerializeField, Header("敵数")]
    List<CharaScript> enemyList = new();

    /// <summary>
    /// ユニット合計数
    /// </summary>
    int UnitCount => playerList.Count + enemyList.Count;

    public void SetPosition(Vector2Int pos)
    {
        cellPos = pos;
    }

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
    /// <param name="direction">ノックバック方向(敵の場合のみ使用)</param>
    public TryEnterResult TryEnter(CellState newState, CharaScript unitSC, Vector2Int direction = default)
    {
        TryEnterResult result = new()
        {
            canMove = false,
            damage = 0,
            knockbackDir = Vector2Int.zero
        };

        //マスに他ユニットがいる場合
        if (UnitCount > 0)
        {
            //プレイヤーの場合
            if (newState == CellState.player)
            {
                //敵ステートだったらノックバックさせる
                if (state == CellState.enemy)
                {
                    foreach (var enemy in enemyList)
                    {
                        result.damage += enemy.damage;
                    }
                }
                else
                {
                    result.canMove = true;
                }
            }
            //敵の場合(プレイヤーが居たら)
            else if (newState == CellState.enemy && state == CellState.player)
            {
                //ノックバック方向のマスがあるか、空かどうかチェックの処理を追記必要
                var ok = false;
                if (ok)
                {
                    result.canMove = true;
                    result.knockbackDir = direction;
                }

                result.damage += unitSC.damage;
            }
        }
        //マスに他ユニットがいない場合
        else if (UnitCount <= 0)
        {
            result.canMove = true;
        }

        if (result.canMove)
        {
            ChangeState(newState, unitSC);
        }

        return result;
    }



    /// <summary>
    /// ステート切り替え処理
    /// </summary>
    /// <param name="newState">切り替えるステート指定</param>
    /// <param name="unitSC">ユニットスクリプト</param>
    void ChangeState(CellState newState, CharaScript unitSC)
    {
        if (newState == CellState.player && !playerList.Contains(unitSC))
        {
            playerList.Add(unitSC);
        }
        else if (newState == CellState.enemy && !enemyList.Contains(unitSC))
        {
            enemyList.Add(unitSC);
        }

        state = newState;
    }


    /// <summary>
    /// 各ユニットがマスを離れる際に呼ぶ関数
    /// マスの状態を空にする
    /// </summary>
    /// <param name="unitSC">ユニットのスクリプト</param>
    public void Leave(CharaScript unitSC)
    {
        playerList.Remove(unitSC);
        enemyList.Remove(unitSC);
        reserveList.Remove(unitSC);

        if (UnitCount == 0)
        {
            state = CellState.empty;
        }
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
