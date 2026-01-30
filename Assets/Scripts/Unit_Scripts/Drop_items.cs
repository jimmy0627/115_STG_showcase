using System.Collections.Generic;
using UnityEngine;

public class Drop_items : MonoBehaviour
{
    // 定義一個結構，把物品和它的權重綁在一起
    [System.Serializable]
    public class LootItem
    {
        public string name;
        public GameObject itemPrefab;
        [Range(1, 100)] public int weight; // 權重 (數值越高越容易掉)
    }
    public float dropChance = 50f; //全局掉落率

    [SerializeField]
    private List<LootItem> lootTable = new List<LootItem>();

    public void DropLoot() //死亡後調用這條
    {
        // 第一層檢定決定是否要掉落
        if (Random.Range(0f, 100f) > dropChance)
        {
            return; // 運氣不好，什麼都沒掉
        }

        // 開始計算要掉哪一個
        SpawnRandomItem();
    }

    private void SpawnRandomItem()
    {
        if (lootTable.Count == 0) return;

        // 計算所有權重的總和
        int totalWeight = 0;
        foreach (var item in lootTable)
        {
            totalWeight += item.weight;
        }

        // 取一個 0 到 總權重 之間的隨機數
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var item in lootTable)
        {
            currentWeight += item.weight;

            // 如果隨機數落在這個區間內，就選中這個物品
            if (randomValue < currentWeight)
            {
                if (item.itemPrefab != null)
                {
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                }
                return; // 掉落後結束，避免重複掉落
            }
        }
    }
}
