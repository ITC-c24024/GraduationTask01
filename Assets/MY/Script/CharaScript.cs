using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaScript : MonoBehaviour
{
    public TurnManager turnManager;
    public GridManager gridManager;
    public CellScript cellScript;

    //�ڐG�D��x
    public int rank;

    //HP
    public int hp = 100;
    //�U����
    public int damage = 1;

    //�s���\��
    public int actionLimit = 1;
    //�ړ��@��
    public Direction[] moveRule;

    //���݈ʒu
    public Vector3 curPos;
    //���ɐi�ޗ\��ʒu
    public Vector3 nextPos;

    /// <summary>
    /// �i�߂邩�̔���
    /// </summary>
    /// <param name="x">�i�݂����}�X��x���W</param>
    /// <param name="z">�i�݂����}�X��z���W</param>
    /// <returns></returns>
    public bool CanMove(Vector3 targetPos)
    {
        //�X�e�[�W�͈͓������ׂ�
        float posX = targetPos.x;
        float posZ = targetPos.z;
        if (posX < 0 || 7 < posX) return false;
        else if (posZ < 0 || 7 < posZ) return false;
        else return true;
    }

    /// <summary>
    /// �������v�Z
    /// </summary>
    /// <param name="targetPos">�ڕW�ʒu</param>
    /// <param name="startPos">�X�^�[�g�ʒu</param>
    /// <returns>���ʂ̋�����Ԃ�</returns>
    float GetDistance(Vector3 targetPos, Vector3 startPos)
    {
        float distance = Mathf.Sqrt(
            Mathf.Pow(targetPos.x - startPos.x, 2) + Mathf.Pow(targetPos.z - startPos.z, 2)
            );
        return distance;
    }

    /// <summary>
    /// �v���C���[�̈ʒu�A�����𒲂ׁA�i�ޕ��������߂�
    /// </summary>
    public Vector2 GetDirection(Vector3[] playerPos, Vector3 startPos, int i)
    {
        //�v���C���[�Ƃ̋������v�Z
        float diff_A = GetDistance(playerPos[0], startPos);
        float diff_B = GetDistance(playerPos[1], startPos);

        //�߂�����ǂ��ʒu�Ƃ���
        Vector3 targetPos;
        if (diff_A <= diff_B)
        {
            targetPos = playerPos[0];
        }
        else
        {
            targetPos = playerPos[1];
        }

        //x���W�Az���W�̍����v�Z
        float diffX = targetPos.x - startPos.x;
        float diffZ = targetPos.z - startPos.z;

        //��藣��Ă鎲�ɐi��
        if (Mathf.Abs(diffX) <= Mathf.Abs(diffZ))
        {
            return new Vector2(moveRule[i].x * Mathf.Sign(diffX), moveRule[i].z * Mathf.Sign(diffZ));
        }
        else
        {
            return new Vector2(moveRule[i].z * Mathf.Sign(diffX), moveRule[i].x * Mathf.Sign(diffZ));
        }
    }

    /// <summary>
    /// �G�L�������ړ�
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Move()
    {
        yield return null;
    }

    /// <summary>
    /// ��e����
    /// </summary>
    /// <param name="amount">��_���[�W</param>
    public virtual void ReciveDamage(int amount, Vector2 kbDir = default)
    {
        
    }
}

[Serializable]
public class Direction
{
    public int x;
    public int z;
}
