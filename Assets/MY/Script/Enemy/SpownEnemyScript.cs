using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownEnemyScript : MonoBehaviour
{
    WaveManager waveManager;

    [SerializeField, Header("敵Prefab")]
    GameObject[] enemyPrefab;

    [SerializeField, Header("ウェーブデータ")]
    List<WaveData> waveDatalist;

    void Start()
    {
        waveManager = GetComponent<WaveManager>();
    }

    public WaveData GetNowWaveData()
    {
        int nowWave = waveManager.GetNowWave();

        if (nowWave< waveDatalist.Count)
        {
            return waveDatalist[nowWave];
        }

        return waveDatalist[waveDatalist.Count - 1];
    }

    /// <summary>
    /// 1waveでスポーンする敵の数を決める
    /// </summary>
    /// <param name="wave">現在のウェーブ数</param>
    /// <returns>スポーンする数</returns>
    public int GetSpownCount(int wave)
    {
        int count = GetNowWaveData().spownCount;
        return count;
    }

    /// <summary>
    /// スポーンする敵の種類を選ぶ
    /// </summary>
    /// <returns>スポーンする敵</returns>
    public GameObject SelectEnemy()
    {
        int wave = waveManager.GetNowWave();

        if (wave <= 2)
        {
            return enemyPrefab[0];
        }
        else if (wave <= 4)
        {
            return enemyPrefab[1]; 
        }
        else if (wave <= 5)
        {
            if (Probability(50))
            {
                return enemyPrefab[0];
            }
            else
            {
                return enemyPrefab[1];
            }
        }
        else if (wave == 6)
        {
            return enemyPrefab[2];
        }
        else if (wave == 7)
        {
            if (Probability(50))
            {
                return enemyPrefab[0];
            }
            else
            {
                return enemyPrefab[2];
            }
        }
        else
        {
            if (Probability(33.3f))
            {
                return enemyPrefab[0];
            }
            else if (Probability(33.3f))
            {
                return enemyPrefab[1];
            }
            else
            {
                return enemyPrefab[2];
            }
        }

        //エラー
        //return null;
    }

    /// <summary>
    /// 確率計算
    /// </summary>
    /// <param name="percent">確率</param>
    /// <returns></returns>
    bool Probability(float percent)
    {
        float rate = Random.value * 100;

        if (percent == 100 && percent == rate) return true;
        else if (rate < percent) return true;
        else return false;
    }
}
