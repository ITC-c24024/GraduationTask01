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

            //�i�߂Ȃ��Ƃ��ړ��I��
            if (!CanMove(targetPos)) yield break;

            nextPos = targetPos;

            //�G�������点���~�߂���
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.enemy) yield break;

            //�v���C���[������ꍇ
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.player)
            {
                //�m�b�N�o�b�N�ł��Ȃ���ԂȂ�U�������\�肷��
                var result = gridManager.ChangeCellState(
                      (int)nextPos.z,
                      (int)nextPos.x,
                      CellScript.CellState.enemy,
                      this,
                      new Vector2Int((int)direction.x, (int)direction.y)
                      );
                if (!result.canMove) attackOnly = true;
            }

            //�\��
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);
            
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
                gridManager.SendDamage((int)curPos.z, (int)curPos.x, damage);

                StartCoroutine(Back(originPos));
                yield break;
            }
            //�v���C���[��������U������
            if(gridManager.CheckCellState((int)curPos.z, (int)curPos.x) == CellScript.CellState.player)
            {
                gridManager.SendDamage(
                    (int)curPos.z,
                    (int)curPos.x, damage,
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
        turnManager.FinCoroutine();
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
