using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaScript : MonoBehaviour
{
    public TurnManager turnManager;
    public GridManager gridManager;
    public CellScript cellScript;

    //接触優先度
    public int rank;

    //HP
    public int hp = 100;
    //攻撃力
    public int damage = 1;

    //行動可能回数
    public int actionLimit = 1;
    //移動法則
    public Direction[] moveRule;

    //現在位置
    public Vector3 curPos;
    //次に進む予定位置
    public Vector3 nextPos;

    /// <summary>
    /// 進めるかの判定
    /// </summary>
    /// <param name="x">進みたいマスのx座標</param>
    /// <param name="z">進みたいマスのz座標</param>
    /// <returns></returns>
    public bool CanMove(Vector3 targetPos)
    {
        //ステージ範囲内か調べる
        float posX = targetPos.x;
        float posZ = targetPos.z;
        if (posX < 0 || 7 < posX) return false;
        else if (posZ < 0 || 7 < posZ) return false;
        else return true;
    }

    /// <summary>
    /// 距離を計算
    /// </summary>
    /// <param name="targetPos">目標位置</param>
    /// <param name="startPos">スタート位置</param>
    /// <returns>結果の距離を返す</returns>
    float GetDistance(Vector3 targetPos, Vector3 startPos)
    {
        float distance = Mathf.Sqrt(
            Mathf.Pow(targetPos.x - startPos.x, 2) + Mathf.Pow(targetPos.z - startPos.z, 2)
            );
        return distance;
    }

    /// <summary>
    /// プレイヤーの位置、距離を調べ、進む方向を決める
    /// </summary>
    public Vector2 GetDirection(Vector3[] playerPos, Vector3 startPos, int i)
    {
        //プレイヤーとの距離を計算
        float diff_A = GetDistance(playerPos[0], startPos);
        float diff_B = GetDistance(playerPos[1], startPos);

        //近い方を追う位置とする
        Vector3 targetPos;
        if (diff_A <= diff_B)
        {
            targetPos = playerPos[0];
        }
        else
        {
            targetPos = playerPos[1];
        }

        //x座標、z座標の差を計算
        float diffX = targetPos.x - startPos.x;
        float diffZ = targetPos.z - startPos.z;

        //より離れてる軸に進む
        if (Mathf.Abs(diffX) <= Mathf.Abs(diffZ))
        {
            return new Vector2(moveRule[i].x * Mathf.Sign(diffX), moveRule[i].z * Mathf.Sign(diffZ));
        }
        else
        {
            return new Vector2(moveRule[i].z * Mathf.Sign(diffX), moveRule[i].x * Mathf.Sign(diffZ));
        }
    }

    /// <summary>
    /// 敵キャラを移動
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Move()
    {
        yield return null;
    }

    /// <summary>
    /// 被弾処理
    /// </summary>
    /// <param name="amount">被ダメージ</param>
    public virtual void ReciveDamage(int amount, Vector2 kbDir = default)
    {
        
    }
}

[Serializable]
public class Direction
{
    public int x;
    public int z;
}
