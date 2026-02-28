using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_move : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint //路徑點的class
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
        // Movement is started explicitly via StartFollowing() to allow the manager
        // to assign waypoints before movement begins.
    }
    // 被管理器設定時，Manager 會填好 Waypoints 後呼叫此方法
    public void StartFollowing()
    {
        if (Waypoints.Count > 0)
        {
            StartCoroutine(FlyFollowWayPoints());
        }
    }

    // Manager 在授權路徑點時會設定此欄位為實際可移動的目標位置
    [HideInInspector]
    public Vector2 ReservedWaypointPos;
    IEnumerator FlyFollowWayPoints()
    {
        do
        {
            foreach (var item in Waypoints)
            {
                Vector2 targetPos = item.pos;

                // 向 EnemyManager 申請保留該路徑點（若有管理器）
                if (EnemyManager.Instance != null)
                {
                    // RequestReserve 會把最終可到達的位置放到 this.ReservedWaypointPos
                    yield return StartCoroutine(EnemyManager.Instance.RequestReserve(item.pos, this));
                    targetPos = this.ReservedWaypointPos;
                }

                // Use distance check (avoid exact-equality float compare)
                float moveSpeed = (state != null && state.shipSpeed > 0) ? state.shipSpeed : 2f;
                while (Vector2.Distance(transform.position, targetPos) > 0.01f)
                {
                    //前往下一個目標點（可能是 manager 指派的 reserved 位置）
                    transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                if (item.Randomove)
                {
                    // 執行原地亂動，直到時間結束；以 reserved 位置或原點為中心
                    yield return StartCoroutine(RandomMoveRoutine(item.waitTime, targetPos));
                }
                else
                {
                    //不動
                    yield return new WaitForSeconds(item.waitTime);
                }

                // 完成在此路徑點的停留後，釋放保留（若有管理器）
                if (EnemyManager.Instance != null)
                {
                    EnemyManager.Instance.ReleaseWaypoint(item.pos, this);
                }
            }
        } while (Loop);
    }
    // 亂動協程
    IEnumerator RandomMoveRoutine(float duration, Vector2 centerPos)
    {
        float timer = 0f;
        float currentMoveSpeed = (state != null && state.shipSpeed > 0) ? state.shipSpeed * 0.5f : 1f;

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
