using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemScript : MonoBehaviour
{
    PlayerController playerCon;
    [SerializeField] MoneyScript moneyScript;

    [SerializeField, Header("アイテムイメージ")]
    GameObject[] itemImage;

    public enum ItemType
    {
        /// <summary>
        /// 体力+1
        /// </summary>
        hpUp,
        /// <summary>
        /// ダメージ+1
        /// </summary>
        damageUp,
        /// <summary>
        /// 行動回数+1
        /// </summary>
        actionLimitUp,
        /// <summary>
        /// 獲得金+n% 
        /// </summary>
        moneyUp,
        /// <summary>
        /// 最大HP+1
        /// </summary>
        maxHpUp,
        /// <summary>
        /// ダメージマス無効
        /// </summary>
        invalidDmage
    }

    ItemType[] itemType = new ItemType[]
    {
        ItemType.hpUp,
        ItemType.damageUp,
        ItemType.actionLimitUp,
        ItemType.moneyUp,
        ItemType.maxHpUp,
        ItemType.invalidDmage
    };

    [SerializeField,Header("現在のアイテム")] ItemType nowItem;

    //このアイテムがすでに選ばれたかどうかの判定
    public bool isSellect = false;

    //アイテムごとの値段
    Dictionary<ItemType, int> itemPrice;

    void Start()
    {
        itemPrice = new Dictionary<ItemType, int>()
        {
            {ItemType.hpUp,100},
            {ItemType.damageUp,600},
            {ItemType.actionLimitUp, 800},
            {ItemType.moneyUp,400},
            {ItemType.maxHpUp,400 },
            {ItemType.invalidDmage ,600}
        };
    }

    /// <summary>
    /// アイテムの総数を取得
    /// </summary>
    /// <returns></returns>
    public int GetItemCount()
    {
        int count = itemType.Length;
        return count;
    }

    /// <summary>
    /// アイテムのタイプを決める
    /// </summary>
    /// <param name="itemNum">ランダムなアイテム番号</param>
    public void SetItemType(int itemNum)
    {
        isSellect = false;
        
        nowItem = itemType[itemNum];

        for (int i = 0; i < itemImage.Length; i++)
        {
            itemImage[i].SetActive(false);
        }       
        itemImage[itemNum].SetActive(true);
    }

    /// <summary>
    /// 現在のアイテムを取得
    /// </summary>
    /// <returns>現在のアイテム</returns>
    public ItemType GetNowItem()
    {
        return nowItem;
    }

    /// <summary>
    /// アイテムの値段を取得
    /// </summary>
    /// <returns></returns>
    public int GetItemPrice()
    {
        int price = itemPrice[nowItem];
        return price;
    }

    /// <summary>
    /// アイテムを取得し、そのアイテムタイプに応じて関数を実行
    /// </summary>
    /// <param name="player">取得したプレイヤー</param>
    public void GetItem(PlayerController player)
    {
        playerCon = player;
        player.GetItem(nowItem);

        itemImage[(int)nowItem].SetActive(false);
        isSellect = true;
    }
}
