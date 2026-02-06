using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Clear_AllBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int clearTime; //消除的效果可以持續多久

    void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item") || collision.CompareTag("Bullet")) return;
        try
        {
            Ship_class state=collision.transform.GetComponent<Ship_class>(); //若為船艦且為我方船隻
            StartCoroutine(ClearBulletsRoutine());
        }
        catch (System.NullReferenceException) //撞牆
        {
            Destroy(gameObject);
        }
        catch (System.Exception e) //其他bug
        {
            Debug.Log(e);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        rb.linearVelocityY=-1;
    }
    IEnumerator ClearBulletsRoutine()
    {
        // 關閉圖片渲染
        if (GetComponent<SpriteRenderer>()) 
            GetComponent<SpriteRenderer>().enabled = false;
        
        // 關閉碰撞器
        if (GetComponent<Collider2D>()) 
            GetComponent<Collider2D>().enabled = false;

        float timer = 0f;
        while (timer <= clearTime)
        {
            // 搜尋場上所有子彈
            GameObject[] results = GameObject.FindGameObjectsWithTag("Bullet");
            
            foreach (var item in results)
            {
                // 檢查是否為敵方子彈
                Bullet bulletScript = item.GetComponent<Bullet>();
                if (bulletScript != null && bulletScript.myIFF == 1)
                {
                    bulletScript.Self_Destruct();
                }
            }

            // 累積時間
            timer += Time.deltaTime;
            yield return null; 
        }

        Destroy(gameObject);
    }

}
