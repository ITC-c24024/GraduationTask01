using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AScript : CharaScript
{
    /// <summary>
    /// 敵キャラを移動
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        Vector3[] playerPos = turnManager.GetPlayerPos();
        Vector3 startPos = transform.position;

        animator.SetTrigger("IsMove");
        for (int i = 0; i < moveRule.Length; i++)
        {
            Vector2 direction = GetDirection(playerPos, startPos, i);
            Vector3 originPos = transform.position;

            Vector3 targetPos = new Vector3(
                originPos.x + direction.x,
                originPos.y,
                originPos.z + direction.y
                );

            //進めないとき移動終了
            if (!CanMove(targetPos)) yield break;

            nextPos = targetPos;

            //敵がいたらせき止められる
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.enemy) yield break;

            //プレイヤーがいる場合
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.player)
            {
                //ノックバックできない状態なら攻撃だけ予定する
                var result = gridManager.ChangeCellState(
                      (int)nextPos.z,
                      (int)nextPos.x,
                      CellScript.CellState.enemy,
                      this,
                      new Vector2Int((int)direction.x, (int)direction.y)
                      );
                if (!result.canMove) attackOnly = true;
            }

            //予約
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);
            
            float time = 0;
            float required = 0.5f / moveRule.Length;
            while (time < required)
            {
                time += Time.deltaTime;

                //現在地を計算
                Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

                //キャラを移動
                transform.position = currentPos;

                yield return null;
            }
            transform.position = targetPos;
            curPos = targetPos;

            if (attackOnly)
            {
                //プレイヤーに攻撃
                gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage);

                StartCoroutine(Back(originPos));
                yield break;
            }
            //プレイヤーがいたら攻撃する
            if(gridManager.CheckCellState((int)curPos.z, (int)curPos.x) == CellScript.CellState.player)
            {
                gridManager.SendDamage(
                    (int)curPos.z,
                    (int)curPos.x, damage,
                    new Vector2Int((int)direction.x, (int)direction.y)
                    );
            }

            //マスの状態を敵自身にする
            gridManager.ChangeCellState(
                (int)curPos.z,
                (int)curPos.x,
                CellScript.CellState.enemy,
                this,
                new Vector2Int((int)direction.x, (int)direction.y)
                );
            //ひとつ前のマスを空にする
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
            
            
        }
        turnManager.FinCoroutine();
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
