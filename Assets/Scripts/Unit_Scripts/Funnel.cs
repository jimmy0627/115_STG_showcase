using UnityEngine;

public class Funnel : MonoBehaviour
{
    public float speed = 100f; // 旋轉速度 (度/秒)

    void LateUpdate()
    {
        // 確保有父物件，不然會報錯
        if (transform.parent != null)
        {

            transform.RotateAround(transform.parent.position, Vector3.forward, speed * Time.deltaTime);
        }
        transform.rotation = Quaternion.identity;
    }
}
