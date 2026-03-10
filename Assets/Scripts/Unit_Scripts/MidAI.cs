using UnityEngine;


public enum EnemyPhase { Phase1, Phase2 }
public class MidAI : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer enemyMainSprite; // 在 Inspector 裡把子物件拖進來
    public EnemyPhase currentPhase = EnemyPhase.Phase1;
    private Ship_class shipData;
    private bool phase2Triggered = false;

    // 假設這是你原本控制射擊的參數
    public float phase1FireRate = 2.0f;
    public float phase2FireRate = 0.5f; // 進入第二階段射速變快

    void Start()
    {
        shipData = GetComponent<Ship_class>();
    }

    void Update()
    {
        // 每幀檢查血量百分比
        float healthPercent = (float)shipData.HP / shipData.MaxHP;

        if (healthPercent <= 0.5f && !phase2Triggered)
        {
            EnterPhaseTwo();
        }

        // 根據不同階段執行不同行為
        if (currentPhase == EnemyPhase.Phase2)
        {
            ExecutePhase2Attack();
        }
        else
        {
            ExecutePhase1Attack();
        }
    }

    void EnterPhaseTwo()
    {
        phase2Triggered = true;
        currentPhase = EnemyPhase.Phase2;

        // 可以在這裡加一些特效
        Debug.Log("警告：怪物進入第二階段！");
        enemyMainSprite.color = Color.red; // 變紅表示生氣
    }

    void ExecutePhase1Attack()
    {

    }

    void ExecutePhase2Attack()
    {
        shipData.AddGunPair(2);
        shipData.ATK = 20;
    }
}
