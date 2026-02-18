using UnityEngine;
using UnityEngine.InputSystem;
public class Player_move : MonoBehaviour
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
        Vector2 dir = inputvalue * velocty;
        rb.linearVelocity = dir;
        float angle = -inputvalue.x * 45;
        Quaternion targetRotation = Quaternion.Euler(0,angle,0);
        // 使用 Slerp 平滑過渡 (從目前旋轉, 到目標旋轉, 根據時間與速度)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, state.rotationSpeed * Time.deltaTime);
        
    }


}

