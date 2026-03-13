using UnityEngine;


public enum EnemyPhase { Phase1, Phase2 }
public class MidAI : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer enemyMainSprite; // 在 Inspector 裡把子物件拖進來
    public EnemyPhase currentPhase = EnemyPhase.Phase1;
    private Ship_class shipData;

    // 假設這是你原本控制射擊的參數
    private float phase1FireRate ;
    private float phase2FireRate ;
    private bool phase2Flag; // 確保只進入一次第二階段
    private int phase1DMG;
    private int phase2DMG;

    void Start()
    {
        //第一階段射速就是原本的射速,第二階段射速是第一階段的4倍
        // 第一階段傷害就是原本的攻擊力,第二階段傷害是第一階段的2倍
        shipData = GetComponent<Ship_class>();
        phase1FireRate = shipData.weaponCD; 
        phase2FireRate = shipData.weaponCD * 0.25f; 
        phase2Flag = false;
        phase1DMG = shipData.ATK; 
        phase2DMG = shipData.ATK * 2; 

    }

    void Update()
    {
        // 每幀檢查血量百分比
        float healthPercent = (float)shipData.HP / shipData.MaxHP;

        if (healthPercent <= 0.5f)
        {
            EnterPhaseTwo();
        }
    }

    void EnterPhaseTwo()
    {
        currentPhase = EnemyPhase.Phase2;
        enemyMainSprite.color = Color.red; // 變紅表示生氣
        if (!phase2Flag)
        {
            phase2Flag = true;
            ExecutePhase2Attack(); // 立即執行第二階段攻擊，讓玩家感受到變化
        }
    }


    void ExecutePhase2Attack()
    {
        shipData.AddGunPair(2);
        shipData.ATK = phase2DMG;
    }
}
