using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    /// <summary>
    /// �}�X�̏��
    /// </summary>
    public enum CellState
    {
        empty,//��
        player,//�v���C���[
        enemy,//�G
    }

    /// <summary>
    /// TryEnter�֐��Ăт����̌��ʗp
    /// </summary>
    public struct TryEnterResult
    {
        public bool canMove;
        public int damage;
        public Vector2Int knockbackDir;
    }

    Vector2Int cellPos = new();

    [SerializeField, Header("�}�X�̏��")]
    CellState state = CellState.empty;

    [SerializeField, Header("�\��")]
    List<CharaScript> reserveList = new();

    [SerializeField, Header("�v���C���[��")]
    List<CharaScript> playerList = new();

    [SerializeField, Header("�G��")]
    List<CharaScript> enemyList = new();

    /// <summary>
    /// ���j�b�g���v��
    /// </summary>
    int UnitCount => playerList.Count + enemyList.Count;

    public void SetPosition(Vector2Int pos)
    {
        cellPos = pos;
    }

    /// <summary>
    /// �\��p�֐�(�X�e�[�g�؂�ւ��̑O�ɕK���Ă�)
    /// </summary>
    /// <param name="unitSC">�Ăт������̃��j�b�g�X�N���v�g</param>
    public void ReserveState(CharaScript unitSC)
    {
        if (!reserveList.Contains(unitSC))
        {
            reserveList.Add(unitSC);
        }
    }


    /// <summary>
    /// �\�ł���΃}�X�̏�Ԃ�ς���
    /// </summary>
    /// <param name="newState">���(empty=��, player=�v���C���[, enemy=�G)</param>
    /// <param name="unitSC">�Ăт������̃��j�b�g�X�N���v�g</param>
    /// <param name="direction">�m�b�N�o�b�N����(�G�̏ꍇ�̂ݎg�p)</param>
    public TryEnterResult TryEnter(CellState newState, CharaScript unitSC, Vector2Int direction = default)
    {
        TryEnterResult result = new()
        {
            canMove = false,
            damage = 0,
            knockbackDir = Vector2Int.zero
        };

        //�}�X�ɑ����j�b�g������ꍇ
        if (UnitCount > 0)
        {
            //�v���C���[�̏ꍇ
            if (newState == CellState.player)
            {
                //�G�X�e�[�g��������m�b�N�o�b�N������
                if (state == CellState.enemy)
                {
                    foreach (var enemy in enemyList)
                    {
                        result.damage += enemy.damage;
                    }
                }
                else
                {
                    result.canMove = true;
                }
            }
            //�G�̏ꍇ(�v���C���[��������)
            else if (newState == CellState.enemy && state == CellState.player)
            {
                //�m�b�N�o�b�N�����̃}�X�����邩�A�󂩂ǂ����`�F�b�N�̏�����ǋL�K�v
                var ok = false;
                if (ok)
                {
                    result.canMove = true;
                    result.knockbackDir = direction;
                }

                result.damage += unitSC.damage;
            }
        }
        //�}�X�ɑ����j�b�g�����Ȃ��ꍇ
        else if (UnitCount <= 0)
        {
            result.canMove = true;
        }

        if (result.canMove)
        {
            ChangeState(newState, unitSC);
        }

        return result;
    }



    /// <summary>
    /// �X�e�[�g�؂�ւ�����
    /// </summary>
    /// <param name="newState">�؂�ւ���X�e�[�g�w��</param>
    /// <param name="unitSC">���j�b�g�X�N���v�g</param>
    void ChangeState(CellState newState, CharaScript unitSC)
    {
        if (newState == CellState.player && !playerList.Contains(unitSC))
        {
            playerList.Add(unitSC);
        }
        else if (newState == CellState.enemy && !enemyList.Contains(unitSC))
        {
            enemyList.Add(unitSC);
        }

        state = newState;
    }


    /// <summary>
    /// �e���j�b�g���}�X�𗣂��ۂɌĂԊ֐�
    /// �}�X�̏�Ԃ���ɂ���
    /// </summary>
    /// <param name="unitSC">���j�b�g�̃X�N���v�g</param>
    public void Leave(CharaScript unitSC)
    {
        playerList.Remove(unitSC);
        enemyList.Remove(unitSC);
        reserveList.Remove(unitSC);

        if (UnitCount == 0)
        {
            state = CellState.empty;
        }
    }


    /// <summary>
    /// �}�X�̏�Ԃ��m�F����
    /// </summary>
    /// <returns>���݂̃}�X�̏��</returns>
    public CellState CheckState()
    {
        return state;
    }

    /// <summary>
    /// �\��m�F�p(��ɓG�̍s������Ŏg���p)
    /// </summary>
    /// <returns>�\�񂠂�=true, �\��Ȃ�=false</returns>
    public bool CheckReserve() => reserveList.Count > 0;

    /// <summary>
    /// �\�񃊃X�g����ɂ���(�s��1�育�Ƃ̏I�����ɌĂ�)
    /// </summary>
    public void ResetList()
    {
        reserveList.Clear();
    }
}
