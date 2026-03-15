using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("生成器綁定")]
    public GameObject FlySpawnner;
    public GameObject MidBossSpawnner;
    public GameObject BossSpawnner;

    [Header("遊戲狀態")]
    public bool isPlayerAlive = true;  // 玩家是否存活
    public int spwanTypeCount=1; // 目前場上允許敵人種類數量
    public float checkInterval = 3f;   // 每隔幾秒檢查一次場上狀態
    public float bossSpawnDelay = 60f; // 遊戲開始後多少秒才允許生成 Boss
    public float midBossSpawnLimitDelay = 30f;     // 遊戲開始後多少秒才允許生成 MidBoss
    public float mutipleEnemySpawnLimitDelay = 45f; // 遊戲開始後多少秒才允許同時生成多種敵人

    void Start()
    {
        // 遊戲開始時，啟動無窮生成迴圈
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // 只要玩家還活著，這個迴圈就會一直執行
        while (isPlayerAlive)
        {
            // 1. 計算場上目前的各種類型敵人數量
            int flyCount = GameObject.FindGameObjectsWithTag("Fly").Length;
            int midBossCount = GameObject.FindGameObjectsWithTag("MidBoss").Length;
            int bossCount = GameObject.FindGameObjectsWithTag("Boss").Length;

            bool hasFly = flyCount > 0;
            bool hasMidBoss = midBossCount > 0;
            bool hasBoss = bossCount > 0;

            // 2. 計算目前有「幾種」敵人在場上
            int activeTypesCount = 0;
            if (hasFly) activeTypesCount++;
            if (hasMidBoss) activeTypesCount++;
            if (hasBoss) activeTypesCount++;
            Debug.Log($"目前場上敵人種類數量: {activeTypesCount} (Fly: {flyCount}, MidBoss: {midBossCount}, Boss: {bossCount})");

            // 3. 判斷是否可以生成新敵人 (條件：場上最多只能有 2 種敵人)
            if (activeTypesCount < spwanTypeCount)
            {
                // 用一個清單來裝「目前允許生成」的敵人種類
                List<string> spawnCandidates = new List<string>();

                // 規則 A：如果場上已經沒有 Fly，且沒有 Boss (互斥)，才允許生成 Fly
                if (!hasFly && !hasBoss) 
                    spawnCandidates.Add("Fly");

                // 規則 B：如果場上已經沒有 MidBoss，且沒有 Boss (互斥)，才允許生成 MidBoss，且必須滿足時間條件
                if (!hasMidBoss && Time.timeSinceLevelLoad >= midBossSpawnLimitDelay) 
                    spawnCandidates.Add("MidBoss");

                // 規則 C：如果場上已經沒有 Boss和midBoss，才允許生成 Boss (互斥)，且必須滿足時間條件
                if (!hasBoss && !hasMidBoss && Time.timeSinceLevelLoad >= bossSpawnDelay) 
                    spawnCandidates.Add("Boss");

                // 4. 從符合條件的清單中，隨機抽籤生成一種敵人
                if (spawnCandidates.Count > 0)
                {
                    string chosenType = spawnCandidates[Random.Range(0, spawnCandidates.Count)];

                    switch (chosenType)
                    {
                        case "Fly":
                            FlySpawnner.GetComponent<EnemyManager>().SpawnFromAllPoints();
                            break;
                        case "MidBoss":
                            MidBossSpawnner.GetComponent<EnemyManager>().SpawnFromAllPoints();
                            break;
                        case "Boss":
                            BossSpawnner.GetComponent<EnemyManager>().SpawnFromAllPoints();
                            break;
                    }
                }
                if (Time.timeSinceLevelLoad >= mutipleEnemySpawnLimitDelay*6)
                {
                    spwanTypeCount = 3; // 遊戲開始後 4 分鐘，允許同時生成 3 種敵人
                }
                else if (Time.timeSinceLevelLoad >= mutipleEnemySpawnLimitDelay)
                {
                    spwanTypeCount = 2; // 遊戲開始後45s，允許同時生成 2 種敵人
                }
            }

            // 休息指定的秒數後，再次進行下一輪檢查
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // 當玩家死亡時，呼叫這個方法來停止生成
    public void GameOver()
    {
        isPlayerAlive = false;
        Debug.Log("玩家死亡，停止生成敵人！");
    }
}
