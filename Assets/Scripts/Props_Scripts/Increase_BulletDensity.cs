using UnityEngine;

public class Increase_BulletDensity : MonoBehaviour
{
    [SerializeField] private int PTPincraseAmount = 2;//
    [SerializeField] private int PPincraseAmount = 1;//
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item") && collision.CompareTag("Bullet")) return;
        try
        {
            Ship_class state=collision.transform.GetComponent<Ship_class>(); //若為船艦且為我方船隻
            if (state.IFF==0)
            {
                if (state.gunAmount < 7)
                {
                    state.AddGunPair(PTPincraseAmount);
                }
                else
                {
                    state.AddFunnel(PPincraseAmount);
                }
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
