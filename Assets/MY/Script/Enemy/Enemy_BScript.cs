using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_BScript : CharaScript
{
    void Start()
    {
        charaState = CharaState.enemy;
        curPos = transform.position;
    }

    /// <summary>
    /// 追うプレイヤー、追う方向を決めておく
    /// </summary>
    public override void SetAction()
    {
        SetTargetPos();
        AttackState();
    }

    /// <summary>
    /// 敵キャラを移動
    /// 行動ルール:敵、移動不可プレイヤー、ステージ外に行くまで１マスずつ移動
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        //行動回数分移動する
        for (int n = 0; n < actionLimit; n++)
        {
            DeleteImage();

            //行動ルールに沿って移動
            while (true)
            {
                //移動開始位置を設定
                Vector3 originPos = curPos;

                //目標位置
                Vector3 targetPos = new Vector3(
                    originPos.x + targetDir.x,
                    originPos.y,
                    originPos.z + targetDir.y
                    );

                if (!InStage(targetPos)) break;
               
                if (!CanMove(targetPos)) goBack = true;

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
                    if (!result.canMove)
                    {
                        attackOnly = true;
                    }
                }

                soundManager.Move();

                animator.SetBool("IsAttack", true);
                shadowAnim.SetBool("IsAttack", true);
                float time = 0;
                float required = 0.3f;
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
                    yield return StartCoroutine(Back(originPos));
                    goBack = false;

                    break;
                }

                if (attackOnly)
                {   
                    attackOnly = false;

                    //プレイヤーに攻撃
                    gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage, true);

                    yield return StartCoroutine(Back(originPos));

                    break;
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

                //敵がいないマスに進んだ場合
                if(gridManager.CheckCellState((int)curPos.z, (int)curPos.x) != CellScript.CellState.player)
                {
                    //進行軸じゃない軸がプレイヤーと重なったら移動終了
                    if (targetDir.x != 0)
                    {
                        if(curPos.x == targetPlayer.transform.position.x)
                        {
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

                            break;
                        }
                    }
                    else if (targetDir.y != 0)
                    {
                        if (curPos.z == targetPlayer.transform.position.z)
                        {
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

                            break;
                        }
                    }
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
        }

        animator.SetBool("IsAttack", false);
        shadowAnim.SetBool("IsAttack", false);

        soundManager.StopMove();
    }

    public override void AttackState()
    {
        //矢印UIの方向を決める
        int angle = 0;
        if (targetDir.x > 0) angle = 90;
        else if (targetDir.x < 0) angle = -90;
        else if (targetDir.y > 0) angle = 0;
        else if (targetDir.y < 0) angle = 180;

        for (int i = 0; i < 3; i++)
        {
            var arrow = Instantiate(arrowPrefab);
            arrowList.Add(arrow);

            arrow.transform.localPosition = new Vector3(
                movePos.x - targetDir.x / 2 - targetDir.x / 3 * -i,
                0.101f,
                movePos.z - targetDir.y / 2 - targetDir.y / 3 * -i
                );

            arrow.transform.localEulerAngles = new Vector3(arrow.transform.localEulerAngles.x, angle, arrow.transform.localEulerAngles.z);
            arrow.SetActive(true);
        }
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        if (hp < 0) hp = 0;

        for (int i = hp; i < hpBar.Count; i++)
        {
            hpBar[i].gameObject.SetActive(false);
        }

        //HPが0になったら死亡
        if (hp <= 0)
        {
            StartCoroutine(Dead());
            turnManager.enemyList.Remove(this);
            spownEnemySC.DeadZombi();
        }
    }
}
