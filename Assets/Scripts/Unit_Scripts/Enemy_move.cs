using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_move : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public float waitTime; //在這個WayPoint停留的時間
        public Vector2 pos; //WayPoint座標
        public bool Randomove; //是否開啟亂動
    }
    public List<Waypoint> Waypoints=new List<Waypoint>();
    public bool Loop; // 是否開啟循環
    Vector2[] vectors=new Vector2[]{Vector2.up,Vector2.down,Vector2.left,Vector2.right};
    Ship_class state;
    public float RandomMoveRange; //亂動的範圍
    public float RandomMoveFreq; //亂動的頻率
    void Awake()
    {
        state=GetComponent<Ship_class>();
        if (Waypoints.Count > 0)
        {
            StartCoroutine(FlyFollowWayPoints());
        }
    }
    IEnumerator FlyFollowWayPoints()
    {
        do
        {
            foreach (var item in Waypoints)
            {
                while ((Vector2)transform.position != item.pos)
                {
                    //前往下一個目標點
                    transform.position=Vector2.MoveTowards(transform.position,item.pos,state.shipSpeed*Time.deltaTime);
                    yield return null;
                }

                if (item.Randomove)
                {
                    // 執行原地亂動，直到時間結束
                    yield return StartCoroutine(RandomMoveRoutine(item.waitTime, item.pos));
                }
                else
                {
                    //不動
                    yield return new WaitForSeconds(item.waitTime);
                }
            }
        } while (Loop);
    }
    // 亂動協程
    IEnumerator RandomMoveRoutine(float duration, Vector2 centerPos)
    {
        float timer = 0f;
        float currentMoveSpeed = state.shipSpeed * 0.5f; 

        while (timer < duration)
        {
            //選定一個隨機方向
            Vector2 randomDir = vectors[Random.Range(0, 4)];
            //方向*範圍+原位置=目標位置
            Vector2 targetPos = centerPos + (randomDir * RandomMoveRange);

            //飛向目標點
            while (Vector2.Distance(transform.position, targetPos) > 0.05f)
            {
                // 如果時間已經到了，就提早結束
                if (timer >= duration) break;
                //朝選定方向前進
                transform.position = Vector2.MoveTowards(transform.position,targetPos,
                currentMoveSpeed * Time.deltaTime);
                
                // 累加時間
                timer += Time.deltaTime;
                yield return null;
            }

            //平滑飛回中心點 (Move Back)
            while (Vector2.Distance(transform.position, centerPos) > 0.05f)
            {
                //朝原位置移動
                transform.position = Vector2.MoveTowards(transform.position,centerPos,
                currentMoveSpeed * Time.deltaTime);

                timer += Time.deltaTime;
                yield return null;
            }
            
            // 確保回到中心
            transform.position = centerPos;
            //每次的移動都增加一小段停頓，視覺效果
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }
    }
}
