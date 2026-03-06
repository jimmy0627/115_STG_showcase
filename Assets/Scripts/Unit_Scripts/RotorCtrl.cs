using UnityEngine;

public class RotorCtrl : MonoBehaviour
{
    public float rotationSpeed = 10f; //旋轉速度

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); //每幀旋轉一定角度
    }
}
