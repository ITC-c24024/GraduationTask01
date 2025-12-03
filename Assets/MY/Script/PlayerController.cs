using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerController : CharaScript
{
    [SerializeField] GameController gameCon;
    [SerializeField] SquareScript squareSC;
    [SerializeField] SelectContentScript SelectContentSC;

    //ノックバック方向
    Vector2 kbDirection;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;
    InputAction speedUp;

    [SerializeField, Header("プレイヤーの初期配置")]
    Vector3 initialPos;

    //クリティカル率
    public float criticalRate = 0;

    //プレイヤーの位置
    public Vector3 playerPos;
    //生存判定
    public bool alive = true;
    //移動中判定
    bool isMove = false;

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

        hpSlider.gameObject.SetActive(true);
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    } 

    /// <summary>
    /// HPを設定
    /// </summary>
    /// <param name="amount">変化量</param>
    public void SetHP(int amount)
    {
        hp += amount;
    }
    /// <summary>
    /// 攻撃力を設定
    /// </summary>
    /// <param name="amount">変化量</param>
    public void SetDamage(int amount)
    {
        damage += amount;
    }
    /// <summary>
    /// クリティカル率を設定
    /// </summary>
    /// <param name="amount">変化量</param>
    public void SetCriticalRate(float amount)
    {
        criticalRate += amount;
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
        hpSlider.value = hp;
        
        
        //ノックバックできる場合
        if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
        //HPが0なら死亡
        else if (hp <= 0)
        {
            alive = false;
            StartCoroutine(Dead());
            gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.dead, this, default);
        }
    }

    IEnumerator KnockBack(Vector2 kbDir)
    {
        //HPが0なら死亡
        if (hp <= 0)
        {
            alive = false;
            StartCoroutine(Dead());
        }
        else
        {
            animator.SetTrigger("IsKB");
            shadowAnim.SetTrigger("IsKB");
        }     

        //元の位置
        Vector3 originPos = playerPos;
        //移動先の位置
        Vector3 targetPos = new Vector3(
            playerPos.x + kbDir.x,
            playerPos.y,
            playerPos.z + kbDir.y
            );

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

        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
        //マス更新
        if (alive)gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
        else gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.dead, this, default);      
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
            StartCoroutine(Attack((int)enemyPos[i].x, (int)enemyPos[i].y, combo));
            yield return new WaitForSeconds(1.0f);
        }

        turnManager.FinCoroutine();
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
                    if (CanMove(new Vector3(playerPos.x + direction.x, playerPos.y, playerPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                    else
                    {
                        direction = Vector2.zero;
                        squareSC.DeleteSelect();
                    }
                }
                else if (0.5 < stick.y && !isMove)
                {
                    direction = new Vector2(-1, 0);
                    if (CanMove(new Vector3(playerPos.x + direction.x, playerPos.y, playerPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                    else
                    {
                        direction = Vector2.zero;
                        squareSC.DeleteSelect();
                    }
                }
                else if (stick.x < -0.5 && !isMove)
                {
                    direction = new Vector2(0, -1);
                    if (CanMove(new Vector3(playerPos.x + direction.x, playerPos.y, playerPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                    else
                    {
                        direction = Vector2.zero;
                        squareSC.DeleteSelect();
                    }
                }
                else if (stick.y < -0.5 && !isMove)
                {
                    direction = new Vector2(1, 0);
                    if (CanMove(new Vector3(playerPos.x + direction.x, playerPos.y, playerPos.z + direction.y)))
                    {                      
                        squareSC.SelectImage(playerPos, direction);
                    }
                    else
                    {
                        direction = Vector2.zero;
                        squareSC.DeleteSelect();
                    }
                }

                //移動入力
                if (move && direction != Vector2.zero)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));
                    
                    isInput = true;
                }
                /*
                //攻撃入力
                else if (attack && direction != Vector2.zero && !isInput)
                {
                    //攻撃処理呼ぶ
                    StartCoroutine(Attack((int)(playerPos.x + direction.x), (int)(playerPos.z + direction.y)));
                    
                    isInput = true;
                }
                */

                yield return null;
            }
            squareSC.DeleteSquare();
        }        
        yield return new WaitForSeconds(0.5f);
        squareSC.DeleteSquare();

        StartCoroutine(SurrundAttack());   
    }
    
    /// <summary>
    /// 強化内容を選ぶ
    /// </summary>
    public IEnumerator SelectContent()
    {
        bool isSelect = false;
        int selectNum = 0;

        while (!isSelect)
        {
            Vector2 stick = stickAction.ReadValue<Vector2>();
            bool decision = selectAttack.triggered;
            if (0.5 < stick.x)
            {
                if (selectNum != 2)
                {
                    selectNum++;
                    SelectContentSC.SelectItem(selectNum);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else if (stick.x < -0.5)
            {
                if (selectNum != 0)
                {
                    selectNum--;
                    SelectContentSC.SelectItem(selectNum);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            if (decision)
            {
                isSelect = true;
                SelectContentSC.DecisionItem(selectNum);
            }

            yield return null;
        }
    }
}
