using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private SpriteRenderer sr;

    [Tooltip("Offset to align sorting with feet")]
    public float yOffset = 0f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-(transform.position.y + yOffset) * 100);
    }
}
