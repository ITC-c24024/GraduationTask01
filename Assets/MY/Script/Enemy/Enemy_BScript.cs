using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_BScript : CharaScript
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
    /// 行動ルール:敵、移動不可プレイヤー、ステージ外に行くまで１マスずつ移動
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        //行動回数分移動する
        for (int n = 0; n < actionLimit; n++)
        {
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
                    Debug.Log("確認");
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
                        Debug.Log("attackOnly");
                    }
                }

                float time = 0;
                float required = 0.5f;
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

                    break;
                }

                if (attackOnly)
                {   
                    attackOnly = false;

                    //プレイヤーに攻撃
                    gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage, true);

                    StartCoroutine(Back(originPos));

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
        
        turnManager.FinCoroutine();
    }
}
