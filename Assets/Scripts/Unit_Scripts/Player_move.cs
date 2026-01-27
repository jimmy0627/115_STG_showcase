using UnityEngine;
using UnityEngine.InputSystem;
public class Player_base : MonoBehaviour
{
    public InputAction movement;
    private Rigidbody2D rb;
    private Ship_class state;
    int velocty;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state=GetComponent<Ship_class>();
        movement.Enable();//記得開機，不然movement沒有作用
        velocty=state.shipSpeed;
    }
    void Update()
    {
        //從movement裡讀取輸入數值，並依照方向分類
        Vector2 inputvalue=movement.ReadValue<Vector2>(); 
        float inputx=inputvalue.x;
        float inputy=inputvalue.y;
        Vector2 dir=new Vector2(inputx*velocty,inputy*velocty);
        rb.linearVelocity=dir;
    }

}

