using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private ParticleSystem fxHit;
    private bool isCut;

    public void GetHit(int amount)
    {
        if (!isCut)
        {
            transform.localScale = new Vector3(1, 1, 1);
            fxHit.Emit(5);
            isCut = true;
        }
    }
}

