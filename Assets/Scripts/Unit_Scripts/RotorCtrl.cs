using UnityEngine;

public class RotorCtrl : MonoBehaviour
{
    public float rotationSpeed = 10f; //旋轉速度
    float timer;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); //每幀旋轉一定角度
        if (timer>10)
        {
            rotationSpeed = -rotationSpeed; //反轉旋轉方向
            timer = 0;
            Debug.Log("Rotor direction changed!"); //輸出調試信息
        }
        timer+=Time.deltaTime;
    }
}
