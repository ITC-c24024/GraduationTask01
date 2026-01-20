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
    SoundManager soundManager;

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
    //ひとつ前の説明Image
    GameObject pastInfo;
    [SerializeField, Header("フェードオブジェクト")]
    GameObject fadeObj;

    [SerializeField, Header("メインカメラ")]
    Camera mainCamera;
    [SerializeField, Header("ショップカメラ")]
    Camera shopCamera;

    [SerializeField]
    Vector2[] infoPos;

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
        soundManager = GetComponent<SoundManager>();
    }

    public IEnumerator SelectContent()
    {
        StartCoroutine(FadeInOut());

        yield return new WaitForSeconds(1.0f);

        shopCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        skipImage.gameObject.SetActive(true);   

        soundManager.Shop();

        //選択アイテムをセット
        SetItem();    

        //プレイヤーが交互に選択する
        for (int i = 0; i < playerCons.Length; i++)
        {
            int originNum = 1;
            if (itemScripts[1].isSellect)
            {
                selects[playerNum].transform.localPosition = new Vector3(
                    items[0].transform.localPosition.x,
                    selects[playerNum].transform.localPosition.y, 
                    items[0].transform.localPosition.z
                    );

                originNum = 0;
            }
            if (pastInfo != null) pastInfo.SetActive(false);
            itemInfoBG.gameObject.transform.localPosition = infoPos[originNum];
            itemInfoBG.gameObject.SetActive(true);
            GameObject info = itemInfo[(int)itemScripts[originNum].GetNowItem()];
            info.SetActive(true);
            pastInfo = info;

            selects[playerNum].gameObject.SetActive(true);
            yield return StartCoroutine(playerCons[i].SelectContent(originNum));

            selects[playerNum-1].gameObject.SetActive(false);
            selects[playerNum-1].transform.localPosition= new Vector3(
            items[1].transform.localPosition.x,
            selects[1].transform.localPosition.y,
            items[1].transform.localPosition.z
            );
        }
        playerNum = 0;

        itemInfoBG.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(1.0f);

        skipImage.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        shopCamera.gameObject.SetActive(false);

        soundManager.StopShop();
        yield return new WaitForSeconds(2.0f);
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
        else if (i < 90) i = 3;
        else i = 4;

        return i;
    }

    IEnumerator FadeInOut()
    {
        Vector2 startPos = new Vector2(2560, fadeObj.transform.localPosition.y);
        Vector2 targetPos = new Vector2(0,fadeObj.transform.localPosition.y);

        float time = 0;
        float reqired = 1.0f;
        while (time < reqired)
        {
            time += Time.deltaTime;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, time / reqired);
            fadeObj.transform.localPosition = currentPos;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        startPos = targetPos;
        targetPos = new Vector2(-2560, fadeObj.transform.localPosition.y);

        time = 0;
        while (time < reqired)
        {
            time += Time.deltaTime;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos,time / reqired);
            fadeObj.transform.localPosition = currentPos;

            yield return null;
        }
    }

    /// <summary>
    /// アイテムを選択できるか判定
    /// </summary>
    /// <param name="selectNum">選択したい番号</param>
    /// /// <param name="dir">選択する方向</param>
    /// <returns>0:選択できる 1:選択できない 2:さらに次のアイテムを選択できる </returns>
    public int CanSelect(int selectNum, int dir)
    {
        //無限再帰しないように
        if (dir == 0) return 1;
        //範囲外なら終了
        if (selectNum < 0 || selectNum >= items.Length) return 1;
        //未獲得なら選択可能
        if (!itemScripts[selectNum].isSellect) return 0;
        //獲得済みなら次を調べる
        if (CanSelect(selectNum + dir, dir) == 1) return 1;
        else return 2;
    }

    /// <summary>
    /// 選択中Imageを動かす
    /// </summary>
    /// <param name="selectNum">選択する内容のImage要素数</param>
    public void SelectItem(int selectNum)
    {
        soundManager.Select();

        pastInfo.SetActive(false);
        ItemScript.ItemType itemType = itemScripts[selectNum].GetNowItem();
        itemInfo[(int)itemType].SetActive(true);
        pastInfo = itemInfo[(int)itemType];

        itemInfoBG.gameObject.transform.localPosition = infoPos[selectNum];
        itemInfoBG.gameObject.SetActive(true);
        
        Vector3 selectPos = new Vector3(
            items[selectNum].transform.localPosition.x,
            selects[playerNum].transform.localPosition.y,
            items[selectNum].transform.localPosition.z
            );
        selects[playerNum].transform.localPosition = selectPos;
        selects[playerNum].gameObject.SetActive(true);   
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
            soundManager.Buy();
            
            selects[playerNum].gameObject.SetActive(false);

            moneyScript.UseMoney(itemScripts[selectNum].GetItemPrice());
            //アイテムを取得
            var item = items[selectNum].GetComponent<ItemScript>();
            item.GetItem(playerCons[playerNum]);

            ItemScript.ItemType itemType = itemScripts[selectNum].GetNowItem();
            itemInfo[(int)itemType].SetActive(false);

            //次のプレイヤーへ
            playerNum++;
            isRun = false;

            return true;
        }
        else
        {
            soundManager.Cant();

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
        soundManager.Decision();
        
        pastInfo.SetActive(false);
        
        //次のプレイヤーへ
        playerNum++;
        isRun = false;
    }
}
