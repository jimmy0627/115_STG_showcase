using UnityEngine;
using UnityEngine.UI;

public class UnitHpBar : MonoBehaviour
{
    [SerializeField] private Color StartColor = Color.green;
    [SerializeField] private Color EndColor = Color.red;
    [SerializeField] private Image HPImage;
    public float HP;
    public float nowHP;
    void Start()
    {
        HP = transform.parent.GetComponent<Ship_class>().HP;
        nowHP = transform.parent.GetComponent<Ship_class>().nowHP;
    }

    public void SetHPBar(float HP, float NowHP)
    {
        if (HPImage == null) return;

        float ratio = Mathf.Clamp01(NowHP / HP);
        HPImage.fillAmount = ratio;
        HPImage.color = Color.Lerp(EndColor, StartColor, ratio);
    }
    void Update()
    {
        if (transform.parent == null) return;
        Ship_class ship = transform.parent.GetComponent<Ship_class>();
        if (ship == null) return;

        SetHPBar(ship.HP, ship.nowHP);
    }
}
