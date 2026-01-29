using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Health_Pack : MonoBehaviour
{
    [SerializeField] private int recoverAmmount;
    private Rigidbody2D rb;

    void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Bullet>())
        {
            return;
        }
        try
        {
            Ship_class state=collision.transform.GetComponent<Ship_class>(); //若為船艦且為我方船隻
            if (state.IFF==0)
            {
                state.RepairShip(recoverAmmount);
                Destroy(gameObject);
            }
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
}
