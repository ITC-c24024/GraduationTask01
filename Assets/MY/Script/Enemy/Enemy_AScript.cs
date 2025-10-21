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
        Vector3 playerPos = turnManager.GetPlayerPos();
        Vector3 startPos = transform.position;

        for (int i = 0; i < moveRule.Length; i++)
        {
            Debug.Log("“GˆÚ“®");
            Vector2 direction = GetDirection(playerPos, startPos, i);

            Vector3 originPos = transform.position;

            Vector3 targetPos = new Vector3(
                originPos.x + direction.x,
                originPos.y,
                originPos.z + direction.y
                );

            nextPos = targetPos;

            //i‚ß‚È‚¢‚Æ‚«ˆÚ“®I—¹
            if (!CanMove(targetPos)) break;

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
        }
    }
}
