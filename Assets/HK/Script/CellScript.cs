using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

    //�O���b�h�}�l�[�W���[�X�N���v�g
    public GridManager gridManagerSC;

    [SerializeField, Header("�}�X�̍��W")]
    Vector2Int cellPos = new();

    [SerializeField, Header("�}�X�̏��")]
    CellState state = CellState.empty;

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
    /// �}�X�̍��W���擾����
    /// </summary>
    /// <returns>���W</returns>
    public Vector2Int GetPosition()
    {
        return cellPos;
    }

    /// <summary>
    /// �\�ł���΃}�X�̏�Ԃ�ς���
    /// </summary>
    /// <param name="newState">���(empty=��, player=�v���C���[, enemy=�G)</param>
    /// <param name="unitSC">�Ăт������̃��j�b�g�X�N���v�g</param>
    /// <param name="direction">�G���g������</param>
    public TryEnterResult TryEnter(CellState newState, CharaScript unitSC, Vector2Int direction)
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
                //�G�X�e�[�g��������m�b�N�o�b�N
                if (state == CellState.enemy)
                {
                    foreach (var enemy in enemyList)
                    {
                        result.damage += enemy.damage;
                        result.knockbackDir = new Vector2Int(-direction.x, -direction.y);
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
                int x = cellPos.x + direction.y;
                int y = cellPos.y + direction.x;
                int damage = unitSC.damage;

                // �m�b�N�o�b�N�����̃}�X���}�b�v�����`�F�b�N
                bool inBounds = x >= 0 && x < gridManagerSC.width &&
                                y >= 0 && y < gridManagerSC.height;

                if (inBounds)
                {
                    var targetCell = gridManagerSC.CheckCellState(x, y);

                    // ���̃}�X���G�łȂ���΃m�b�N�o�b�N�\
                    if (targetCell != CellState.enemy)
                    {
                        result.canMove = true;
                    }
                }
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
    /// �v���C���[�U���̎󂯕t���p
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    public void ReciveAttack(int damage, bool isEnemy, Vector2Int direction)//���o���Xbool�Ԃ����ƂɂȂ邩��
    {
        if (!isEnemy && enemyList.Count > 0)
        {
            foreach (var enemy in enemyList)
            {
                enemy.ReciveDamage(damage, new Vector2(0, 0));
            }
        }
        else if (isEnemy && playerList.Count > 0)
        {
            foreach (var player in playerList)
            {
                player.ReciveDamage(damage, direction);
            }
        }
    }
}
