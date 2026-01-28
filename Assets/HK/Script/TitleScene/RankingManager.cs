using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RankingManager : MonoBehaviour
{
    [SerializeField, Header("入力用")]
    InputActionAsset inputAsset;

    //選択アクション
    InputAction selectAct;

    [SerializeField, Header("記録表示オブジェクトのスクリプト")]
    RankingObjScript[] scoreObj = new RankingObjScript[7];

    //上から5番目の記録(デフォルトで1～5が見えてる状態)
    public int selectNum = 4;

    [SerializeField, Header("スクロール速度")]
    float scrollSpeed;

    //スクロールコルーチン重複防止用
    bool isScroll = false;

    //スクロール完了カウント
    int scrollFinishedCount = 0;
    void Start()
    {
        selectAct = inputAsset.FindAction("Move");
        selectAct.Enable();
    }


    void Update()
    {
        //スティック入力の値を取得
        var value = selectAct.ReadValue<Vector2>();

        //入力可能だったら
        if (!isScroll)
        {
            if (value.y >= 0.5f && selectNum > 4)
                StartScroll(true);

            else if (value.y <= -0.5f && selectNum < CheckRankingAmount())
                StartScroll(false);
        }

    }

    int CheckRankingAmount()
    {
        return PlayerPrefs.GetInt("RankCount", 100);
    }

    void StartScroll(bool isUp)
    {
        isScroll = true;

        foreach (var obj in scoreObj)
            StartCoroutine(ScrollRoutine(obj, isUp));
    }
    IEnumerator ScrollRoutine(RankingObjScript obj, bool isUp)
    {
        yield return StartCoroutine(obj.MovePos(scrollSpeed, isUp));

        // 全オブジェクト終了を待ったあと解除
        scrollFinishedCount++;
        if (scrollFinishedCount >= scoreObj.Length)
        {
            scrollFinishedCount = 0;
            isScroll = false;

            if (isUp) selectNum--;
            else selectNum++;
        }
    }

}
