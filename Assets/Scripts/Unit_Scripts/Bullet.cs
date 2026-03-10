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
    public Vector2 fireDirection; //子彈的發射方向
    private Rigidbody2D rb; //子彈的剛體
    private Ship_class enmeyShip; //碰撞到的敵方艦船

    void Awake()
    {
        //設定變數
        rb=GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        rb.linearVelocityY=Math.Abs(bulletSpeed)*(fireDirection.y); //設定子彈的速度
        rb.linearVelocityX=Math.Abs(bulletSpeed)*(fireDirection.x);
        float angle=Mathf.Atan2(rb.linearVelocityY,rb.linearVelocityX)*Mathf.Rad2Deg; //計算子彈的旋轉角度
        transform.rotation=Quaternion.Euler(0,0,angle-90); //設定子彈的旋轉，使其面向發射方向
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item") || collision.CompareTag("Bullet")) return; //若碰撞到的是物品則跳過
        try
        {
            enmeyShip=collision.transform.GetComponent<Ship_class>();
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
