using UnityEngine;

public class CameraStartPosition : MonoBehaviour
{
    private void Awake()
    {
        transform.position = new Vector3(-12f, -0.25f, -10f);
    }
}
