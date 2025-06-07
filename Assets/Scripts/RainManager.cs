using UnityEngine;

public class RainManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]private bool isRain;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.OnRain(isRain);
        }
    }
}
