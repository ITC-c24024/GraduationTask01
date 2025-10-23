using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField, Header("ウェーブ数")]
    int waveCount = 0;

    [SerializeField, Header("ターン数")]
    int turnCount = 0;

    [SerializeField, Header("総敵数")]
    int allEnemyCount = 0;

    [SerializeField, Header("プレイヤー数")]
    int playerCount = 0;

    [SerializeField, Header("ウェーブクリア判定")]
    bool isClear = false;

    [SerializeField, Header("ゲームオーバー判定")]
    bool isGameOver = false;

    [SerializeField, Header("ターンマネージャースクリプト")]
    TurnManager turnManagerSC;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
