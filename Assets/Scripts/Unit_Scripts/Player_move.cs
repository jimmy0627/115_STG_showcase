using UnityEngine;
using UnityEngine.InputSystem;
public class Player_move : Ship_class
{
    public InputAction movement;
    private Rigidbody2D rb;
    int velocty;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement.Enable();//記得開機，不然movement沒有作用
        velocty=speed;
    }
    void Update()
    {
        //從movement裡讀取輸入數值，並依照方向分類
        Vector2 inputvalue=movement.ReadValue<Vector2>(); 
        float inputx=inputvalue.x;
        float inputy=inputvalue.y;
        //以下可以使用movebygetaxis中的三種方法任一實現，這裡用linearVelocity演示
        Vector2 dir=new Vector2(inputx*velocty,inputy*velocty);
        rb.linearVelocity=dir;
    }

}

