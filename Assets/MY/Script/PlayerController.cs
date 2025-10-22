using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerController : CharaScript
{
    [SerializeField] SquareScript squareSC;

    [SerializeField, Header("�U���C���[�W")]//��
    GameObject attackImage;

    //���s������
    bool isRun = false;

    InputAction stickAction;
    InputAction selectAttack;
    InputAction selectMove;

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
        var actionMap = GetComponent<PlayerInput>().currentActionMap;
        stickAction = actionMap["Move"];
        selectMove = actionMap["SelectMove"];
        selectAttack = actionMap["SelectAttack"];

        playerPos = transform.position;
        curPos = playerPos;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// �v���C���[�̈ړ�
    /// </summary>
    /// <param name="x">x������</param>
    /// <param name="z">z������</param>
    public IEnumerator MovePlayer(int x, int z)
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

        //���s���Ȃ�s�����\��
        if (isRun)
        {
            Debug.Log("�\��");
            nextPos = targetPos;
            gridManager.ReserveCell((int)nextPos.z, (int)nextPos.x, this);
            //�m�b�N�o�b�N���邩����
            if (gridManager.CheckCellState((int)nextPos.z, (int)nextPos.x) == CellScript.CellState.enemy)
            {
                //�m�b�N�o�b�N����
                StartCoroutine(KnockBack());
            }
        }

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
        transform.position = targetPos;
        playerPos = transform.position;
        curPos = playerPos;

        //���s���Ȃ猻�ݒn�̃}�X��Ԃ�ύX
        if (isRun)
        {
            Debug.Log("�}�X�X�V");
            gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this);
            //�������}�X����ɂ���
            gridManager.ChangeCellState((int)originPos.z, (int)originPos.x, CellScript.CellState.empty, this);
        }

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

    IEnumerator KnockBack()
    {
        Debug.Log("�ڐG");
        yield return null;
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

        for (int i = 0; i < actionLimit; i++)
        {        
            yield return new WaitForSeconds(0.2f);
            squareSC.SetImage(playerPos);

            Vector2 direction = Vector2.zero;

            //���͂��󂯂čs���\��List�ɒǉ�
            bool isInput = false;
            while (!isInput)
            {
                Vector2 stick = stickAction.ReadValue<Vector2>();
                bool move = selectMove.triggered;
                bool attack = selectAttack.triggered;

                //�}�X�I�����
                if (0.5 < stick.x && !isMove)
                {
                    direction = new Vector2(0, 1);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (0.5 < stick.y && !isMove)
                {
                    direction = new Vector2(-1, 0);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (stick.x < -0.5 && !isMove)
                {
                    direction = new Vector2(0, -1);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {
                        squareSC.SelectImage(playerPos, direction);
                    }
                }
                else if (stick.y < -0.5 && !isMove)
                {
                    direction = new Vector2(1, 0);
                    if (CanMove(new Vector3(curPos.x + direction.x, curPos.y, curPos.z + direction.y)))
                    {                      
                        squareSC.SelectImage(playerPos, direction);
                    }
                }

                //�ړ�����
                if (move && direction != Vector2.zero && !isInput)
                {
                    StartCoroutine(MovePlayer((int)direction.x, (int)direction.y));

                    Action action = new Action(0, direction);
                    actionList.Add(action);
                    
                    isInput = true;
                }
                //�U������
                else if (attack && direction != Vector2.zero && !isInput)
                {
                    //�U�������Ă�

                    Action action = new Action(1, direction);
                    actionList.Add(action);
                    
                    isInput = true;
                }

                yield return null;
            }

            Debug.Log($"�c��s����:{actionLimit - i - 1}");
        }
        
        yield return new WaitForSeconds(0.2f);
        squareSC.DeleteSquare();

        yield return new WaitForSeconds(0.2f);
        turnManager.FinCoroutine();
    }

    /// <summary>
    /// �I���t�F�[�Y�I����ʒu���Z�b�g
    /// </summary>
    public void PosReset()
    {    
        //�ʒu��߂�(��)
        playerPos = startPos;
        transform.position = playerPos;       
    }

    /// <summary>
    /// �s�������s
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutionAct()
    {
        isRun = true;
        
        if (actionList[executionNum].a == 0) //�ړ�
        {
            //�}�X�̊֐����Ă�
            
            StartCoroutine(
                MovePlayer(
                    (int)actionList[executionNum].direction.x,
                    (int)actionList[executionNum].direction.y
                    )
                );
        }
        else //�U��
        {
            //�}�X�̊֐����Ă�

            StartCoroutine(
                Attack(
                    (int)actionList[executionNum].direction.x,
                    (int)actionList[executionNum].direction.y
                    )
                );
        }
        executionNum++;

        yield return new WaitForSeconds(1.0f);
        isRun = false;

        turnManager.FinCoroutine();
    }
}
