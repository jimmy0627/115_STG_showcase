using System;
using Unity.Mathematics;
using UnityEngine;

public class Ship_class :MonoBehaviour
{
    [SerializeField]
    public int MaxHP;       //最大血量（只會用在HPbar運算
    [SerializeField]
    public int HP;      //現在的血量也就是傷害運算用這個
    [SerializeField]
    public int ATK; //攻擊力
    [SerializeField]
    public int gunAmmount; //彈幕數量
    [SerializeField]
    public int weaponCD; //攻擊冷卻
    [SerializeField]
    public int shipSpeed; //移動速度
    [SerializeField]
    public int bulletSpeed; //子彈速度
    [SerializeField]
    public int shipType; //艦船類型
    [SerializeField]
    public int IFF; //敵我標示(我為0,敵為1)
    [SerializeField] 
    public GameObject bullet; //子彈的object
    public void RepairShip(int ammount) //回血
    {
        HP += ammount;
        nowHP=math.min(HP,nowHP+ammount);
    }
    public void IncreaseDamage(int ammount) //加攻擊力
    {
        ATK+=ammount;
    }
    public void AddGun() //加彈幕
    {
        gunAmmount+=1;
    }
    public void Hit(int ammount) //受傷
    {
        nowHP-=ammount;
        if (nowHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void IncreaseAttackspeed(int ammount)
    {
        weaponCD+=ammount;
    }
}
