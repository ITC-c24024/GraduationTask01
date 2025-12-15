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
    
    enum ItemType
    {
        hpUp,//体力+1
        damageUp,//ダメージ+1
        actionLimitUp,//行動回数+1
        moneyUp//獲得金+n% 
    }

    ItemType[] itemType = new ItemType[]
    {
        ItemType.hpUp,
        ItemType.damageUp,
        ItemType.actionLimitUp,
        ItemType.moneyUp
    };

    [SerializeField,Header("現在のアイテム")] ItemType nowItem;

    //このアイテムがすでに選ばれたかどうかの判定
    public bool isSellect = false;

    //アイテムごとの実行関数
    Dictionary<ItemType, Action> itemEffect;
    //アイテムごとの値段
    Dictionary<ItemType, int> itemPrice;

    void Start()
    {
        itemEffect = new Dictionary<ItemType, Action>()
        {
            {ItemType.hpUp,HpUp},
            {ItemType.damageUp,DamageUp},
            {ItemType.actionLimitUp, ActionLimitUp},
            {ItemType.moneyUp,MoneyUp }
        };
        itemPrice = new Dictionary<ItemType, int>()
        {
            {ItemType.hpUp,500},
            {ItemType.damageUp,500},
            {ItemType.actionLimitUp, 500},
            {ItemType.moneyUp,500}
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
        nowItem = itemType[itemNum];
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
        isSellect = true;
        itemEffect[nowItem]();
    }

    void HpUp()
    {
        playerCon.SetHP(1);
    }
    void DamageUp()
    {
        playerCon.SetDamage(1);
    }
    void ActionLimitUp()
    {       
        playerCon.SetActionLimit(1);
    }
    void MoneyUp()
    {
        moneyScript.SetRate(0.5f);
    }
}
