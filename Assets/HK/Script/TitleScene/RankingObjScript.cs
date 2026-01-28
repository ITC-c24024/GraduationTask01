using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingObjScript : MonoBehaviour
{
    [SerializeField, Header("順位UI")]
    Text rankText;

    [SerializeField, Header("ウェーブ数UI")]
    Text waveValueText;

    [SerializeField, Header("順位")]
    int rank = 0;
    void Start()
    {
        rankText.text = $"#{rank}";
        waveValueText.text = $"{PlayerPrefs.GetInt($"#{rank}", Random.Range(0, 100))}";
    }

    public IEnumerator MovePos(float speed, bool isUp)
    {
        var startPos = transform.localPosition;
        var targetPos = Vector3.zero;

        if (isUp)
        {
            if (startPos.y <= -4.18f)
            {
                transform.localPosition = new Vector3(startPos.x, 4.1f, startPos.z);
                startPos = transform.localPosition;
            }
            targetPos = new Vector3(startPos.x, startPos.y - 1.38f, startPos.z);
        }
        else
        {
            if (startPos.y >= 4.1f)
            {
                transform.localPosition = new Vector3(startPos.x, -4.18f, startPos.z);
                startPos = transform.localPosition;
            }
            targetPos = new Vector3(startPos.x, startPos.y + 1.38f, startPos.z);
        }

        while (Vector3.Distance(transform.localPosition, targetPos) > 0.001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // 誤差吸収
        transform.localPosition = targetPos;


        if (transform.localPosition.y >= 4.1f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, -4.18f, transform.localPosition.z);
            rank += 5;
        }
        else if (transform.localPosition.y <= -4.18f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 4.1f, transform.localPosition.z);
            rank -= 5;
        }

    }
}
