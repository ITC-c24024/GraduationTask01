using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectContentScript : MonoBehaviour
{
    WaveManager waveManager;
    [SerializeField] PlayerController[] playerCons;

    [SerializeField, Header("強化内容選択画面")]
    Image selectImage;
    [SerializeField, Header("選択中Image")]
    Image[] selects;
    [SerializeField, Header("強化内容Image")]
    Image[] items; 

    int playerNum = 0;

    //選択中判定
    bool isRun = false;

    void Start()
    {
        waveManager = GetComponent<WaveManager>();
    }

    void Update()
    {
        
    }

    public IEnumerator SelectContent()
    {
        //選択アイテムをセット
        SetItem();
        
        //選択画面表示
        selectImage.gameObject.SetActive(true);     

        //プレイヤーが交互に選択する
        for (int i = 0; i < playerCons.Length; i++)
        {
            selects[playerNum].gameObject.SetActive(true);
            StartCoroutine(playerCons[i].SelectContent());
            isRun = true;

            while (isRun) yield return null;
        }
        playerNum = 0;

        selectImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        //次のウェーブへ
        waveManager.StartWave();
    }

    void SetItem()
    {

    }

    public void SelectItem(int selectNum)
    {
        //選べるかどうかのboolを返したい(範囲外か、すでに選択されてるかを判定)


        Vector2 selectPos = new Vector2(items[selectNum].transform.position.x, items[selectNum].transform.position.y + 165);
        selects[playerNum].transform.position = selectPos;
        selects[playerNum].gameObject.SetActive(true);
    }

    public void DecisionItem(int selectNum)
    {
        selects[playerNum].gameObject.SetActive(false);

        //アイテムを取得
        var item = items[selectNum].GetComponent<ItemScript>();
        item.GetItem(playerCons[playerNum]);
        items[selectNum].color = new Color(120, 120, 120, 255);

        //次のプレイヤーへ
        playerNum++;
        isRun = false;
    }
}
