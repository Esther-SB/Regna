using UnityEngine;

[ExecuteAlways]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D targetRb;

    [Header("Offsets")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float horizontalLookAhead = 2f;
    [SerializeField] private float lookAheadDamp = 8f;

    [Header("Seguimiento")]
    [SerializeField] private float smoothSpeed = 6f;
    [SerializeField] private float hardCatchupDistanceX = 3.5f;
    [SerializeField] private float hardCatchupDistanceY = 2.5f;

    [Header("Límites del mundo (opcional)")]
    [SerializeField] private bool clampToWorld = false;
    [SerializeField] private Rect worldBounds = new Rect(-50, -10, 100, 30);

    [Header("Pixel-Perfect")]
    [SerializeField] private bool pixelSnap = true;
    [Tooltip("PPU real de tus sprites (16, 32, 48, 100…).")]
    [SerializeField] private int pixelsPerUnit = 16;

    private Camera cam;
    private float lastFacing = 1f;
    private float currentLookAheadX = 0f;

    private void Reset()
    {
        cam = GetComponent<Camera>() ?? Camera.main;
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
            targetRb = p ? p.GetComponent<Rigidbody2D>() : null;
        }
    }

    private void Awake()
    {
        cam = GetComponent<Camera>() ?? Camera.main;
        if (Application.isPlaying && targetRb != null)
        {
            // Interpolate para suavizar entre FixedUpdate y frame (reduce jitter del target)
            targetRb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1) Dirección horizontal
        float dirX = 0f;
        if (targetRb)
        {
            dirX = Mathf.Sign(targetRb.velocity.x);
            if (Mathf.Abs(targetRb.velocity.x) < 0.01f) dirX = 0f;
        }
        else
        {
            dirX = Mathf.Sign(Mathf.Abs(target.localScale.x) < 0.0001f ? lastFacing : target.localScale.x);
        }
        if (dirX != 0f) lastFacing = dirX;

        // 2) Look-ahead suavizado
        float targetLookAheadX = horizontalLookAhead * lastFacing;
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAheadX, lookAheadDamp * Time.deltaTime);

        // 3) Posición deseada
        Vector3 desired = target.position + baseOffset + new Vector3(currentLookAheadX, 0f, 0f);

        // 4) Suavizado y catch-up
        Vector3 pos = transform.position;
        float dx = desired.x - pos.x;
        float dy = desired.y - pos.y;
        bool snapX = Mathf.Abs(dx) > hardCatchupDistanceX;
        bool snapY = Mathf.Abs(dy) > hardCatchupDistanceY;

        float t = Mathf.Clamp01(smoothSpeed * Time.deltaTime);
        float newX = snapX ? desired.x : Mathf.Lerp(pos.x, desired.x, t);
        float newY = snapY ? desired.y : Mathf.Lerp(pos.y, desired.y, t);
        float newZ = baseOffset.z != 0 ? baseOffset.z : (cam ? -10f : pos.z);

        Vector3 newPos = new Vector3(newX, newY, newZ);

        // 5) Clamp mundo
        if (clampToWorld && cam && cam.orthographic)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            float minX = worldBounds.xMin + halfW;
            float maxX = worldBounds.xMax - halfW;
            float minY = worldBounds.yMin + halfH;
            float maxY = worldBounds.yMax - halfH;
            if (minX > maxX) { float cx = (worldBounds.xMin + worldBounds.xMax) * 0.5f; minX = maxX = cx; }
            if (minY > maxY) { float cy = (worldBounds.yMin + worldBounds.yMax) * 0.5f; minY = maxY = cy; }
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        // 6) SNAP a rejilla de píxeles (clave para quitar blur/flicker)
        if (pixelSnap && pixelsPerUnit > 0)
            newPos = SnapToPixelGrid(newPos, pixelsPerUnit);

        transform.position = newPos;
    }

    private static Vector3 SnapToPixelGrid(Vector3 worldPos, int ppu)
    {
        float unitsPerPixel = 1f / ppu;               // tamaño de un píxel en unidades de mundo
        worldPos.x = Mathf.Round(worldPos.x / unitsPerPixel) * unitsPerPixel;
        worldPos.y = Mathf.Round(worldPos.y / unitsPerPixel) * unitsPerPixel;
        // Z no se toca
        return worldPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Vector3 basePoint = target.position + baseOffset;
        Vector3 rightPoint = basePoint + Vector3.right * Mathf.Abs(horizontalLookAhead);
        Vector3 leftPoint = basePoint - Vector3.right * Mathf.Abs(horizontalLookAhead);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(target.position, basePoint);
        Gizmos.DrawWireCube(rightPoint, Vector3.one);
        Gizmos.DrawWireCube(leftPoint, Vector3.one);
        if (clampToWorld)
        {
            Gizmos.color = Color.yellow;
            Vector3 bl = new Vector3(worldBounds.xMin, worldBounds.yMin, 0f);
            Vector3 br = new Vector3(worldBounds.xMax, worldBounds.yMin, 0f);
            Vector3 tr = new Vector3(worldBounds.xMax, worldBounds.yMax, 0f);
            Vector3 tl = new Vector3(worldBounds.xMin, worldBounds.yMax, 0f);
            Gizmos.DrawLine(bl, br); Gizmos.DrawLine(br, tr); Gizmos.DrawLine(tr, tl); Gizmos.DrawLine(tl, bl);
        }
    }
}
