using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    //變數定義
    private Ship_class state;
    private GameObject bullet;
    void Start()
    {
        state=GetComponent<Ship_class>();
        StartCoroutine(attackRoutine()); //開始攻擊循環
    }
    void Fire()
    {
        bullet=state.bullet;
        Instantiate(bullet,transform.position,Quaternion.identity,transform); //生成在當前位置的當前船隻下
    }

    IEnumerator attackRoutine()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(state.weaponCD);
        }
    }
}
