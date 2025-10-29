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

        animator.SetTrigger("IsAttack");
        for (int i = 0; i < moveRule.Length; i++)
        {
            Vector2[] vector2s = GetDirection(playerPos, startPos, i);
            Vector2 selectPos = vector2s[0];//選んだプレイヤーの位置
            Vector2 direction = vector2s[1];//selectPosに進むための方向
            Vector3 originPos = transform.position;

            Vector3 targetPos = new Vector3(
                originPos.x + direction.x,
                originPos.y,
                originPos.z + direction.y
                );

            //範囲外の時
            if (!CanMove(targetPos))
            {
                Debug.Log("進めない");
                direction = SelectAgain(direction, selectPos);

                targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
            }
            /*
            //敵がいたら
            if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.enemy)
            {
                Debug.Log("進めない");
                direction = SelectAgain(direction, selectPos);

                targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
            }
            */
            //プレイヤーがいる場合
            if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.player)
            {
                //ノックバックできない状態なら攻撃だけ予定する
                var result = gridManager.ChangeCellState(
                      (int)targetPos.z,
                      (int)targetPos.x,
                      CellScript.CellState.enemy,
                      this,
                      new Vector2Int((int)direction.x, (int)direction.y)
                      );
                if (!result.canMove) attackOnly = true;
            }

            //先約がいなかったら予約
            if (!gridManager.CheckCellReserve((int)targetPos.z, (int)targetPos.x))
                gridManager.ReserveCell((int)targetPos.z, (int)targetPos.x, this);
            else
            {
                Debug.Log("進めない");
                direction = SelectAgain(direction, selectPos);

                targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
            }

            nextPos = targetPos;

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
                gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage, true);

                StartCoroutine(Back(originPos));
                yield break;
            }
            //プレイヤーがいたら攻撃する
            else
            {
                gridManager.SendDamage(
                    (int)curPos.z,
                    (int)curPos.x,
                    damage,
                    true,
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
        if (!attackOnly) turnManager.FinCoroutine();
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
