using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(PlayerInput))]

public class PlayerController : CharaScript
{
    [SerializeField] GameController gameCon;
    [SerializeField] SquareScript squareSC;
    [SerializeField] SelectContentScript SelectContentSC;
    [SerializeField, Header("味方プレイヤー")] PlayerController playerCon;

    [SerializeField, Header("HPバーの中央Prefab")]
    GameObject midBar;

    [SerializeField, Header("アイテムアイコン")]
    Image[] itemIcon;

    //ノックバック方向
    Vector2 kbDirection;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;
    InputAction speedUp;

    [SerializeField, Header("プレイヤーの初期配置")]
    Vector3 initialPos;

    //プレイヤーの位置
    public Vector3 playerPos;
    //生存判定
    public bool alive = true;
    //移動中判定
    bool isMove = false;

    public int maxHP;

    public ItemScript.ItemType haveItem;

    void Awake()
    {
        var actionMap = GetComponent<PlayerInput>().currentActionMap;
        stickAction = actionMap["Move"];
        selectMove = actionMap["SelectMove"];
        selectAttack = actionMap["SelectAttack"];
        speedUp = actionMap["SpeedUp"];
    }
    
    void Start()
    {
        hp = maxHP;
    }

    void Update()
    {
        //倍速
        bool speedUpAct = speedUp.triggered;
        if (speedUpAct) gameCon.SpeedUp();
    }

    public void SetPlayer()
    {
        playerPos = initialPos;
        curPos = transform.position;

        StartCoroutine(MovePlayer(0,0));

        charaState = CharaState.player;

        hpObj.SetActive(true);
    }

    public void GetItem(ItemScript.ItemType itemType)
    {
        if (itemType != ItemScript.ItemType.hpUp && haveItem != ItemScript.ItemType.hpUp) 
        {
            //持っていたアイテムを失う
            LoseItem(haveItem);                  
        }

        //新しくアイテムを得る
        haveItem = itemType;
        if(haveItem != ItemScript.ItemType.hpUp)
        {
            itemIcon[(int)haveItem - 1].gameObject.SetActive(true);
        }

        switch (itemType)
        {
            case ItemScript.ItemType.hpUp:
                if (hp < maxHP)
                {
                    hp++;
                    hpBar[hp - 1].SetActive(true);
                }              
                break;
            case ItemScript.ItemType.damageUp:
                damage++;
                break;
            case ItemScript.ItemType.moneyUp:
                moneyScript.SetRate(0.5f);
                break;
            case ItemScript.ItemType.actionLimitUp:
                actionLimit++;
                break;
            case ItemScript.ItemType.maxHpUp:
                maxHP++;
                AddHpBar();
                break;
        }
    }

    public void LoseItem(ItemScript.ItemType itemType)
    {
        itemIcon[(int)itemType - 1].gameObject.SetActive(false);

        switch (itemType)
        {
            case ItemScript.ItemType.damageUp:
                damage--;
                break;
            case ItemScript.ItemType.moneyUp:
                moneyScript.SetRate(-0.5f);
                break;
            case ItemScript.ItemType.actionLimitUp:
                actionLimit--;
                break;
            case ItemScript.ItemType.maxHpUp:
                maxHP--;
                break;
        }
    }

    /// <summary>
    /// HPバーの個数を増やす
    /// </summary>
    void AddHpBar()
    {
        GameObject mid = Instantiate(
            midBar, 
            hpBar[hpBar.Count - 1].transform.position, 
            hpBar[hpBar.Count - 1].transform.rotation
            );
        mid.transform.parent = hpObj.transform;

        //右端のバーを一つ右に移動
        GameObject parent = hpBar[hpBar.Count - 1].transform.parent.gameObject;
        parent.transform.localPosition = new Vector3(
            parent.transform.localPosition.x + 0.135f,
            parent.transform.localPosition.y,
            parent.transform.localPosition.z
            );

        //リストの要素を1つ増やす
        hpBar.Add(null);
        //右端のバーをリストの一番後ろに移動
        hpBar[hpBar.Count - 1] = hpBar[hpBar.Count - 2];
        //新しく追加したバーをリストに追加
        GameObject fill = mid.transform.GetChild(0).gameObject;
        hpBar[hpBar.Count - 2] = fill;

        if (hp < maxHP)
        {
            hp++;
            hpBar[hp - 1].SetActive(true);
        }
        /*
        if (hp < hpBar.Count - 2)
        {
            fill.SetActive(false);
        }*/

        //増えたバーの分位置をずらす
        foreach(var fililObj in hpBar)
        {
            GameObject parentObj = fililObj.transform.parent.gameObject;
            parentObj.transform.localPosition = new Vector3(
                parentObj.transform.localPosition.x-0.135f/2, 
                parentObj.transform.localPosition.y, 
                parentObj.transform.localPosition.z
                );
        }
    }

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    /// <param name="x">x軸入力</param>
    /// <param name="z">z軸入力</param>
    public IEnumerator MovePlayer(int x, int z)
    {
        isMove = true;

        //元の位置
        Vector3 originPos = curPos;
        //移動先の位置
        Vector3 targetPos = new Vector3(
            playerPos.x + x,
            playerPos.y,
            playerPos.z + z
            );

        animator.SetBool("IsWalk", true);
        shadowAnim.SetBool("IsWalk", true);
        float time = 0;
        float required = 0.5f;
        //ゲーム開始していないなら
        if (!gameCon.isStart) required = 1.5f;
        while (time < required)
        {
            time += Time.deltaTime;
            
            //現在地を計算
            Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

            //プレイヤーを移動
            transform.position = currentPos;

            yield return null;
        }
        transform.position = targetPos;
        playerPos = transform.position;
        curPos = playerPos;
        animator.SetBool("IsWalk", false);
        shadowAnim.SetBool("IsWalk", false);

        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);

        //ゲーム進行中なら
        if (gameCon.isStart)
        {
            //元居たマスを空にする
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
        }     

        isMove = false;
    }

    //攻撃
    IEnumerator Attack(int x, int z,int amount)
    {
        if (gridManager.CheckCellState(z, x)==CellScript.CellState.enemy)
        {
            animator.SetTrigger("IsAttack");
            shadowAnim.SetTrigger("IsAttack");

            yield return new WaitForSeconds(0.8f);

            //敵にダメージを与える
            gridManager.SendDamage(z, x, amount, false, default);

            yield return new WaitForSeconds(0.2f);
        }  
    }

    //ダメージを受けてノックバックさせる
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        if (hp < 0) hp = 0;

        for (int i = hp; i < hpBar.Count; i++)
        {
            hpBar[i].gameObject.SetActive(false);
        }

        //ノックバックできる場合
        if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
        //HPが0なら死亡
        else if(hp <= 0)
        {
            alive = false;
            StartCoroutine(Dead());
            gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.dead, this, default);
        }
    }

    IEnumerator KnockBack(Vector2 kbDir)
    {
        //元の位置
        Vector3 originPos = playerPos;
        //移動先の位置
        Vector3 targetPos = new Vector3(
            playerPos.x + kbDir.x,
            playerPos.y,
            playerPos.z + kbDir.y
            );

        //HPが0なら死亡
        if (hp <= 0)
        {
            alive = false;
            StartCoroutine(Dead());
            gridManager.ChangeCellState((int)targetPos.z, (int)targetPos.x, CellScript.CellState.dead, this, default);
        }
        else
        {
            gridManager.ChangeCellState((int)targetPos.z, (int)targetPos.x, CellScript.CellState.player, this, default);
            animator.SetTrigger("IsKB");
            shadowAnim.SetTrigger("IsKB");
        }

        //マス更新(KBが確定しているので、ここで更新)
        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);

        float time = 0;
        float required = 0.1f;
        while (time < required)
        {
            time += Time.deltaTime;

            //現在地を計算
            Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

            //プレイヤーを移動
            transform.position = currentPos;

            yield return null;
        }
        transform.position = targetPos;
        playerPos = targetPos;
        curPos = playerPos;
    }

    /// <summary>
    /// 四方に攻撃
    /// </summary>
    public IEnumerator SurrundAttack()
    {      
        //隣接敵の位置
        var enemyPos = new List<Vector2>();
        //四方に敵がいるか確認
        for (int i = -1; i <= 1; i++) 
        {
            Vector2 checkPos = new Vector2(playerPos.x + i, playerPos.z);
            if (checkPos.x < 0 || 7 < checkPos.x || i == 0) continue;
            if (gridManager.CheckCellState((int)checkPos.y, (int)checkPos.x) == CellScript.CellState.enemy)
            {
                enemyPos.Add(checkPos);
            }        
        }
        for (int i = -1; i <= 1; i++) 
        {
            Vector2 checkPos = new Vector2(playerPos.x, playerPos.z + i);
            if (checkPos.y < 0 || 7 < checkPos.y || i == 0) continue;
            if (gridManager.CheckCellState((int)checkPos.y, (int)checkPos.x) == CellScript.CellState.enemy)
            {
                enemyPos.Add(checkPos);
            }
        }

        //隣接している敵の数に応じて攻撃力UP
        int combo = damage * enemyPos.Count;

        for (int i = 0; i < enemyPos.Count; i++)
        {   
            //敵にダメージを与える
            yield return StartCoroutine(Attack((int)enemyPos[i].x, (int)enemyPos[i].y, combo));
            //yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// 蘇生できる味方がいるか確認
    /// </summary>
    public IEnumerator ResurrectionCheck()
    {
        //四方にを確認
        for (int i = -1; i <= 1; i++)
        {
            Vector2 checkPos = new Vector2(playerPos.x + i, playerPos.z);
            if (checkPos.x < 0 || 7 < checkPos.x || i == 0) continue;
            if (gridManager.CheckCellState((int)checkPos.y, (int)checkPos.x) == CellScript.CellState.dead)
            {
                yield return StartCoroutine(playerCon.Resurrection());
            }
        }
        for (int i = -1; i <= 1; i++)
        {
            Vector2 checkPos = new Vector2(playerPos.x, playerPos.z + i);
            if (checkPos.y < 0 || 7 < checkPos.y || i == 0) continue;
            if (gridManager.CheckCellState((int)checkPos.y, (int)checkPos.x) == CellScript.CellState.dead)
            {
                yield return StartCoroutine(playerCon.Resurrection());
            }
        }       
    }

    /// <summary>
    /// 蘇生する
    /// </summary>
    public IEnumerator Resurrection()
    {
        Debug.Log("復活");
        hp = 2;
        hpObj.SetActive(true);
        for (int i = 0; i < hp; i++)
        {
            hpBar[i].SetActive(true);
        }

        alive = true;
        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);

        animator.SetBool("IsDead", false);
        shadowAnim.SetBool("IsDead", false);

        waveManager.PlayerResurrection();

        yield return null;
    }

    /// <summary>
    /// 行動選択フェーズ
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectAction()
    {
        //行動回数分
        for (int i = 0; i < actionLimit; i++)
        {        
            yield return new WaitForSeconds(0.5f);
            squareSC.SetImage(playerPos);

            Vector2 direction = Vector2.zero;

            //入力を受けて行動予定Listに追加
            bool isInput = false;
            while (!isInput)
            {
                Vector2 stick = stickAction.ReadValue<Vector2>();//スティックで移動方向指定
                bool move = selectAttack.triggered;//Aボタンで移動

                //マス選択入力
                if (0.5 < stick.x && !isMove)
                {
                    direction = new Vector2(0, 1);
                }
                else if (0.5 < stick.y && !isMove)
                {
                    direction = new Vector2(-1, 0);             
                }
                else if (stick.x < -0.5 && !isMove)
                {
                    direction = new Vector2(0, -1);
                }
                else if (stick.y < -0.5 && !isMove)
                {
                    direction = new Vector2(1, 0);
                }

                //進みたいマスの座標
                Vector3 selectPos = new Vector3(playerPos.x + direction.x, playerPos.y, playerPos.z + direction.y);
                if (InStage(selectPos) && CanMove(selectPos))
                {
                    squareSC.SelectImage(playerPos, direction);
                }
                else
                {
                    direction = Vector2.zero;
                    squareSC.DeleteSelect();
                }

                //移動入力
                if (move && direction != Vector2.zero)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));
                    
                    isInput = true;
                }

                yield return null;
            }
            squareSC.DeleteSquare();
            yield return new WaitForSeconds(0.5f);    
        }
        yield return StartCoroutine(SurrundAttack());
    }
    
    /// <summary>
    /// 強化内容を選ぶ
    /// </summary>
    public IEnumerator SelectContent(int originNum)
    {
        bool isSelect = false;
        int selectNum = originNum;

        while (!isSelect)
        {
            Vector2 stick = stickAction.ReadValue<Vector2>();
            bool decision = selectAttack.triggered;
            bool skip = selectMove.triggered;
            //選択
            if (0.5 < stick.x)
            {
                if (SelectContentSC.CanSelect(selectNum + 1,1)) 
                {
                    selectNum++;
                    SelectContentSC.SelectItem(selectNum);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }
            else if (stick.x < -0.5)
            {
                if (SelectContentSC.CanSelect(selectNum - 1,-1))
                {
                    selectNum--;
                    SelectContentSC.SelectItem(selectNum);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }
            //決定
            if (decision)
            {
                 isSelect = SelectContentSC.DecisionItem(selectNum);
            }
            //スキップ
            if (skip)
            {
                SelectContentSC.Skip();
                break;
            }

            yield return null;
        }
    }
}
