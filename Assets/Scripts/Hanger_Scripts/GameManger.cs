using UnityEngine;

public class GameManger : MonoBehaviour
{
    public int FlySpawnCount=4;
    public GameObject FlySpawnner;
    void Start()
    {
        FlySpawnner.GetComponent<EnemyManager>().SpawnFromAllPoints(FlySpawnCount);
    }

}
