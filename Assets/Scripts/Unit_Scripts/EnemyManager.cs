using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyManager
/// - 負責生成敵人 (spawn)
/// - 以 `PathPoint[] pathPoints` 管理路徑節點（每個 point 可設定停等與亂動）
/// - 提供節點保留(Reserve)/釋放(Release)介面以避免多敵人同時佔位
/// 使用說明：在 Inspector 填入 `enemyPrefab`、`spawnPoints`、並建立 `pathPoints` 陣列。
/// </summary>
public class EnemyManager : MonoBehaviour
{
    // singleton 参考
    public static EnemyManager Instance { get; private set; }

    // 內部表示的節點（運行時使用）
    [System.Serializable]
    public class Node
    {
        public Vector2 pos;             // 節點世界座標
        public float radius = 0.5f;     // 節點半徑（用於碰撞/顯示）
        public float waitTime = 1f;     // 停等時間（會傳給 Enemy_move.Waypoint.waitTime）
        public bool Randomove = false;  // 該點停等時是否啟用亂動

        // 運行時管理欄位（不序列化）
        [System.NonSerialized]
        public List<Enemy_move> occupants = new List<Enemy_move>();
        [System.NonSerialized]
        public Queue<Enemy_move> waiters = new Queue<Enemy_move>();
        // 最大同時佔位數（由 PathPoint.maxOccupants 傳入）
        public int maxOccupants = 1;
        // precomputed docking positions (built at path point setup)
        [System.NonSerialized]
        public List<Vector2> dockingPositions = new List<Vector2>();
        // runtime counter for how many docking slots have been assigned so far
        [System.NonSerialized]
        public int assignedCount = 0;
    }

    // Inspector 用的可序列化 PathPoint，允許為每個路徑點單獨設定屬性
    [System.Serializable]
    public class PathPoint
    {
        public Transform WayPointTransform;     // 指向場景中表示路徑點的 Transform
        public float waitTime = 1f;     // 該點的停等時間
        public bool Randomove = false;  // 該點停等是否亂動
        public float radius = 0.5f;     // 該點半徑
        public int maxOccupants = 1;    // 允許同時間停在此點的最大敵人數
    }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;     // 敵人預製體
    public Transform[] spawnPoints;    // 敵人生成點陣列
    [Tooltip("Default number of enemies to spawn per spawn point when auto-spawning or when using SpawnFromAllPoints")]
    public int spawnCountPerPoint = 1;
    [Tooltip("時間間隔：同一生成點每個敵人間的出生延遲 (秒)")]
    public float spawnInterval = 0.15f;
    // spawnSpacing removed: spawning will use spawn point position only
    // Slot spacing for precomputed docking offsets (used internally)
    [Tooltip("Slot lateral spacing used for docking offsets (world units)")]
    public float slotspace = 2f;
    [Tooltip("Enable debug logs for spawn/waypoint assignment")] public bool debugSpawn = false;

    [Header("Path Points")]
    // 只有 PathPoint 管理方式；為每個點可設定停等與亂動
    public PathPoint[] pathPoints;
    public float defaultWaitTime = 1f; // 若 PathPoint 未設定，使用的預設停等
    public bool defaultLoop = true;

    [Header("Node Settings")]
    public float nodeRadius = 0.5f;    // 節點預設半徑

    // 運行時使用的節點清單
    List<Node> nodes = new List<Node>();

    // Ensure nodes list is built (useful if someone calls spawn before Awake/Start finished)
    void BuildNodesIfNeeded()
    {
        if (nodes != null && nodes.Count > 0) return;
        nodes.Clear();
        if (pathPoints != null && pathPoints.Length > 0)
        {
            foreach (var pp in pathPoints)
            {
                if (pp == null || pp.WayPointTransform == null) continue;
                Node n = new Node();
                n.pos = (Vector2)pp.WayPointTransform.position;
                // ensure node radius covers possible shifted docking slots
  
                n.radius = Mathf.Max(pp.radius > 0 ? pp.radius : nodeRadius);
                n.waitTime = pp.waitTime;
                n.Randomove = pp.Randomove;
                n.maxOccupants = Mathf.Max(1, pp.maxOccupants);
                // precompute docking positions: first one is the original node.pos,
                // subsequent positions shift to the right by `slotspace` each
                n.dockingPositions.Clear();
                for (int k = 0; k < n.maxOccupants; k++)
                {
                    n.dockingPositions.Add(n.pos + Vector2.right * (k * slotspace));
                }
                n.assignedCount = 0;
                nodes.Add(n);
            }
            Debug.Log($"EnemyManager: built {nodes.Count} nodes from pathPoints.");
        }
        else
        {
            Debug.LogWarning("EnemyManager: no pathPoints assigned; nodes list will be empty.");
        }
    }

    // Spawn enemies at configured spawn points and assign the same path (nodes)
    // Spawn enemies at every configured spawn point with a specified count
    public void SpawnFromAllPoints(int countPerPoint)
    {
        // Ensure nodes exist before spawning so Waypoints get populated
        BuildNodesIfNeeded();

        if (enemyPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points set in EnemyManager.");
            return;
        }

        foreach (var sp in spawnPoints)
        {
            if (sp == null) continue;
            // start coroutine per spawn point so each point spawns with its own timing
            StartCoroutine(SpawnEnemiesAtPositionRoutine((Vector2)sp.position, countPerPoint));
        }
    }

    // Public: spawn `count` enemies at a Transform spawn point
    // Public: spawn `count` enemies at a world position
    // Coroutine that spawns `count` enemies at `position`. All spawns use the
    // same spawn point (no lateral displacement); docking offsets are provided
    // per-node using `slotspace`.
    private IEnumerator SpawnEnemiesAtPositionRoutine(Vector2 position, int count)
    {
        // Ensure nodes exist before spawning so Waypoints get populated
        BuildNodesIfNeeded();

        if (enemyPrefab == null || count <= 0) yield break;

        // All spawned enemies originate at the same `position` (spawn point)
        int nodeCount = nodes.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = position;

            GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            Enemy_move em = go.GetComponent<Enemy_move>();
            if (em == null)
            {
                em = go.AddComponent<Enemy_move>();
            }

            // assign waypoints based on nodes
            em.Waypoints = new List<Enemy_move.Waypoint>();
            for (int ni = 0; ni < nodeCount; ni++)
            {
                var n = nodes[ni];
                // choose docking position precomputed at node setup
                int idx = Mathf.Min(n.assignedCount, Math.Max(0, n.dockingPositions.Count - 1));
                Vector2 dockPos = n.dockingPositions[idx];
                if (n.waitTime > 0f)
                {
                    // only advance assignedCount if this node uses waitTime (docking behavior)
                    n.assignedCount = Mathf.Min(n.assignedCount + 1, n.dockingPositions.Count - 1);
                }

                // If the docking position is different from the node position, add
                // a docking waypoint first (no reservation on node), then add the
                // original node waypoint so the enemy will later request the node
                // and thus follow/queue behind any leader occupying it.
                if (Vector2.Distance(dockPos, n.pos) > 0.01f)
                {
                    var wpDock = new Enemy_move.Waypoint();
                    // Docking waypoint inherits node's wait/random settings
                    wpDock.waitTime = n.waitTime > 0 ? n.waitTime : defaultWaitTime;
                    wpDock.pos = dockPos;
                    wpDock.Randomove = n.Randomove;
                    em.Waypoints.Add(wpDock);

                    var wpOrig = new Enemy_move.Waypoint();
                    // Appended original node waypoint should not wait or random-move
                    wpOrig.waitTime = 0f;
                    wpOrig.pos = n.pos;
                    wpOrig.Randomove = false;
                    em.Waypoints.Add(wpOrig);
                }
                else
                {
                    var wp = new Enemy_move.Waypoint();
                    wp.waitTime = n.waitTime > 0 ? n.waitTime : defaultWaitTime;
                    wp.pos = dockPos;
                    wp.Randomove = n.Randomove;
                    em.Waypoints.Add(wp);
                }
            }
            em.Loop = defaultLoop;
            em.StartFollowing();

            if (debugSpawn)
            {
                // log assigned waypoints for this enemy
                for (int wi = 0; wi < em.Waypoints.Count; wi++)
                {
                    var w = em.Waypoints[wi];
                    Debug.Log($"[EnemyManager] Spawned {go.name} wp[{wi}] = {w.pos} wait={w.waitTime} rand={w.Randomove}");
                }
            }

            // wait a short interval before spawning the next one so they don't stack
            if (spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
            else yield return null;
        }
    }

    // Find node whose position matches desired using node.radius
    Node FindNode(Vector2 desired)
    {
        foreach (var n in nodes)
        {
            if (Vector2.Distance(n.pos, desired) <= n.radius) return n;
        }
        return null;
    }

    // Coroutine: request to reserve the waypoint. Grants when it's your turn.
    public IEnumerator RequestReserve(Vector2 desiredPos, Enemy_move requester)
    {
        Node node = FindNode(desiredPos);
        if (node == null)
        {
            // No managed node, allow direct access
            requester.ReservedWaypointPos = desiredPos;
            yield break;
        }
        // If this desired position is a shifted docking position (not the node's actual pos),
        // do not enqueue or consume a node slot — just allow the enemy to go there immediately.
        if (Vector2.Distance(desiredPos, node.pos) > 0.01f)
        {
            requester.ReservedWaypointPos = desiredPos;
            yield break;
        }

        // Otherwise, this is a request for the actual node position: enqueue and wait for a slot.
        node.waiters.Enqueue(requester);

        // Wait until you're at the front of the queue and the node has a free slot
        while (node.waiters.Count == 0 || node.waiters.Peek() != requester || node.occupants.Count >= node.maxOccupants)
        {
            yield return null;
        }

        // Dequeue and occupy (reserve a slot)
        if (node.waiters.Count > 0 && node.waiters.Peek() == requester)
            node.waiters.Dequeue();

        node.occupants.Add(requester);

        // Otherwise, recalculate reserved slot positions for all occupants so they are laid out
        // deterministically based on their index in the occupants list.
        float offsetCenter = (node.maxOccupants - 1) * 0.5f;
        for (int i = 0; i < node.occupants.Count; i++)
        {
            var occ = node.occupants[i];
            if (occ == null) continue;
            float xOffset = (i - offsetCenter) * slotspace;
            Vector2 reservedPos = node.pos + new Vector2(xOffset, 0);
            occ.ReservedWaypointPos = reservedPos;
            // if occupant is already at node we snap its transform to reservedPos to avoid overlap
            if (occ.transform != null && Vector2.Distance(occ.transform.position, node.pos) <= node.radius + 0.1f)
            {
                occ.transform.position = reservedPos;
            }
        }
        yield break;
    }

    // Release the node when the enemy leaves the waypoint
    public void ReleaseWaypoint(Vector2 desiredPos, Enemy_move requester)
    {
        Node node = FindNode(desiredPos);
        if (node == null) return;
        if (node.occupants.Contains(requester))
        {
            node.occupants.Remove(requester);
        }
    }

    // (Removed duplicate placeholder ReleaseWaypoint)
}
