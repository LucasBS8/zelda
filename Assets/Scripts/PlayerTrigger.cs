using Benjathemaker;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Cameras")]
    [SerializeField] private GameObject camB;
    private GameObject player;

    [Header("Gem")]
   
    private MeshRenderer meshGem; 
    private SimpleGemsAnim gemAnim;
    private Playsound playSound;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playSound = GetComponent<Playsound>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "TriggerCam":
                camB.SetActive(true);
                break;
            case "Collectable":
                StartCoroutine(ColetableGem(other));
                break;
            case "Key":
                gameManager.keyAmount++;
                gameManager.OpenGate();
                if (gameManager.keyAmount >= 2)
                {
                    playSound.Play(4);
                }
                Destroy(other.gameObject, 0.6f);
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

    IEnumerator ColetableGem(Collider other)
    {
        gameManager.SetGems(10);
        other.enabled=false;
        
        yield return new WaitForSeconds(1);
        Destroy(other.gameObject);
    }
}
