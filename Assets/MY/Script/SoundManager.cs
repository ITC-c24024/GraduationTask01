using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField, Header("タイトルBGM")]
    AudioSource titleSource;
    [SerializeField, Header("メインBGM")]
    AudioSource mainSource;
    [SerializeField, Header("ショップBGM")]
    AudioSource shopSource;
    [SerializeField, Header("攻撃SE")]
    AudioSource attackSource;
    [SerializeField, Header("被弾SE")]
    AudioSource reciveSource;
    [SerializeField, Header("ツタが生える音")]
    AudioSource ivySource;
    [SerializeField, Header("UI表示音")]
    AudioSource uiSource;
    [SerializeField, Header("UI選択音")]
    AudioSource selectSource;
    [SerializeField, Header("UI決定音")]
    AudioSource decisionSource;
    [SerializeField, Header("アイテム購入音")]
    AudioSource itemSource;
    [SerializeField, Header("アイテム購入不可音")]
    AudioSource cantSource;

    [SerializeField, Header("タイトルBGM")]
    AudioClip titleClip;
    [SerializeField, Header("メインBGM")]
    AudioClip mainClip;
    [SerializeField, Header("ショップBGM")]
    AudioClip shopClip;
    [SerializeField, Header("攻撃SE")]
    AudioClip attackClip;
    [SerializeField, Header("被弾SE")]
    AudioClip reciveClip;
    [SerializeField, Header("ツタが生える音")]
    AudioClip ivyClip;
    [SerializeField, Header("UI表示音")]
    AudioClip uiClip;
    [SerializeField, Header("UI選択音")]
    AudioClip selectClip;
    [SerializeField, Header("UI決定音")]
    AudioClip decisionClip;
    [SerializeField, Header("アイテム購入音")]
    AudioClip itemClip;
    [SerializeField, Header("アイテム購入不可音")]
    AudioClip cantClip;

    void Start()
    {
        
    }

    public void Title()
    {
        titleSource.PlayOneShot(titleClip);
    }
    public void StopTitle()
    {
        titleSource.Stop();
    }
    public void Main()
    {
        mainSource.PlayOneShot(mainClip);
    }
    public void StopMain()
    {
        mainSource.Stop();
    }
    public void Shop()
    {
        shopSource.PlayOneShot(shopClip);
    }
    public void StopShop()
    {
        shopSource.Stop();
    }
    public void Attack()
    {
        attackSource.PlayOneShot(attackClip);
    }
    public void Recive()
    {
        reciveSource.PlayOneShot(reciveClip);
    }
    public void Ivy()
    {
        ivySource.PlayOneShot(ivyClip);
    }
    public void UI()
    {
        uiSource.PlayOneShot(uiClip);
    }
    public void Select()
    {
        selectSource.PlayOneShot(selectClip);
    }
    public void Decision()
    {
        decisionSource.PlayOneShot(decisionClip);
    }
    public void Buy()
    {
        itemSource.PlayOneShot(itemClip);
    }
    public void Cant()
    {
        cantSource.PlayOneShot(cantClip);
    }
}
