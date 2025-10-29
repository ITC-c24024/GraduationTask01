using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AScript : CharaScript
{
    /// <summary>
    /// �G�L�������ړ�
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        //�s���񐔕�
        for(int n = 0; n < actionLimit; n++)
        {
            //���݈ʒu���ړ��J�n�ʒu�Ƃ���
            Vector3 startPos = curPos;

            //�v���C���[�̈ʒu�����āA�ǂ̃v���C���[�ɂ��Ă��������߂�
            Vector3 playerPos = SelectPlayer();
            
            //1�s��
            for (int i = 0; i < moveRule.Length; i++)
            {
                //�ړ��J�n�ʒu����v���C���[�̈ʒu�ɍs�����߂ɂǂ̕����ɍs���΂��������߂�
                Vector2 direction = GetDirection(playerPos, startPos, i);

                Vector3 originPos = curPos;

                //�i�ވʒu
                Vector3 targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );

                //�͈͊O�̎�
                if (!CanMove(targetPos))
                {
                    Debug.Log("���͈͊O�ł�");
                }

                //�G�������点���~��
                if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.enemy)
                {
                    Debug.Log("�i�߂Ȃ�");
                }

                //�v���C���[������ꍇ
                if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.player)
                {
                    //�m�b�N�o�b�N�ł��Ȃ���ԂȂ�U�������\�肷��
                    var result = gridManager.ChangeCellState(
                          (int)targetPos.z,
                          (int)targetPos.x,
                          CellScript.CellState.enemy,
                          this,
                          new Vector2Int((int)direction.x, (int)direction.y)
                          );
                    if (!result.canMove) attackOnly = true;
                }

                //�ʏ�ړ�
                animator.SetTrigger("IsAttack");
                float time = 0;
                float required = 0.5f / moveRule.Length;
                while (time < required)
                {
                    time += Time.deltaTime;

                    //���ݒn���v�Z
                    Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

                    //�L�������ړ�
                    transform.position = currentPos;

                    yield return null;
                }
                transform.position = targetPos;
                curPos = targetPos;

                if (attackOnly)
                {
                    //�v���C���[�ɍU��
                    gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage, true);

                    StartCoroutine(Back(originPos));
                    yield break;
                }
                //�v���C���[��������U������
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

                //�}�X�̏�Ԃ�G���g�ɂ���
                gridManager.ChangeCellState(
                    (int)curPos.z,
                    (int)curPos.x,
                    CellScript.CellState.enemy,
                    this,
                    new Vector2Int((int)direction.x, (int)direction.y)
                    );
                //�ЂƂO�̃}�X����ɂ���
                gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
            }
        }

        
        if (!attackOnly) turnManager.FinCoroutine();
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
