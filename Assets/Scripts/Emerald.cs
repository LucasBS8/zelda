using UnityEngine;
using System.Collections;

public class Emerald : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public ParticleSystem emeraldParticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case ("Player"):
                StartCoroutine(EmitEmeraldParticle());
                break;
           


        }

    }
    IEnumerator EmitEmeraldParticle()
    {
        yield return null;
        meshRenderer.enabled = false;
        emeraldParticle.Emit(10);
    }

}
