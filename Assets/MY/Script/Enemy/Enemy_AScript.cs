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
        //行動回数分
        for(int n = 0; n < actionLimit; n++)
        {
            //現在位置を移動開始位置とする
            Vector3 startPos = curPos;

            //プレイヤーの位置を見て、どのプレイヤーについていくか決める
            Vector3 playerPos = SelectPlayer();
            
            //1行動
            for (int i = 0; i < moveRule.Length; i++)
            {
                //移動開始位置からプレイヤーの位置に行くためにどの方向に行けばいいか決める
                Vector2 direction = GetDirection(playerPos, startPos, i);

                Vector3 originPos = curPos;

                //進む位置
                Vector3 targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );

                //範囲外の時
                if (!CanMove(targetPos))
                {
                    Debug.Log("※範囲外です");
                    goBack = true;
                }

                //敵がいたらせき止め
                if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.enemy)
                {
                    Debug.Log("進めない");
                    goBack = true;
                }

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

                //通常移動
                animator.SetTrigger("IsAttack");
                float time = 0;
                float required = 0.5f / moveRule.Length;
                while (time < required)
                {
                    time += Time.deltaTime;

                    //if (time >= (required / 2) && goBack) break;

                    //現在地を計算
                    Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

                    //キャラを移動
                    transform.position = currentPos;

                    yield return null;
                }
                transform.position = targetPos;
                curPos = targetPos;
                if (goBack)
                {
                    StartCoroutine(Back(originPos));
                    goBack = false;

                    continue;
                }

                

                if (attackOnly)
                {
                    attackOnly = false;

                    //プレイヤーに攻撃
                    gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage, true);

                    StartCoroutine(Back(originPos));
                    
                    continue;
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
            yield return new WaitForSeconds(0.2f);
        }

        
        if (!attackOnly) turnManager.FinCoroutine();
    }
    
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;

        //HPが0になったら死亡
        if (hp <= 0)
        {
            StartCoroutine(Dead());
            turnManager.enemyList.Remove(this);
        }
    }
}
