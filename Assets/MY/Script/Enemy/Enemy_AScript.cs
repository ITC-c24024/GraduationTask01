using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AScript : CharaScript
{
    /// <summary>
    /// “GƒLƒƒƒ‰‚ðˆÚ“®
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

            //i‚ß‚È‚¢‚Æ‚«ˆÚ“®I—¹
            if (!CanMove(targetPos)) break;

            nextPos = targetPos;

            //“G‚ª‚¢‚½‚ç‚¹‚«Ž~‚ß‚ç‚ê‚é
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.enemy) break;
            //—\–ñ
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);

            float time = 0;
            float required = 0.1f / moveRule.Length;
            while (time < required)
            {
                time += Time.deltaTime;

                //Œ»Ý’n‚ðŒvŽZ
                Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

                //ƒLƒƒƒ‰‚ðˆÚ“®
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
            //‚Ð‚Æ‚Â‘O‚Ìƒ}ƒX‚ð‹ó‚É‚·‚é
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x,this);
        }
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;
    }
}
