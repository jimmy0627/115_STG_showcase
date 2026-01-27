using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    //變數定義
    public int damage; //該子彈的傷害
    public int myIFF; //我方敵我識別代碼
    private Rigidbody2D rb; //子彈的剛體
    private Ship_class enmeyShip; //碰撞到的敵方艦船

    void Start()
    {
        //設定變數
        damage=transform.parent.GetComponent<Ship_class>().ATK;
        myIFF=transform.parent.GetComponent<Ship_class>().IFF;
        rb=GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocityY=Math.Abs(transform.parent.GetComponent<Ship_class>().bulletSpeed); //恆定向上
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        try //若敵方為船
        {
            enmeyShip=collision.transform.GetComponent<Ship_class>();
            if (enmeyShip.IFF!=myIFF) //若非我方船艦，造成傷害後刪除
            {
                enmeyShip.Hit(damage);
                Destroy(gameObject);
            }
        }
        catch (System.Exception e) //出bug或不是船艦(可能是敵方子彈或者牆壁)
        {
            Debug.Log("Target is not a ship or bug like"+e);
            Destroy(gameObject);
        }
    }
}
