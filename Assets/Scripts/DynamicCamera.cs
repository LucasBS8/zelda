using Unity.Cinemachine;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject camB;
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "TriggerCam":
                camB.SetActive(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "TriggerCam":
                camB.SetActive(false);
                break;
        }
    }
}
