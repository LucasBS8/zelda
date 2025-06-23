using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector2 sensitivity = new Vector2(4f, 1.5f);
    public float distance = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 8f;
    public float smoothTime = 0.1f;
    public LayerMask obstructionMask;

    float yaw = 0f;
    float pitch = 10f;
    Vector3 currentVelocity;
    Vector3 currentPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0)) // Reativa ao clicar no jogo
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        // INPUT
        yaw += Input.GetAxis("Mouse X") * sensitivity.x;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity.y;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        // DIREÇÃO E POSIÇÃO ALVO
        Vector3 desiredDirection = Quaternion.Euler(pitch, yaw, 0) * Vector3.back;
        Vector3 desiredPosition = target.position + desiredDirection * distance;

        // COLISÃO (para não atravessar parede)
        if (Physics.Raycast(target.position, desiredDirection, out RaycastHit hit, distance, obstructionMask))
        {
            desiredPosition = hit.point;
        }

        // SUAVIZAÇÃO
        currentPosition = Vector3.SmoothDamp(currentPosition, desiredPosition, ref currentVelocity, smoothTime);
        transform.position = currentPosition;

        transform.LookAt(target.position);
    }
}
