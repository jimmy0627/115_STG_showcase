using UnityEngine;

public class Summon : MonoBehaviour
{
    public GameObject spawner; //生成器物件
    public int spawnTieme; //生成時間
    void Awake()
    {
        spawner.GetComponent<EnemyManager>().SpawnFromAllPoints();
        InvokeRepeating("Spawn", spawnTieme, spawnTieme); //每隔spawnTime秒調用Spawn方法
    }
    void Spawn()
    {
        spawner.GetComponent<EnemyManager>().SpawnFromAllPoints();
    }
}
