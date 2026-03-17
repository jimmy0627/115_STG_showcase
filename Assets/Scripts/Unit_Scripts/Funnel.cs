using System.Collections;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Funnel : MonoBehaviour
{
    public float speed = 100f; // 旋轉速度 (度/秒)
    public GameObject state;
    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartCoroutine(attackRoutine());
    }
    void LateUpdate()
    {
        // 確保有父物件，不然會報錯
        if (transform.parent != null)
        {

            transform.RotateAround(transform.parent.position, Vector3.forward, speed * Time.deltaTime);
        }
        transform.rotation = Quaternion.identity;
    }
    public IEnumerator attackRoutine()
    {
        while (true)
        {
            transform.GetComponent<Shooting>().Fire(state);
            yield return new WaitForSeconds(state.GetComponent<Ship_class>().weaponCD);
        }
    }
}
