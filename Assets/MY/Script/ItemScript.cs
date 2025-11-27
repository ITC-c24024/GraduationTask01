using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemScript : MonoBehaviour
{
    PlayerController playerCon;

    public enum ItemType
    {
        hpUp,//体力アップ
        damageUp,//ダメージアップ
        criticalRateUp//クリティカル率アップ
    }

    public ItemType itemType;

    //このアイテムがすでに選ばれたかどうかの判定
    public bool isSellect = false;

    //アイテムごとの実行関数
    Dictionary<ItemType, Action> itemEffectDict;
    
    void Start()
    {
        itemEffectDict = new Dictionary<ItemType, Action>()
        {
            {ItemType.hpUp,HpUp},
            {ItemType.damageUp,DamageUp },
            {ItemType.criticalRateUp,CriticalRateUp }
        };
    }

    /// <summary>
    /// アイテムを取得し、そのアイテムタイプに応じて関数を実行
    /// </summary>
    /// <param name="player">取得したプレイヤー</param>
    public void GetItem(PlayerController player)
    {
        playerCon = player;
        isSellect = true;
        itemEffectDict[itemType]();
    }

    void HpUp()
    {
        int amount = playerCon.hp / 10;
        playerCon.SetHP(amount);
    }
    void DamageUp()
    {
        int amount = playerCon.damage / 10;
        playerCon.SetDamage(amount);
    }
    void CriticalRateUp()
    {
        float amount = playerCon.criticalRate / 10;
        playerCon.SetCriticalRate(amount);
    }
}
