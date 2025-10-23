using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;

    [SerializeField, Header("�}�X�v���n�u")]
    GameObject cellPrefab;

    CellScript[,] cellSC;

    void Start()
    {
        cellSC = new CellScript[width, height];//�z�񏉊���

        //�}�b�v�������e�}�X�X�N���v�g�擾
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cellObj = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity);
                cellSC[y, x] = cellObj.GetComponent<CellScript>();
                cellSC[y, x].SetPosition(new Vector2Int(y, x));
                cellSC[y, x].gridManagerSC = this;
            }
        }
    }

    /// <summary>
    /// �w�肵���}�X�Ɉړ��\�������
    /// </summary>
    /// <param name="y">y���W</param>
    /// <param name="x">x���W</param>
    /// <param name="unitSC">�Ăт������̃��j�b�g�X�N���v�g</param>
    public void ReserveCell(int y, int x, CharaScript unitSC)
    {
        cellSC[y, x].ReserveState(unitSC);
    }

    /// <summary>
    /// �w�肵���}�X�̏�Ԃ�ύX����
    /// </summary>
    /// <param name="y">y���W(�c�̗�)</param>
    /// <param name="x">x���W(���̗�)</param>
    /// <param name="state">���(empty=��,player=�v���C���[,enemy=�G,damageTile=�_���[�W��)</param>
    /// <param name="unitSC">�Ăт������̃��j�b�g�X�N���v�g</param>
    /// <param name="direction">�G���g���̕���</param>
    public CellScript.TryEnterResult ChangeCellState(int y, int x, CellScript.CellState state, CharaScript unitSC, Vector2Int direction)
    {
        return cellSC[y, x].TryEnter(state, unitSC,direction);
    }

    /// <summary>
    /// �w�肵���}�X�̏�Ԃ��m�F����
    /// </summary>
    /// <param name="y">y���W(�c�̗�)</param>
    /// <param name="x">x���W(���̗�)</param>
    /// <returns>���݂̃}�X�̏��</returns>
    public CellScript.CellState CheckCellState(int y, int x)
    {
        return cellSC[y, x].CheckState();
    }

    /// <summary>
    /// �w�肵���}�X�̗\��̗L�����m�F����
    /// </summary>
    /// <param name="y">y���W</param>
    /// <param name="x">x���W</param>
    /// <returns>�\�񂠂�=true, �\��Ȃ�=false</returns>
    public bool CheckCellReserve(int y, int x) => cellSC[y, x].CheckReserve();

    /// <summary>
    /// ���ׂẴ}�X�̗\������Z�b�g(1�s�����Ƃ̏I�����ɌĂ�)
    /// </summary>
    public void ResetReserveListAll()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cellSC[y, x].ResetList();
            }
        }
    }

    /// <summary>
    /// �w�肵���}�X���痣���
    /// </summary>
    /// <param name="y">y���W</param>
    /// <param name="x">x���W</param>
    /// <param name="unitSC">���j�b�g�̃X�N���v�g</param>
    public void LeaveCell(int y, int x, CharaScript unitSC)
    {
        cellSC[y, x].Leave(unitSC);
    }

    /// <summary>
    /// �w�肵���}�X�ɑ΂��čU��
    /// </summary>
    /// <param name="y">y���W</param>
    /// <param name="x">x���W</param>
    /// <param name="damage">�_���[�W��</param>
    public void SendDamage(int y,int x, int damage)
    {
        cellSC[y, x].ReciveAttack(damage);
    }
}
