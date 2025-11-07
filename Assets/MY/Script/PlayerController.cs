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

    //ノックバック方向
    Vector2 kbDirection;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;

    //プレイヤーの位置
    public Vector3 playerPos;

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

        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
        //元居たマスを空にする
        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);

        isMove = false;
    }

    //仮攻撃
    IEnumerator Attack(int x, int z)
    {
        attackImage.transform.position = new Vector3(x, 0.102f, z);
        attackImage.SetActive(true);
        //敵にダメージを与える
        gridManager.SendDamage(z, x, damage, false, default);

        yield return new WaitForSeconds(0.2f);
        attackImage.SetActive(false);
    }

    //ダメージを受けてノックバックさせる
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;

        //HPが0なら死亡
        if (hp <= 0) StartCoroutine(Dead());
        //ノックバックできる場合
        else if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
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
        playerPos = targetPos;
        curPos = playerPos;

        //マス更新
        gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
        gridManager.LeaveCell((int)originPos.z, (int)originPos.x, this);
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
                //攻撃入力
                else if (attack && direction != Vector2.zero && !isInput)
                {
                    //攻撃処理呼ぶ
                    StartCoroutine(Attack((int)(playerPos.x + direction.x), (int)(playerPos.z + direction.y)));
                    
                    isInput = true;
                }

                yield return null;
            }
        }        
        yield return new WaitForSeconds(0.5f);
        squareSC.DeleteSquare();

        turnManager.FinCoroutine();
    }
}
