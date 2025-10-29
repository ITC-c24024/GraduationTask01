using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

    //グリッドマネージャースクリプト
    public GridManager gridManagerSC;

    [SerializeField, Header("マスの座標")]
    Vector2Int cellPos = new();

    [SerializeField, Header("マスの状態")]
    CellState state = CellState.empty;

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
    /// マスの座標を取得する
    /// </summary>
    /// <returns>座標</returns>
    public Vector2Int GetPosition()
    {
        return cellPos;
    }

    /// <summary>
    /// 可能であればマスの状態を変える
    /// </summary>
    /// <param name="newState">状態(empty=空, player=プレイヤー, enemy=敵)</param>
    /// <param name="unitSC">呼びだす側のユニットスクリプト</param>
    /// <param name="direction">エントリ方向</param>
    public TryEnterResult TryEnter(CellState newState, CharaScript unitSC, Vector2Int direction)
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
                //敵ステートだったらノックバック
                if (state == CellState.enemy)
                {
                    foreach (var enemy in enemyList)
                    {
                        result.damage += enemy.damage;
                        result.knockbackDir = new Vector2Int(-direction.x, -direction.y);
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
                int x = cellPos.x + direction.y;
                int y = cellPos.y + direction.x;
                int damage = unitSC.damage;

                // ノックバック方向のマスがマップ内かチェック
                bool inBounds = x >= 0 && x < gridManagerSC.width &&
                                y >= 0 && y < gridManagerSC.height;

                if (inBounds)
                {
                    var targetCell = gridManagerSC.CheckCellState(x, y);

                    // そのマスが敵でなければノックバック可能
                    if (targetCell != CellState.enemy)
                    {
                        result.canMove = true;
                    }
                }
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
    /// プレイヤー攻撃の受け付け用
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    public void ReciveAttack(int damage, bool isEnemy, Vector2Int direction)//演出上後々bool返すことになるかも
    {
        if (!isEnemy && enemyList.Count > 0)
        {
            foreach (var enemy in enemyList)
            {
                enemy.ReciveDamage(damage, new Vector2(0, 0));
            }
        }
        else if (isEnemy && playerList.Count > 0)
        {
            foreach (var player in playerList)
            {
                player.ReciveDamage(damage, direction);
            }
        }
    }
}
