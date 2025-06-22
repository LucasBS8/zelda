using UnityEngine;

public class LockRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.rotation = originalRotation;
    }
}
