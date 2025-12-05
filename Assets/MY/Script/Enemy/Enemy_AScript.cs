using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AScript : CharaScript
{
    void Start()
    {
        charaState = CharaState.enemy;
        hpSlider.transform.SetAsFirstSibling();
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        curPos = transform.position;
    }
    /// <summary>
    /// 敵キャラを移動
    /// 行動ルール:前方1マス移動
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        //行動回数分
        for(int n = 0; n < actionLimit; n++)
        {
            /*
            //現在位置を移動開始位置とする
            Vector3 startPos = curPos;

            //プレイヤーの位置を見て、どのプレイヤーについていくか決める
            Vector3 playerPos = SelectPlayer();
            */
            //1行動
            for (int i = 0; i < moveRule.Length; i++)
            {
                /*
                //移動開始位置からプレイヤーの位置に行くためにどの方向に行けばいいか決める
                Vector2 direction = GetDirection(playerPos, startPos, i);
                */
                Vector3 originPos = curPos;
                /*
                //進む位置
                Vector3 targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
                */

                Vector3 targetPos = movePos;

                //範囲外の時、敵がいるとき
                if (!CanMove(targetPos))
                {
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
                          new Vector2Int((int)targetDir.x, (int)targetDir.y)
                          );
                    if (!result.canMove) attackOnly = true;
                }

                //通常移動
                animator.SetTrigger("IsAttack");
                shadowAnim.SetTrigger("IsAttack");
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
                        new Vector2Int((int)targetDir.x, (int)targetDir.y)
                        );
                }

                //マスの状態を敵自身にする
                gridManager.ChangeCellState(
                    (int)curPos.z,
                    (int)curPos.x,
                    CellScript.CellState.enemy,
                    this,
                    new Vector2Int((int)targetDir.x, (int)targetDir.y)
                    );
                //ひとつ前のマスを空にする
                gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
            }
            DeleteImage();
            yield return new WaitForSeconds(0.2f);
        }
      　
        turnManager.FinCoroutine();
    }

    public override void AttackState()
    {
        var arrow = Instantiate(arrowPrefab);
        arrowList.Add(arrow);

        //攻撃予定位置を表示
        attackImage.transform.localPosition = new Vector3(movePos.x, 0.101f, movePos.z);
        attackImage.SetActive(true);

        //矢印イメージを表示
        arrow.transform.localPosition = new Vector3(movePos.x - (targetDir.x / 2), 0.101f, movePos.z - (targetDir.y / 2));

        int angle = 0;
        if (targetDir.x > 0) angle = 90;
        else if (targetDir.x < 0) angle = -90;
        else if (targetDir.y > 0) angle = 0;
        else if (targetDir.y < 0) angle = 180;
        arrow.transform.localEulerAngles = new Vector3(arrow.transform.localEulerAngles.x, angle, arrow.transform.localEulerAngles.z);
        arrow.SetActive(true);
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
