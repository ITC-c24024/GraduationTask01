using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    /// <summary>
    /// お金ゲット演出
    /// </summary>
    /// <param name="startPos">開始位置</param>
    /// <param name="targetPos">目標位置</param>
    /// <returns></returns>
    public IEnumerator MoveMoney(Vector2 startPos, Vector2 targetPos)
    {
        float time = 0;
        float reqierd = 1.0f;
        while (time < reqierd)
        {
            time += Time.deltaTime;

            gameObject.SetActive(true);

            float rate = Mathf.Sin(Mathf.Lerp(0, 3.14f / 2, time / reqierd));
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, rate);
            transform.localPosition = currentPos;

            yield return null;
        }

        Destroy(gameObject);
    }
}
