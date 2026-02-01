using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Ship_class : MonoBehaviour
{
    [SerializeField]
    public int MaxHP;       //最大血量（只會用在HPbar運算
    [SerializeField]
    public int HP;      //現在的血量也就是傷害運算用這個
    [SerializeField]
    public int ATK; //攻擊力
    static public int gunAmount = 1; //彈幕數量
    public int funnelAmount= 0; //浮游砲數量
    [SerializeField]
    public float radius = 5f;//浮游砲的半徑
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
    [SerializeField] List<GameObject> Pylons = new List<GameObject>();//機槍的list
    [SerializeField] Transform funnelRoot;//浮游砲要放的父物件
    [SerializeField] GameObject funnelPrefab;//浮游砲的prefab


    List<Transform> funnels = new List<Transform>();//浮游砲的list

    public void RepairShip(int amount) //回血
    {
        HP += amount;
        HP = math.min(MaxHP, HP + amount);
    }
    public void IncreaseDamage(int amount) //加攻擊力
    {
        ATK += amount;
    }
    public void AddGunPair(int amount) //加彈幕
    {
        gunAmount += amount;
        UpdateGunActive();
    }
    public void AddFunnel(int amount)//加浮游砲
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject f = Instantiate(funnelPrefab, funnelRoot);
            //Instantiate這函數能管旋轉角度，我沒寫它，等於就是直接用物體本身的角度了
            //沒研究過Instantiate，到時候套上圖片如果要用角度再說
            funnels.Add(f.transform);
            funnelAmount++;
        }

        UpdateFunnel();//計算角度以及位置，然後把砲的相對位置算出來
    }
    public void Hit(int amount) //受傷
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void IncreaseAttackspeed(int amount)
    {
        weaponCD += amount;
    }
    public void UpdateGunActive()//
    {
        for (int i = 0; i < Pylons.Count; i++)
        {
            Pylons[i].SetActive(i < gunAmount);
        }
    }
    public void UpdateFunnel()
    {
        int count = funnels.Count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * 360f / count;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 localPos = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            ) * radius;

            funnels[i].localPosition = localPos;
        }
    }
}
