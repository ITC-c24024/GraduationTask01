using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharaScript
{
    [SerializeField] TurnManager gameController;
    [SerializeField] SquareScript squareSC;

    [SerializeField, Header("攻撃イメージ")]//仮
    GameObject attackImage;

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
    //行動可能回数
    public int actionlimit = 5;
    //実行回数
    int executionNum = 0;

    //プレイヤーの位置
    public Vector3 playerPos;
    //行動開始位置
    Vector3 startPos;

    //移動中判定
    bool isMove = false;

    void Awake()
    {
        playerPos = transform.position;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    /// <param name="x">x軸入力</param>
    /// <param name="z">z軸入力</param>
    IEnumerator MovePlayer(int x, int z)
    {
        

        isMove = true;

        //元の位置
        Vector3 originPos = playerPos;
        //移動先の位置
        Vector3 targetPos = new Vector3(
            playerPos.x + x,
            playerPos.y,
            playerPos.z + z
            );

        nextPos = targetPos;

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
        playerPos = transform.position;
        curPos = playerPos;

        isMove = false;
    }

    //仮攻撃
    IEnumerator Attack(int x, int z)
    {
        attackImage.transform.position = new Vector3(
            playerPos.x + x,
            0.01f,
            playerPos.z + z
            );
        attackImage.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackImage.SetActive(false);
    }

    /// <summary>
    /// 行動選択フェーズ
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectAction()
    {
        Debug.Log("選択中");

        //行動位置を保存
        startPos = playerPos;

        for (int i = 0; i < actionlimit; i++)
        {        
            yield return new WaitForSeconds(0.2f);
            squareSC.SetImage(playerPos);

            Vector2 direction = Vector2.zero;

            //入力を受けて行動予定Listに追加
            bool isInput = false;
            while (!isInput)
            {    
                //マス選択入力
                if (Input.GetKeyDown(KeyCode.RightArrow) && !isMove)
                {
                    direction = new Vector2(0, 1);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y))) 
                    {                       
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && !isMove)
                {
                    direction = new Vector2(-1, 0);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {                       
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isMove)
                {
                    direction = new Vector2(0, -1);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {                      
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && !isMove)
                {
                    direction = new Vector2(1, 0);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {                      
                        squareSC.SelectImage(playerPos, direction);
                    }
                }

                //移動入力
                if (Input.GetKeyDown(KeyCode.Return) && direction != Vector2.zero)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));

                    Action action = new Action(0, direction);
                    actionList.Add(action);

                    isInput = true;
                }
                //攻撃入力
                else if (Input.GetKeyDown(KeyCode.Space) && direction != Vector2.zero)
                {
                    //攻撃処理呼ぶ

                    Action action = new Action(1, direction);
                    actionList.Add(action);

                    isInput = true;
                }
                
                yield return null;
            }
            Debug.Log($"残り行動回数：{actionlimit - i}");
        }
        
        yield return new WaitForSeconds(0.2f);
        squareSC.DeleteSquare();

        yield return new WaitForSeconds(0.2f);
        //位置を戻す(仮)
        playerPos = startPos;
        transform.position = playerPos;

        gameController.FinCoroutine();
    }

    /// <summary>
    /// 行動を実行
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutionAct()
    {
        Debug.Log("実行中");

        if (actionList[executionNum].a == 0) //攻撃
        {
            StartCoroutine(
                MovePlayer(
                    (int)actionList[executionNum].direction.x,
                    (int)actionList[executionNum].direction.y
                    )
                );
        }
        else //移動
        {
            StartCoroutine(
                Attack(
                    (int)actionList[executionNum].direction.x,
                    (int)actionList[executionNum].direction.y
                    )
                );
        }      
        executionNum++;

        yield return new WaitForSeconds(0.2f);

        gameController.FinCoroutine();
    }
}
