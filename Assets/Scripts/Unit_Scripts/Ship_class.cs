using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    public int gunAmount = 1; //彈幕數量
    public int funnelAmount= 0; //浮游砲數量
    [SerializeField]
    public float radius = 5f;//浮游砲的半徑
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
    [SerializeField] private GameObject ScoreBorad; //分數板

    [SerializeField] PlayAreaClamp playAreaClamp;
    [SerializeField] List<GameObject> Pylons = new List<GameObject>();//機槍的list
    [SerializeField] Transform funnelRoot;//浮游砲要放的父物件
    [SerializeField] GameObject funnelPrefab;//浮游砲的prefab
    int[] Scores=new int[]{0,100,500,1000}; //擊毀艦船的分數，依照ShipType排序

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
            AddScore(shipType,ScoreBorad);
        }
    }
    public void IncreaseAttackspeed(float amount)
    {
        float temp=weaponCD;
        weaponCD -= temp*(amount/100);
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
        //用極座標的方式算出一個浮游砲應該要在哪個位置
        //因為我們是設定等角速度，所以只要每次生成檢查一次浮游砲們的相對位置就行
        for (int i = 0; i < count; i++)//分別算每個浮游的for迴圈
        {
            float angle = i * 360f / count;     //算極座標的角度 1個就是360，2個180\360，三個120\240\360
            float rad = angle * Mathf.Deg2Rad;//角度轉徑渡

            Vector3 localPos = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            ) * radius;//極座標表示位置，x是cos，y是sin，然後z是0

            funnels[i].localPosition = localPos;//把各個浮游砲給位置
        }
    }
    private void AddScore(int shipType , GameObject ScoreBoard) //增加分數
    {

        TextMeshProUGUI borad = ScoreBoard.GetComponent<TextMeshProUGUI>();
        int origin=int.Parse(borad.text);
        if (Scores[shipType] != -1)
        {
            origin+=Scores[shipType];
            borad.text=origin.ToString();
        }
        else
        {
            GetComponent<Mange_scenes>().Change_Scenes("Settlement_scene");
        }
    }
}
