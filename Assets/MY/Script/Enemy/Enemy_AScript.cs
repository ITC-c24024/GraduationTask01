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
            if (!CanMove(targetPos)) break;

            nextPos = targetPos;

            //�G�������点���~�߂���
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.enemy) break;
            //�\��
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);

            float time = 0;
            float required = 0.1f / moveRule.Length;
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

            gridManager.ChangeCellState(
                (int)curPos.z, 
                (int)curPos.x, 
                CellScript.CellState.enemy, 
                this, 
                new Vector2Int((int)direction.x, (int)direction.y)
                );
            //�ЂƂO�̃}�X����ɂ���
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x,this);
        }
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
