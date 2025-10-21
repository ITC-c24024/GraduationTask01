using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaScript : MonoBehaviour
{
    public TurnManager turnManager;

    //接触優先度
    public int rank;

    //HP
    public int hp = 100;

    //移動可能回数
    public int moveLimit = 1;
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
    /// プレイヤーの位置を調べ、進む方向を決める
    /// </summary>
    public Vector2 GetDirection(Vector3 playerPos, Vector3 startPos, int i)
    {
        float diffX = playerPos.x - startPos.x;
        float diffZ = playerPos.z - startPos.z;

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
}

[Serializable]
public class Direction
{
    public int x;
    public int z;
}
