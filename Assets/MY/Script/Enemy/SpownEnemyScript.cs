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

        if (nowWave < waveDatalist.Count) 
        {
            return waveDatalist[nowWave - 1];
        }

        return waveDatalist[waveDatalist.Count - 1];
    }

    /// <summary>
    /// 1waveでスポーンする敵の数を決める
    /// </summary>
    /// <returns>スポーンする数</returns>
    public int GetSpownCount()
    {
        int count = GetNowWaveData().spownCount;
        return count;
    }

    /// <summary>
    /// スポーンする敵の種類を選ぶ(重み付き抽選)
    /// </summary>
    /// <returns>スポーンする敵</returns>
    public GameObject SelectEnemy()
    {
        int wave = waveManager.GetNowWave();
        WaveData data = waveDatalist[wave - 1];

        float total = 0;
        foreach(var enemy in data.enemies)
        {
            total += enemy.percent;
        }

        float rand = Random.Range(0, total);
        foreach(var enemy in data.enemies)
        {
            if(rand< enemy.percent)
                return enemy.enemy;

            rand -= enemy.percent;
        }
        return data.enemies[0].enemy;
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
