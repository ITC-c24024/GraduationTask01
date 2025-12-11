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

    [SerializeField] Canvas canvas;

    List<Image> coinList = new List<Image>();

    //所持金
    public int money = 0;

    void Start()
    {
        
    }

    /// <summary>
    /// お金を得る
    /// </summary>
    /// <param name="amount">取得量</param>
    /// <param name="startPos">お金ドロップ位置</param>
    public void GetMoney(int amount, Vector2 startPos)
    {
        money += amount;

        StartCoroutine(MoveMoney(startPos));
    }
    /// <summary>
    /// お金を消費
    /// </summary>
    /// <param name="amount">消費量</param>
    public void UseMoney(int amount)
    {
        money -= amount;
    }

    /// <summary>
    /// お金ゲット演出
    /// </summary>
    /// <param name="startPos">開始位置</param>
    /// <returns></returns>
    IEnumerator MoveMoney(Vector2 startPos)
    {
        for(int i = 0; i < 10; i++)
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
