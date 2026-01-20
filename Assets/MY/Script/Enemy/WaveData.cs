using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/WaveData")]
public class WaveData : ScriptableObject
{
    public int spownCount;
    public List<EnemyPercent> enemies;
}

[System.Serializable]
public class EnemyPercent
{
    public GameObject enemy;
    [Range(0,100)]
    public float percent;
}
