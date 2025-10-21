using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaScript : MonoBehaviour
{
    public TurnManager turnManager;

    //�ڐG�D��x
    public int rank;

    //HP
    public int hp = 100;

    //�ړ��\��
    public int moveLimit = 1;
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
    /// �v���C���[�̈ʒu�𒲂ׁA�i�ޕ��������߂�
    /// </summary>
    public Vector2 GetDirection(Vector3 playerPos, Vector3 startPos, int i)
    {
        float diffX = playerPos.x - startPos.x;
        float diffZ = playerPos.z - startPos.z;

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
}

[Serializable]
public class Direction
{
    public int x;
    public int z;
}
