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
    [SerializeField] private ParticleSystem gemParticle;
    private MeshRenderer meshGem; 
    private SimpleGemsAnim gemAnim;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
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
        meshGem = other.GetComponent<MeshRenderer>();
        gemAnim = other.GetComponent<SimpleGemsAnim>();
        gemAnim.rotationSpeed = 1000;

        for (float i = 0; i <= 3f; i += 0.1f)
        {
            gemAnim.floatHeight = i;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        meshGem.enabled = false;
        var particleInstantiate = Instantiate(gemParticle, other.transform);
        particleInstantiate.Play();
        yield return new WaitForSeconds(1);
        Destroy(other.gameObject);
    }
}
