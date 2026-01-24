using UnityEngine;

public class Ship_class :MonoBehaviour
{
    [SerializeField]
    protected int HP;
    [SerializeField]
    protected int damageIndex;
    [SerializeField]
    protected int gunAmmount;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int shipType;
    [SerializeField]
    protected int IFF;
    public void repairShip(int ammount)
    {
        HP+=ammount;
    }
    public void addDamage(int ammount)
    {
        damageIndex+=ammount;
    }
    public void AddGun()
    {
        gunAmmount+=1;
    }
}
