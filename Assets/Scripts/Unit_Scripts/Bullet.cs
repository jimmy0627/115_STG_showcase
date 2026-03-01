using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    //變數定義
    public int damage; //該子彈的傷害
    public int myIFF; //我方敵我識別代碼
    public int bulletSpeed=0;
    private Rigidbody2D rb; //子彈的剛體
    private Ship_class enmeyShip; //碰撞到的敵方艦船

    void Start()
    {
        //設定變數
        rb=GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        int factor= (int)Math.Pow(-1,myIFF);
        rb.linearVelocityY=Math.Abs(bulletSpeed)*factor; //若是我方船則向上，敵方向下
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item") || collision.CompareTag("Bullet")) return; //若碰撞到的是物品則跳過
        try
        {
            enmeyShip=collision.transform.GetComponentInParent<Ship_class>();
            if (enmeyShip.IFF!=myIFF) //若非我方船艦，造成傷害後刪除
            {
                enmeyShip.Hit(damage);
                Destroy(gameObject);
            }
        }
        catch (System.NullReferenceException) //出bug或不是船艦
        {
            Destroy(gameObject);
        }
        catch (System.Exception e) //其他bug
        {
            Debug.Log(e);
            Destroy(gameObject);
        }
    }
    public void Self_Destruct()
    {
        transform.GetComponent<SpriteRenderer>().enabled=false; //關閉渲染，避免顯示bug
        Destroy(gameObject);
    }
}
