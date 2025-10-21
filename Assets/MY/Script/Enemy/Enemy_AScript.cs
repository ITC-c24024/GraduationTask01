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
        Vector3 playerPos = turnManager.GetPlayerPos();
        Vector3 startPos = transform.position;

        for (int i = 0; i < moveRule.Length; i++)
        {
            Debug.Log("�G�ړ�");
            Vector2 direction = GetDirection(playerPos, startPos, i);

            Vector3 originPos = transform.position;

            Vector3 targetPos = new Vector3(
                originPos.x + direction.x,
                originPos.y,
                originPos.z + direction.y
                );

            nextPos = targetPos;

            //�i�߂Ȃ��Ƃ��ړ��I��
            if (!CanMove(targetPos)) break;

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
        }
    }
}
