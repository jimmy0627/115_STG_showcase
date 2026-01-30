using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    //變數定義
    private Ship_class state;
    private GameObject bullet;
    void Start()
    {
        state=transform.parent.parent.GetComponent<Ship_class>();
        StartCoroutine(attackRoutine()); //開始攻擊循環
    }
    void Fire()
    {
        bullet=state.bullet;
        GameObject clone=Instantiate(bullet,transform.position,Quaternion.identity); //生成在當前位置的當前船隻下
        clone.GetComponent<Bullet>().damage=state.ATK;
        clone.GetComponent<Bullet>().bulletSpeed=state.bulletSpeed;
        clone.GetComponent<Bullet>().myIFF=state.IFF;
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
