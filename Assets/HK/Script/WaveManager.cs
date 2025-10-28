using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField, Header("�E�F�[�u��")]
    int waveCount = 0;

    [SerializeField, Header("�^�[����")]
    int turnCount = 0;

    [SerializeField, Header("���G��")]
    int allEnemyCount = 3;

    [SerializeField, Header("���ݏo���G��")]
    int nowEnemyCount = 0;

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

    void WaveStart()
    {
        if(allEnemyCount <= 0 && nowEnemyCount < 10)
        {
            //turnManagerSC.EnemySpown();
        }
        //StartCoroutine(turnManagerSC.TurnStart());
    }

    void TurnFinish()
    {
        if (playerCount <= 0)
        {
            GameOver();
            return;
        }
        else if (allEnemyCount <= 0)
        {
            WaveClear();
        }
    }


    void PlayerDead()
    {
        playerCount--;
        if (playerCount <= 0)
        {
            GameOver();
        }
    }

    void EnemyDead()
    {
        allEnemyCount--;
        nowEnemyCount--;
    }

    void WaveClear()
    {
        isClear = true;
    }

    void GameOver()
    {
        isGameOver = true;
    }
}
