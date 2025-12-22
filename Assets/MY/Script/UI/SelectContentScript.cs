using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectContentScript : MonoBehaviour
{
    WaveManager waveManager;
    [SerializeField] PlayerController[] playerCons;
    ItemScript[] itemScripts=new ItemScript[3];
    MoneyScript moneyScript;

    [SerializeField, Header("強化内容選択画面")]
    Image selectImage;
    [SerializeField, Header("選択中Image")]
    GameObject[] selects;
    [SerializeField, Header("強化内容Image")]
    GameObject[] items;
    [SerializeField, Header("スキップImage")]
    Image skipImage;
    [SerializeField, Header("アイテム情報背景")]
    Image itemInfoBG;
    [SerializeField, Header("アイテム情報")]
    GameObject[] itemInfo;

    [SerializeField, Header("メインカメラ")]
    Camera mainCamera;
    [SerializeField, Header("ショップカメラ")]
    Camera shopCamera;

    int playerNum = 0;

    //選択中判定
    bool isRun = false;

    void Start()
    {
        waveManager = GetComponent<WaveManager>();
        for(int i = 0; i < items.Length; i++)
        {
            itemScripts[i] = items[i].gameObject.GetComponent<ItemScript>();
        }
        moneyScript = GetComponent<MoneyScript>();
    }

    public IEnumerator SelectContent()
    {
        shopCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        skipImage.gameObject.SetActive(true);
        
        //選択アイテムをセット
        SetItem();    

        //プレイヤーが交互に選択する
        for (int i = 0; i < playerCons.Length; i++)
        {
            int originNum = 1;
            if (itemScripts[1].isSellect)
            {
                selects[playerNum].transform.position = new Vector3(
                    items[0].transform.localPosition.x,
                    selects[playerNum].transform.localPosition.y, 
                    items[0].transform.localPosition.z
                    );

                originNum = 0;
            }
            itemInfoBG.gameObject.SetActive(true);
            itemInfo[originNum].SetActive(true);

            selects[playerNum].gameObject.SetActive(true);
            yield return StartCoroutine(playerCons[i].SelectContent(originNum));

            selects[playerNum-1].gameObject.SetActive(false);
        }
        playerNum = 0;

        selectImage.gameObject.SetActive(false);
        itemInfoBG.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        skipImage.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        shopCamera.gameObject.SetActive(false);

        //次のウェーブへ
        StartCoroutine(waveManager.StartWave());
    }

    void SetItem()
    {
        //すでに選ばれた番号
        List<int> selectList = new List<int>();

        for(int i = 0; i < itemScripts.Length; i++)
        {
            //アイテム番号が重複しないようにする
            int itemNum;
            while (true)
            {
                itemNum = SelectItemNum();
                if (!selectList.Contains(itemNum)) break;
            }
            selectList.Add(itemNum);

            itemScripts[i].SetItemType(itemNum);   
        }
    }

    /// <summary>
    /// セットするアイテムをランダムで選ぶ
    /// </summary>
    /// <returns>アイテム番号</returns>
    int SelectItemNum()
    {
        int i= Random.Range(0, 100);
        if (i < 50) i = 0;
        else if (i < 60) i = 1;
        else if (i < 70) i = 2;
        else if (i < 80) i = 3;
        else if (i < 90) i = 4;
        else i = 5;

        return i;
    }

    /// <summary>
    /// アイテムを選択できるか判定
    /// </summary>
    /// <param name="selectNum">選択したい番号</param>
    /// /// <param name="dir">選択する方向</param>
    /// <returns></returns>
    public bool CanSelect(int selectNum, int dir)
    {
        //無限再帰しないように
        if (dir == 0) return false;
        //範囲外なら終了
        if (selectNum < 0 || selectNum >= items.Length) return false;
        //未獲得なら選択可能
        if (!itemScripts[selectNum].isSellect) return true;
        //獲得済みなら次を調べる
        return CanSelect(selectNum + dir, dir);
    }

    /// <summary>
    /// 選択中Imageを動かす
    /// </summary>
    /// <param name="selectNum">選択する内容のImage要素数</param>
    public void SelectItem(int selectNum)
    {
        itemInfoBG.gameObject.SetActive(true);
        
        Vector3 selectPos = new Vector3(
            items[selectNum].transform.localPosition.x,
            selects[playerNum].transform.localPosition.y,
            items[selectNum].transform.localPosition.z
            );
        selects[playerNum].transform.localPosition = selectPos;
        selects[playerNum].gameObject.SetActive(true);

        ItemScript.ItemType itemType = itemScripts[selectNum].GetNowItem();
        itemInfo[(int)itemType].SetActive(true);
    }

    /// <summary>
    /// 選択を決定
    /// </summary>
    /// <param name="selectNum">決定する内容のImage要素数</param>
    /// <returns>アイテムを変えたかの判定</returns>
    public bool DecisionItem(int selectNum)
    {
        if (CanDecision(selectNum))
        {
            selects[playerNum].gameObject.SetActive(false);

            moneyScript.UseMoney(itemScripts[selectNum].GetItemPrice());
            //アイテムを取得
            var item = items[selectNum].GetComponent<ItemScript>();
            item.GetItem(playerCons[playerNum]);

            ItemScript.ItemType itemType = itemScripts[selectNum].GetNowItem();
            itemInfo[(int)itemType].SetActive(true);

            //次のプレイヤーへ
            playerNum++;
            isRun = false;

            return true;
        }
        else
        {
            Debug.Log("※買えません");

            //演出案
            //金額テキスト揺らす

            return false;
        }
        
    }
    /// <summary>
    /// 決定できるかを判定
    /// </summary>
    /// <param name="selectNum">決定する内容のImage要素数</param>
    /// <returns>判定結果</returns>
    bool CanDecision(int selectNum)
    {
        int price = itemScripts[selectNum].GetItemPrice();
        int money = moneyScript.money;

        if (money >= price)
        {
            return true;
        }
        else return false;
    }

    /// <summary>
    /// 購入をスキップ
    /// </summary>
    public void Skip()
    {
        //次のプレイヤーへ
        playerNum++;
        isRun = false;
    }
}
