using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// EnemyManager
/// - 負責生成敵人 (spawn)
/// - 以 `PathPoint[] pathPoints` 管理路徑節點
/// - 提供節點保留(Reserve)/釋放(Release)介面避免多敵人佔位衝突
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    
    /*
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    [System.Serializable]
    */
    public class Node
    {
        public Vector2 pos;
        public float radius = 0.5f;
        public float waitTime = 1f;
        public bool Randomove = false;
        public int maxOccupants = 1;

        [System.NonSerialized] public List<Enemy_move> occupants = new List<Enemy_move>();
        [System.NonSerialized] public Queue<Enemy_move> waiters = new Queue<Enemy_move>();
        [System.NonSerialized] public List<Vector2> dockingPositions = new List<Vector2>();
        [System.NonSerialized] public int assignedCount = 0;
    }

    [System.Serializable]
    public class PathPoint
    {
        public Transform WayPointTransform;
        public float waitTime = 1f;
        public bool Randomove = false;
        public float radius = 0.5f;
        public int maxOccupants = 1;
    }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public GameObject scoreBoard;
    public Transform[] spawnPoints;
    public int spawnCountPerPoint = 1;
    public float spawnInterval = 0.15f;
    public float slotspace = 2f;
    public bool debugSpawn = false;

    [Header("Path Points")]
    public PathPoint[] pathPoints;
    public float defaultWaitTime = 1f;
    public bool defaultLoop = true;

    [Header("Node Settings")]
    public float nodeRadius = 0.5f;

    private List<Node> nodes = new List<Node>();

    private void BuildNodesIfNeeded()
    {
        if (nodes != null && nodes.Count > 0) return;
        
        // 恢復原本的寫法：不重新 new List，而是清空，避免破壞潛在的記憶體參考
        if (nodes == null) nodes = new List<Node>();
        nodes.Clear(); 

        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogWarning("EnemyManager: no pathPoints assigned.");
            return;
        }

        foreach (var pp in pathPoints)
        {
            if (pp?.WayPointTransform == null) continue;

            var n = new Node
            {
                pos = pp.WayPointTransform.position,
                radius = Mathf.Max(pp.radius > 0 ? pp.radius : nodeRadius),
                waitTime = pp.waitTime,
                Randomove = pp.Randomove,
                maxOccupants = Mathf.Max(1, pp.maxOccupants)
            };

            for (int k = 0; k < n.maxOccupants; k++)
            {
                n.dockingPositions.Add(n.pos + Vector2.right * (k * slotspace));
            }
            
            nodes.Add(n);
        }
    }

    //無參數版本，自動使用 Inspector 中設定的 Spawn Count Per Point
    public void SpawnFromAllPoints()
    {
        SpawnFromAllPoints(spawnCountPerPoint);
    }

    //如果傳入參數，就依參數；若未強制指定，就依 Inspector
//如果傳入參數，就依參數；若未強制指定，就依 Inspector
    public void SpawnFromAllPoints(int countPerPoint)
    {
        BuildNodesIfNeeded();

        // 在每次生成新的一波敵人之前，把所有節點的排隊號碼歸零
        if (nodes != null)
        {
            foreach (var n in nodes)
            {
                n.assignedCount = 0; 
            }
        }

        if (enemyPrefab == null || spawnPoints == null) return;

        // 如果外部呼叫時傳入 -1 (或不合理的值)，就退回使用 Inspector 的設定
        int actualCount = countPerPoint > 0 ? countPerPoint : spawnCountPerPoint;

        foreach (var sp in spawnPoints)
        {
            if (sp != null) StartCoroutine(SpawnEnemiesAtPositionRoutine(sp.position, actualCount));
        }
    }

    private IEnumerator SpawnEnemiesAtPositionRoutine(Vector2 position, int count)
    {
        BuildNodesIfNeeded();
        if (enemyPrefab == null || count <= 0) yield break;

        for (int i = 0; i < count; i++)
        {
            Enemy_move em = SetupEnemy(position);
            em.Waypoints = GenerateWaypoints();
            em.Loop = defaultLoop;
            em.StartFollowing();

            if (debugSpawn) LogDebugWaypoints(em);

            if (spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            else yield return null;
        }
    }

    private Node FindNode(Vector2 desired) => nodes.FirstOrDefault(n => Vector2.Distance(n.pos, desired) <= n.radius);

    public IEnumerator RequestReserve(Vector2 desiredPos, Enemy_move requester)
    {
        Node node = FindNode(desiredPos);

        if (node == null || Vector2.Distance(desiredPos, node.pos) > 0.01f)
        {
            requester.ReservedWaypointPos = desiredPos;
            yield break;
        }

        node.waiters.Enqueue(requester);

        // 【關鍵修正】恢復原本的 while 迴圈保護機制，避免多協程交錯時 Peek() 觸發例外錯誤導致路徑卡死
        while (node.waiters.Count == 0 || node.waiters.Peek() != requester || node.occupants.Count >= node.maxOccupants)
        {
            yield return null;
        }

        // 恢復原本的安全 Dequeue 檢查
        if (node.waiters.Count > 0 && node.waiters.Peek() == requester)
        {
            node.waiters.Dequeue();
        }

        node.occupants.Add(requester);
        UpdateNodeOccupantsPositions(node);
    }

    public void ReleaseWaypoint(Vector2 desiredPos, Enemy_move requester)
    {
        FindNode(desiredPos)?.occupants.Remove(requester);
    }

    // ================= 輔助函式 (Helper Methods) =================

    private Enemy_move SetupEnemy(Vector2 spawnPos)
    {
        GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0, 0, 180));
        if (go.TryGetComponent<Ship_class>(out var ship)) ship.ScoreBorad = scoreBoard;
        if (!go.TryGetComponent<Enemy_move>(out var em)) em = go.AddComponent<Enemy_move>();
        
        return em;
    }

    private List<Enemy_move.Waypoint> GenerateWaypoints()
    {
        var waypoints = new List<Enemy_move.Waypoint>();

        foreach (var n in nodes)
        {
            int idx = Mathf.Clamp(n.assignedCount, 0, n.dockingPositions.Count - 1);
            Vector2 dockPos = n.dockingPositions[idx];

            if (n.waitTime > 0f)
            {
                n.assignedCount = Mathf.Min(n.assignedCount + 1, n.dockingPositions.Count - 1);
            }

            float finalWaitTime = n.waitTime > 0 ? n.waitTime : defaultWaitTime;

            waypoints.Add(new Enemy_move.Waypoint { pos = dockPos, waitTime = finalWaitTime, Randomove = n.Randomove });

            if (Vector2.Distance(dockPos, n.pos) > 0.01f)
            {
                waypoints.Add(new Enemy_move.Waypoint { pos = n.pos, waitTime = 0f, Randomove = false });
            }
        }
        return waypoints;
    }

    private void UpdateNodeOccupantsPositions(Node node)
    {
        float offsetCenter = (node.maxOccupants - 1) * 0.5f;
        for (int i = 0; i < node.occupants.Count; i++)
        {
            var occ = node.occupants[i];
            if (occ == null) continue;

            Vector2 reservedPos = node.pos + new Vector2((i - offsetCenter) * slotspace, 0);
            occ.ReservedWaypointPos = reservedPos;

            if (occ.transform != null && Vector2.Distance(occ.transform.position, node.pos) <= node.radius + 0.1f)
            {
                occ.transform.position = reservedPos;
            }
        }
    }

    private void LogDebugWaypoints(Enemy_move em)
    {
        for (int wi = 0; wi < em.Waypoints.Count; wi++)
        {
            var w = em.Waypoints[wi];
            Debug.Log($"[EnemyManager] Spawned {em.name} wp[{wi}] = {w.pos} wait={w.waitTime} rand={w.Randomove}");
        }
    }

    // ================= 編輯器除錯視覺化 (Optional) =================
    
    private void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            var pp = pathPoints[i];
            if (pp?.WayPointTransform == null) continue;

            Gizmos.color = new Color(0, 1, 1, 0.3f); 
            Gizmos.DrawSphere(pp.WayPointTransform.position, pp.radius > 0 ? pp.radius : nodeRadius);

            if (i > 0 && pathPoints[i - 1]?.WayPointTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(pathPoints[i - 1].WayPointTransform.position, pp.WayPointTransform.position);
            }
        }
    }
}