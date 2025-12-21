using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyScript : MonoBehaviour
{
    [SerializeField, Header("金袋Image")]
    Image moneyBag;
    [SerializeField, Header("お金イメージ")]
    Image moneyImage;

    [SerializeField, Header("所持金Text")]
    Text moneyText;

    [SerializeField] Canvas canvas;

    //所持金
    public int money = 0;
    [SerializeField, Header("お金追加獲得割合")]
    float moneyGetRate = 0;

    void Start()
    {
        
    }

    //アイテム効果
    /// <summary>
    /// お金追加獲得割合を設定
    /// </summary>
    /// <param name="amount">変化量</param>
    public void SetRate(float amount)
    {
        moneyGetRate += amount;
    }

    /// <summary>
    /// お金を得る
    /// </summary>
    /// <param name="amount">取得量</param>
    /// <param name="startPos">お金ドロップ位置</param>
    public void GetMoney(int amount, Vector2 startPos)
    {
        money += amount + (int)(amount * moneyGetRate);
        moneyText.text = "" + money;

        StartCoroutine(MoveMoney(startPos));
    }
    /// <summary>
    /// お金を消費
    /// </summary>
    /// <param name="amount">消費量</param>
    public void UseMoney(int amount)
    {
        money -= amount;
        moneyText.text = "" + money;
    }

    /// <summary>
    /// お金ゲット演出
    /// </summary>
    /// <param name="startPos">開始位置</param>
    /// <returns></returns>
    IEnumerator MoveMoney(Vector2 startPos)
    {
        for(int i = 0; i < 6; i++)
        {
            var coin = Instantiate(moneyImage);
            coin.transform.SetParent(canvas.transform);
            coin.transform.localPosition = startPos;

            var coinSC = coin.GetComponent<CoinScript>();

            StartCoroutine(coinSC.MoveMoney(startPos, moneyBag.transform.localPosition));

            yield return null;
        }     
    }
}
