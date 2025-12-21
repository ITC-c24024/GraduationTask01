using System.Collections;
using System.Collections.Generic;
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
        
        //選択アイテムをセット
        SetItem();
        
        //選択画面表示
        //selectImage.gameObject.SetActive(true);     

        //プレイヤーが交互に選択する
        for (int i = 0; i < playerCons.Length; i++)
        {
            selects[playerNum].gameObject.SetActive(true);
            yield return StartCoroutine(playerCons[i].SelectContent());
            //isRun = true;

            //while (isRun) yield return null;
            selects[playerNum-1].gameObject.SetActive(false);
        }
        playerNum = 0;

        selectImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

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
                itemNum = Random.Range(0, itemScripts[0].GetItemCount());
                if (!selectList.Contains(itemNum)) break;
            }
            selectList.Add(itemNum);

            itemScripts[i].SetItemType(itemNum);   
        }
    }

    /// <summary>
    /// 選択中Imageを動かす
    /// </summary>
    /// <param name="selectNum">選択する内容のImage要素数</param>
    public void SelectItem(int selectNum)
    {
        Vector3 selectPos = new Vector3(
            items[selectNum].transform.localPosition.x,
            items[selectNum].transform.localPosition.y + 0.02f,
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
            selects[playerNum].gameObject.SetActive(false);

            moneyScript.UseMoney(itemScripts[selectNum].GetItemPrice());
            //アイテムを取得
            var item = items[selectNum].GetComponent<ItemScript>();
            item.GetItem(playerCons[playerNum]);
            items[selectNum].SetActive(false);

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
