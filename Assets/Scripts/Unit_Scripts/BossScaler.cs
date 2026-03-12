using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class BossScaler : MonoBehaviour
{
    [Header("成長係數 (每 X 分強化一級)")]
    public float scoreThreshold = 1000f;

    [Header("強化上限")]
    public float maxMultiplier = 10.0f;

    void Start()
    {
        StartCoroutine(ScaleBossRoutine());
    }
    IEnumerator ScaleBossRoutine()
    {
        yield return new WaitForEndOfFrame();

        // 1. 取得分數 
        GameObject sb = GameObject.Find("Score");
        if (sb == null) yield break;

        if (int.TryParse(sb.GetComponent<TextMeshProUGUI>().text, out int score))
        {
            float multiplier = 1f + (score / 1000f);
            multiplier = Mathf.Clamp(multiplier, 1f, 3f);

            Ship_class ship = GetComponent<Ship_class>();
            if (ship != null)
            {
                // 檢查：如果是從 Prefab 讀取的初始血量是 100
                // 我們直接根據倍率重新計算
                int newMaxHealth = Mathf.RoundToInt(ship.MaxHP * multiplier);

                ship.MaxHP = newMaxHealth;
                ship.HP = newMaxHealth; // 順便把血補滿

                Debug.Log($"血量修正成功！原血量：{newMaxHealth / multiplier} -> 新血量：{ship.MaxHP}");
            }
        }

    }
}
