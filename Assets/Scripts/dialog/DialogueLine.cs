using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "Dialogue/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    public string speakerName;
    [TextArea(3, 10)]
    public string sentence;
    public Sprite portrait;
}
