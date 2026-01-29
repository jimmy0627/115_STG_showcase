using UnityEngine;

public class Incrase_ATK : MonoBehaviour
{
    [SerializeField] private int incraseAmmount;
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
                state.IncreaseDamage(incraseAmmount);
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
