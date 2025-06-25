using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public DialogueSequence sequence;
    public DialogueManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.StartDialogue(sequence);
        }
        gameObject.SetActive(false);
    }
}
