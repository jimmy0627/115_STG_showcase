using UnityEngine;

public class PlayAreaClamp : MonoBehaviour
{
    public RectTransform boundaryPanel; // 拖入Canvas Panel
    public float optionRadius = 1.5f;   // 浮游砲旋轉的最大半徑(需要加上的半徑)

    void LateUpdate()
    {
        if (boundaryPanel == null) return;

        // 1. 取得 Panel 的世界座標邊界
        Vector3[] corners = new Vector3[4];//這邊是以左下左上右上右下打後續的程式
        boundaryPanel.GetWorldCorners(corners);

        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[2].y;

        // 2. 根據浮游砲半徑，縮減玩家的可動範圍 (Safe Zone)
        float safeMinX = minX + optionRadius;
        float safeMaxX = maxX - optionRadius;
        float safeMinY = minY + optionRadius;
        float safeMaxY = maxY - optionRadius;

        // 3. 限制玩家位置
        Vector3 playerPos = transform.position;
        playerPos.x = Mathf.Clamp(playerPos.x, safeMinX, safeMaxX);
        playerPos.y = Mathf.Clamp(playerPos.y, safeMinY, safeMaxY);

        transform.position = playerPos;
    }
}