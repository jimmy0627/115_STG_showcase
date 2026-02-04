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
    [SerializeField]
    public int gunAmount=1; //彈幕數量
    public int funnelAmount; //浮游砲數量
    [SerializeField]
    public float radius;//浮游砲的半徑
    [SerializeField]
    public float weaponCD; //攻擊冷卻
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
            GameObject f = Instantiate(funnelPrefab,transform.position+new Vector3(0,1.25f,0),quaternion.identity,funnelRoot);
            funnels.Add(f.transform);
            funnelAmount++;
        }
        UpdateFunnel();
    }
    public void Hit(int amount) //受傷
    {
        HP -= amount;
        if (HP <= 0)
        {
            if (IFF==1)
            {
                GetComponent<Drop_items>().DropLoot();
            }
            Destroy(gameObject);
        }
    }
    public void IncreaseAttackspeed(int amount) //減少CD
    {
        float factor=1-(amount/100f);
        weaponCD*=factor;        
    }
    public void UpdateGunActive() //啟用gunAmount數量的機槍
    {
        for (int i = 1; i < Pylons.Count; i++)
        {
            Pylons[i].SetActive(i < gunAmount);
        }
    }
    public void UpdateFunnel() //更新浮游泡位置
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
