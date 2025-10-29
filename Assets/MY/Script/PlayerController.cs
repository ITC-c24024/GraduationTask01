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

    //�m�b�N�o�b�N����
    bool isKnockBack = false;

    //�m�b�N�o�b�N����
    Vector2 kbDirection;

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

    /// <summary>
    /// �v���C���[�̈ړ�
    /// </summary>
    /// <param name="x">x������</param>
    /// <param name="z">z������</param>
    public IEnumerator MovePlayer(int x, int z)
    {
        isMove = true;

        //���̈ʒu
        Vector3 originPos = curPos;
        //�ړ���̈ʒu
        Vector3 targetPos = new Vector3(
            playerPos.x + x,
            playerPos.y,
            playerPos.z + z
            );

        //���s���Ȃ�s�����\��
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
            
            //���ݒn���v�Z
            Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

            //�v���C���[���ړ�
            transform.position = currentPos;

            yield return null;
        }
        transform.position = targetPos;
        playerPos = transform.position;
        curPos = playerPos;

        //�m�b�N�o�b�N
        if (isKnockBack)
        {            
            ReciveDamage(recieveDamage, kbDirection);

            yield break;
        }

        //���s���Ȃ猻�ݒn�̃}�X��Ԃ�ύX
        if (isRun)
        {
            gridManager.ChangeCellState((int)curPos.z, (int)curPos.x, CellScript.CellState.player, this, default);
            //�������}�X����ɂ���
            gridManager.LeaveCell((int)originPos.z, (int)originPos.x,this);

            turnManager.FinCoroutine();
        }

        isMove = false;
        isRun = false;  
    }

    //���U��
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

    //�_���[�W���󂯂ăm�b�N�o�b�N������
    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        hpSlider.value = hp;

        if (kbDir != Vector2.zero) StartCoroutine(KnockBack(kbDir));
    }

    IEnumerator KnockBack(Vector2 kbDir)
    {
        Debug.Log("KB");

        //���̈ʒu
        Vector3 originPos = playerPos;
        //�ړ���̈ʒu
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

            //���ݒn���v�Z
            Vector3 currentPos = Vector3.Lerp(originPos, targetPos, time / required);

            //�v���C���[���ړ�
            transform.position = currentPos;

            yield return null;
        }
        transform.position = targetPos;
        playerPos = transform.position;
        curPos = playerPos;

        //�}�X�X�V
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
    /// �s���I���t�F�[�Y
    /// </summary>
    /// <returns></returns>
    public IEnumerator SelectAction()
    {
        //�s���ʒu��ۑ�
        startPos = playerPos;

        for (int i = 0; i < actionLimit; i++)
        {        
            yield return new WaitForSeconds(0.5f);
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
        }
        
        yield return new WaitForSeconds(0.5f);
        squareSC.DeleteSquare();
        turnManager.FinCoroutine();
    }

    /// <summary>
    /// �I���t�F�[�Y�I����ʒu���Z�b�g
    /// </summary>
    public void PosReset()
    {    
        //�ʒu��߂�(��)
        playerPos = startPos;
        curPos = playerPos;
        transform.position = playerPos;       
    }

    /// <summary>
    /// �s�������s
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutionAct(int i)
    {
        isRun = true;

        int x = (int)actionList[i].direction.x;
        int z = (int)actionList[i].direction.y;
        
        if (actionList[i].a == 0) //�ړ�
        {
            //�������m�F
            var result = gridManager.ChangeCellState(
                (int)playerPos.z + z,
                (int)playerPos.x + x,
                CellScript.CellState.player,
                this,
                new Vector2Int(x, z)
                );
            //������Ȃ畁�ʂɓ���
            if(result.canMove) StartCoroutine(MovePlayer(x, z));
            else//�m�b�N�o�b�N��\�肵�ē���
            {        
                isKnockBack = true;
                kbDirection = result.knockbackDir;
                recieveDamage = result.damage;
                StartCoroutine(MovePlayer(x, z));
            }
        }
        else //�U��
        {
            //�}�X���U���\��ɂ���
            //gridManager.SendDamage((int)playerPos.z + z, (int)playerPos.x + x, damage, false);
            //�^�C�~���O����   
            yield return new WaitForSeconds(0.5f);
            turnManager.FinCoroutine();
            yield return null;
            yield return new WaitForSeconds(1.0f);
            
            //�}�X���U���\��ɂ���
            gridManager.SendDamage((int)playerPos.z + z, (int)playerPos.x + x, damage, false);            
            StartCoroutine(Attack(x, z));
        }

        yield return null;
    }
}
