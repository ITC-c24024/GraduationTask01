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

    [SerializeField, Header("�}�X�̏��")]
    CellState state = CellState.empty;

    [SerializeField, Header("�\��")]
    List<CharaScript> reserveList = new();

    [SerializeField, Header("���݂̃}�X��ɂ��郆�j�b�g�̃X�N���v�g")]
    CharaScript nowUnitSC;

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
    public IEnumerator MoveCheck(CellState newState, CharaScript unitSC)
    {
        yield return new WaitForSeconds(0.2f);

        //�����\�񂪂���ꍇ
        if (reserveList.Count > 1)
        {
            unitSC.ReciveDamage(0);
            yield break;
        }
        //�\�񂪂Ȃ�(���g�̂�)�ꍇ
        else if (reserveList.Count == 1)
        {
            //�}�X�ɑ����j�b�g������ꍇ
            if (nowUnitSC != null)
            {
                // ���肪���̏�ɗ��܂�ꍇ�i�������}�X��_���j
                if (nowUnitSC.nextPos == nowUnitSC.curPos)
                {
                    if (unitSC.rank < nowUnitSC.rank)
                    {
                        unitSC.ReciveDamage(nowUnitSC.damage);
                        yield break;
                    }
                }
                // ����Ⴂ����
                //�����Փ�
                else if (nowUnitSC.nextPos == unitSC.curPos)
                {
                    unitSC.ReciveDamage(0);
                    nowUnitSC.ReciveDamage(0);
                    yield break;
                }
                //�Փ˖���
                else if(nowUnitSC.nextPos != unitSC.curPos)
                {
                    ChangeState(newState, unitSC);
                    yield break;
                }
            }
            //�}�X�ɑ����j�b�g�����Ȃ��ꍇ
            else if (nowUnitSC == null)
            {
                ChangeState(newState, unitSC);
                yield break;
            }
        }
    }


    /// <summary>
    /// �X�e�[�g�؂�ւ�����
    /// </summary>
    /// <param name="newState">�؂�ւ���X�e�[�g�w��</param>
    /// <param name="unitSC">���j�b�g�X�N���v�g</param>
    void ChangeState(CellState newState, CharaScript unitSC)
    {
        if (nowUnitSC != null && nowUnitSC != unitSC)
        {
            nowUnitSC = null;
        }

        state = newState;
        nowUnitSC = unitSC;
    }


    /// <summary>
    /// �e���j�b�g���}�X�𗣂��ۂɌĂԊ֐�
    /// �}�X�̏�Ԃ���ɂ���
    /// </summary>
    public void Leave()
    {
        state = CellState.empty;
        nowUnitSC = null;
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
