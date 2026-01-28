using UnityEngine;
using UnityEngine.UI;

public class UnitHpBar : MonoBehaviour
{
    [SerializeField] private Color StartColor = Color.green; //滿血的顏色
    [SerializeField] private Color EndColor = Color.red;//缺血的顏色
    [SerializeField] private Image HPImage;//用canva下面的image來填滿（顏色沒差）
    public float MaxHP;
    public float HP;    //現在的血量也就是傷害運算用這個
    void Start()
    {
        MaxHP = transform.parent.GetComponent<Ship_class>().MaxHP;
        HP = transform.parent.GetComponent<Ship_class>().HP;
    }

    public void SetHPBar(float MaxHP, float HP)
    {
        if (HPImage == null) return;

        float ratio = Mathf.Clamp01(HP / MaxHP);        //計算比例，Clamp01這東西會限制裡面的值0~1這樣後面才不會出錯
        HPImage.fillAmount = ratio;                     //用比例填滿
        HPImage.color = Color.Lerp(EndColor, StartColor, ratio);    // EndColor+(StartColor-EndColor)*ratio
    }
    void Update()
{
    if (transform.parent == null) return;
    Ship_class ship = transform.parent.GetComponent<Ship_class>();
    if (ship == null) return;

    SetHPBar(ship.MaxHP, ship.HP);      //隨時更新血量
}
}
