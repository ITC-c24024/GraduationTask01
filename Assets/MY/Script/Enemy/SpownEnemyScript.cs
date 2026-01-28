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

    //現在出現しているゾンビの数
    [SerializeField] int zombieCount = 0;

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
        int nowWave = waveManager.GetNowWave();
        int count;
        if (nowWave < waveDatalist.Count)
        {
            count = GetNowWaveData().spownCount;
        }
        else
        {
            count = nowWave - 3;
        }
         
        return count;
    }

    /// <summary>
    /// スポーンする敵の種類を選ぶ(重み付き抽選)
    /// </summary>
    /// <returns>スポーンする敵</returns>
    public GameObject SelectEnemy()
    {
        int wave = waveManager.GetNowWave();
        WaveData data;
        if (wave < waveDatalist.Count)
        {
            data = waveDatalist[wave - 1];
        }
        else
        {
            data = waveDatalist[waveDatalist.Count - 1];
        }

        GameObject spown = null;

        float total = 0;
        foreach(var enemy in data.enemies)
        {
            total += enemy.percent;
        }
        
        float rand = Random.Range(0, total);
        foreach(var enemy in data.enemies)
        {
            if(rand< enemy.percent)
            {
                spown = enemy.enemy;
                break;
            }            

            rand -= enemy.percent;
        }

        if (spown == null) spown = data.enemies[0].enemy;

        if (spown == enemyPrefab[1])
        {            
            if(zombieCount >= 2)
            {
                //ゾンビが重複しないようにする
                spown = enemyPrefab[0];
            }
            else
            {
                SpownZombie();
            }       
        }

        return spown;
    }

    public void SpownZombie()
    {
        zombieCount++;
    }
    public void DeadZombi()
    {
        zombieCount--;
    }
}
