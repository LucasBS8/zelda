using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Dialogue/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueLine[] lines;
}
