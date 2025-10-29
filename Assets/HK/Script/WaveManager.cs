using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class WaveManager : MonoBehaviour
{
    [SerializeField, Header("�E�F�[�u��")]
    int waveCount = 1;

    [SerializeField, Header("�^�[����")]
    int turnCount = 0;

    [SerializeField, Header("���G��")]
    int allEnemyCount = 3;

    [SerializeField, Header("���ݏo���G��")]
    int nowEnemyCount = 0;

    [SerializeField, Header("�^�[�����Ƃ̏o���G��")]
    int spawnEnemyCount = 1;

    [SerializeField, Header("�v���C���[��")]
    int playerCount = 0;

    [SerializeField, Header("�E�F�[�u�N���A����")]
    bool isClear = false;

    [SerializeField, Header("�Q�[���I�[�o�[����")]
    bool isGameOver = false;

    [SerializeField, Header("�^�[���}�l�[�W���[�X�N���v�g")]
    TurnManager turnManagerSC;

    void Start()
    {

    }

    void Update()
    {

    }

    void StartTurn()
    {
        turnCount++;

        for (int i = 0; i < spawnEnemyCount + (int)waveCount / 5; i++)
        {
            if (allEnemyCount <= 0 && nowEnemyCount < 10)
            {
                if (turnManagerSC.EnemySpown())
                {
                    nowEnemyCount++;
                }
            }
            else
            {
                break;
            }
        }
        StartCoroutine(turnManagerSC.TurnStart());
    }

    void FinishTurn()
    {
        if (playerCount <= 0)
        {
            GameOver();
            return;
        }
        else if (allEnemyCount <= 0)
        {
            WaveClear();
            return;
        }
        else
        {
            StartTurn();
        }
    }


    void PlayerDead()
    {
        playerCount--;
    }

    void EnemyDead()
    {
        allEnemyCount--;
        nowEnemyCount--;
    }

    void WaveClear()
    {
        isClear = true;
        waveCount++;
        turnCount--;
        allEnemyCount = 3 + 2 * waveCount;

    }

    void GameOver()
    {
        isGameOver = true;
    }
}
