using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    //變數定義
    private Ship_class state;
    private GameObject bullet;
    public void Fire(GameObject ship)
    {
        state=ship.GetComponent<Ship_class>();
        bullet=state.bullet;
        GameObject clone=Instantiate(bullet,transform.position,Quaternion.identity); //生成在當前位置的當前船隻下
        clone.GetComponent<Bullet>().damage=state.ATK;
        clone.GetComponent<Bullet>().bulletSpeed=state.bulletSpeed;
        clone.GetComponent<Bullet>().myIFF=state.IFF;
        clone.GetComponent<Bullet>().fireDirection=transform.up; //設定子彈的發射方向為當前物件的上方
    }
}
