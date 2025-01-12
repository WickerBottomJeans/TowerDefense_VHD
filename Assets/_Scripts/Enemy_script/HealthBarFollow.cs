using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Lấy camera chính
    }

    void LateUpdate()
    {
        // Làm thanh máu luôn hướng về camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}
