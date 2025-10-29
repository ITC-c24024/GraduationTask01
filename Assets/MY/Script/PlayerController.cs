using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerController : CharaScript
{
    [SerializeField] SquareScript squareSC;

    [SerializeField, Header("攻撃イメージ")]//仮
    GameObject attackImage;

    //実行中判定
    bool isRun = false;

    //ノックバック判定
    bool isKnockBack = false;

    //ノックバック方向
    Vector2 kbDirection;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;

    public struct Action 
    {
        public int a;//0:移動,1:攻撃
        public Vector2 direction;//移動方向

        public Action(int a,Vector2 direction) 
        {
            this.a = a;
            this.direction = direction;
        }
    }

    //行動予定List
    List<Action> actionList = new List<Action>();

    //プレイヤーの位置
    public Vector3 playerPos;
    //行動開始位置
    Vector3 startPos;

    //移動中判定
    bool isMove = false;

    void Awake()
    {
        var actionMap = GetComponent<PlayerInput>().currentActionMap;
        stickAction = actionMap["Move"];
        selectMove = actionMap["SelectMove"];
        selectAttack = actionMap["SelectAttack"];

        playerPos = transform.position;
        curPos = playerPos;
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

        //実行中なら行き先を予約
        if (isRun)
        {
            nextPos = targetPos;
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);
        }

        float time = 0;
        float required = 0.5f;
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

        //ノックバック
        if (isKnockBack)
        {            
            ReciveDamage(recieveDamage, kbDirection);

            yield break;
        }

        //実行中なら現在地のマス状態を変更
        if (isRun)
        {
            gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
            //元居たマスを空にする
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x,this);

            turnManager.FinCoroutine();
        }

        isMove = false;
        isRun = false;  
    }

    //仮攻撃
    IEnumerator Attack(int x, int z)
    {
        attackImage.transform.position = new Vector3(
            playerPos.x + x,
            0.1f,
            playerPos.z + z
            );
        attackImage.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackImage.SetActive(false);
    }

    //ダメージを受けてノックバックさせる
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;

        if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
    }

    IEnumerator KnockBack(Vector2 kbDir)
    {
        Debug.Log("KB");

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
        playerPos = transform.position;
        curPos = playerPos;

        //マス更新
        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
        gridManager.LeaveCell((int)nextPos.z, (int)nextPos.x, this);

        isRun = false;
        if (isKnockBack)
        {
            isKnockBack = false;
            turnManager.FinCoroutine();
        }
    }

    /// <summary>
    /// 行動選択フェーズ
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectAction()
    {
        //行動位置を保存
        startPos = playerPos;

        for (int i = 0; i < actionLimit; i++)
        {        
            yield return new WaitForSeconds(0.5f);
            squareSC.SetImage(playerPos);

            Vector2 direction = Vector2.zero;

            //入力を受けて行動予定Listに追加
            bool isInput = false;
            while (!isInput)
            {
                Vector2 stick = stickAction.ReadValue<Vector2>();
                bool move = selectMove.triggered;
                bool attack = selectAttack.triggered;

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
                if (move && direction != Vector2.zero && !isInput)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));

                    Action action = new Action(0, direction);
                    actionList.Add(action);
                    
                    isInput = true;
                }
                //攻撃入力
                else if (attack && direction != Vector2.zero && !isInput)
                {
                    //攻撃処理呼ぶ

                    Action action = new Action(1, direction);
                    actionList.Add(action);
                    
                    isInput = true;
                }

                yield return null;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        squareSC.DeleteSquare();
        turnManager.FinCoroutine();
    }

    /// <summary>
    /// 選択フェーズ終了後位置リセット
    /// </summary>
    public void PosReset()
    {    
        //位置を戻す(仮)
        playerPos = startPos;
        curPos = playerPos;
        transform.position = playerPos;       
    }

    /// <summary>
    /// 行動を実行
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutionAct(int i)
    {
        isRun = true;

        int x = (int)actionList[i].direction.x;
        int z = (int)actionList[i].direction.y;
        
        if (actionList[i].a == 0) //移動
        {
            //動きを確認
            var result = gridManager.ChangeCellState(
                (int)playerPos.z + z,
                (int)playerPos.x + x,
                CellScript.CellState.player,
                this,
                new Vector2Int(x, z)
                );
            //動けるなら普通に動く
            if(result.canMove) StartCoroutine(MovePlayer(x, z));
            else//ノックバックを予定して動く
            {        
                isKnockBack = true;
                kbDirection = result.knockbackDir;
                recieveDamage = result.damage;
                StartCoroutine(MovePlayer(x, z));
            }
        }
        else //攻撃
        {
            //マスを攻撃予定にする
            //gridManager.SendDamage((int)playerPos.z + z, (int)playerPos.x + x, damage, false);
            //タイミング調整   
            yield return new WaitForSeconds(0.5f);
            turnManager.FinCoroutine();
            yield return null;
            yield return new WaitForSeconds(1.0f);
            
            //マスを攻撃予定にする
            gridManager.SendDamage((int)playerPos.z + z, (int)playerPos.x + x, damage, false);            
            StartCoroutine(Attack(x, z));
        }

        yield return null;
    }
}
