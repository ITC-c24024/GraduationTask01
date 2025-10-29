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
        Vector3[] playerPos = turnManager.GetPlayerPos();
        Vector3 startPos = transform.position;

        animator.SetTrigger("IsAttack");
        for (int i = 0; i < moveRule.Length; i++)
        {
            Vector2[] vector2s = GetDirection(playerPos, startPos, i);
            Vector2 selectPos = vector2s[0];//�I�񂾃v���C���[�̈ʒu
            Vector2 direction = vector2s[1];//selectPos�ɐi�ނ��߂̕���
            Vector3 originPos = transform.position;

            Vector3 targetPos = new Vector3(
                originPos.x + direction.x,
                originPos.y,
                originPos.z + direction.y
                );

            //�͈͊O�̎�
            if (!CanMove(targetPos))
            {
                Debug.Log("�i�߂Ȃ�");
                direction = SelectAgain(direction, selectPos);

                targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
            }
            /*
            //�G��������
            if (gridManager.CheckCellState((int)targetPos.z, (int)targetPos.x) == CellScript.CellState.enemy)
            {
                Debug.Log("�i�߂Ȃ�");
                direction = SelectAgain(direction, selectPos);

                targetPos = new Vector3(
                    originPos.x + direction.x,
                    originPos.y,
                    originPos.z + direction.y
                    );
            }
            */
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

            //��񂪂��Ȃ�������\��
            if (!gridManager.CheckCellReserve((int)targetPos.z, (int)targetPos.x))
                gridManager.ReserveCell((int)targetPos.z, (int)targetPos.x, this);
            else
            {
                Debug.Log("�i�߂Ȃ�");
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
        if (!attackOnly) turnManager.FinCoroutine();
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
