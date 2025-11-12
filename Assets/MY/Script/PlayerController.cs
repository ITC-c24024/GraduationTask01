using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerController : CharaScript
{
    [SerializeField] SquareScript squareSC;

    //ノックバック方向
    Vector2 kbDirection;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;

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

        playerPos = transform.position;
        curPos = playerPos;
    }
    
    void Start()
    {
        SetPlayerState();

        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }

    /// <summary>
    /// ゲーム開始時に、マスにプレイヤーを設定
    /// </summary>
    public void SetPlayerState()
    {
        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
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
        //元居たマスを空にする
        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);

        isMove = false;
    }

    //攻撃
    IEnumerator Attack(int x, int z,int amount)
    {       
        animator.SetTrigger("IsAttack");
        shadowAnim.SetTrigger("IsAttack");

        yield return new WaitForSeconds(0.8f);

        //敵にダメージを与える
        gridManager.SendDamage(z, x, amount, false, default);

        yield return new WaitForSeconds(0.2f);
    }

    //ダメージを受けてノックバックさせる
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;

        //HPが0なら死亡
        if (hp <= 0)
        {
            alive = false;
            StartCoroutine(Dead());
        }
        //ノックバックできる場合
        else if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
    }

    IEnumerator KnockBack(Vector2 kbDir)
    {
        animator.SetTrigger("IsKB");
        shadowAnim.SetTrigger("IsKB");

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

        //マス更新
        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
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
}
