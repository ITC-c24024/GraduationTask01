using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharaScript
{
    [SerializeField] TurnManager gameController;
    [SerializeField] SquareScript squareSC;

    [SerializeField, Header("�U���C���[�W")]//��
    GameObject attackImage;

    public struct Action 
    {
        public int a;//0:�ړ�,1:�U��
        public Vector2 direction;//�ړ�����

        public Action(int a,Vector2 direction) 
        {
            this.a = a;
            this.direction = direction;
        }
    }

    //�s���\��List
    List<Action> actionList = new List<Action>();
    //�s���\��
    public int actionlimit = 5;
    //���s��
    int executionNum = 0;

    //�v���C���[�̈ʒu
    public Vector3 playerPos;
    //�s���J�n�ʒu
    Vector3 startPos;

    //�ړ�������
    bool isMove = false;

    void Awake()
    {
        playerPos = transform.position;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �v���C���[�̈ړ�
    /// </summary>
    /// <param name="x">x������</param>
    /// <param name="z">z������</param>
    IEnumerator MovePlayer(int x, int z)
    {
        

        isMove = true;

        //���̈ʒu
        Vector3 originPos = playerPos;
        //�ړ���̈ʒu
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
            
            //���ݒn���v�Z
            Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

            //�v���C���[���ړ�
            transform.position = currentPos;

            yield return null;
        }
        playerPos = transform.position;
        curPos = playerPos;

        isMove = false;
    }

    //���U��
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
    /// �s���I���t�F�[�Y
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectAction()
    {
        Debug.Log("�I��");

        //�s���ʒu��ۑ�
        startPos = playerPos;

        for (int i = 0; i < actionlimit; i++)
        {        
            yield return new WaitForSeconds(0.2f);
            squareSC.SetImage(playerPos);

            Vector2 direction = Vector2.zero;

            //���͂��󂯂čs���\��List�ɒǉ�
            bool isInput = false;
            while (!isInput)
            {    
                //�}�X�I�����
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

                //�ړ�����
                if (Input.GetKeyDown(KeyCode.Return) && direction != Vector2.zero)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));

                    Action action = new Action(0, direction);
                    actionList.Add(action);

                    isInput = true;
                }
                //�U������
                else if (Input.GetKeyDown(KeyCode.Space) && direction != Vector2.zero)
                {
                    //�U�������Ă�

                    Action action = new Action(1, direction);
                    actionList.Add(action);

                    isInput = true;
                }
                
                yield return null;
            }
            Debug.Log($"�c��s���񐔁F{actionlimit - i}");
        }
        
        yield return new WaitForSeconds(0.2f);
        squareSC.DeleteSquare();

        yield return new WaitForSeconds(0.2f);
        //�ʒu��߂�(��)
        playerPos = startPos;
        transform.position = playerPos;

        gameController.FinCoroutine();
    }

    /// <summary>
    /// �s�������s
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutionAct()
    {
        Debug.Log("���s��");

        if (actionList[executionNum].a == 0) //�U��
        {
            StartCoroutine(
                MovePlayer(
                    (int)actionList[executionNum].direction.x,
                    (int)actionList[executionNum].direction.y
                    )
                );
        }
        else //�ړ�
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
