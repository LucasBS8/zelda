using System.Collections; // 1. Adicionar a namespace para Corrotinas
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public DialogueSequence currentSequence;
    private int index = 0;

    [Header("UI References")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI sentenceText;
    /*    public Image portraitImage;*/
    public GameObject dialogueUI;

    [Header("Input Action")]
    public InputActionReference nextLineAction;

    [Header("Dialogue Settings")]
    [Tooltip("A velocidade em que o texto aparece, em segundos por caractere.")]
    public float typingSpeed = 0.04f; // Velocidade do efeito máquina de escrever

    // 2. A nossa "trava". Se for true, o diálogo está ocupado e não aceita input.
    private bool isLinePlaying = false;

    private void OnEnable()
    {
        nextLineAction.action.Enable();
        nextLineAction.action.performed += OnNextLineInput;
    }

    private void OnDisable()
    {
        nextLineAction.action.Disable();
        nextLineAction.action.performed -= OnNextLineInput;
    }

    void Start()
    {
        dialogueUI.SetActive(false);
    }

    // O input agora verifica a trava primeiro.
    private void OnNextLineInput(InputAction.CallbackContext context)
    {
        if (dialogueUI.activeInHierarchy)
        {
            // 3. Se a linha está sendo "digitada", este input será ignorado.
            if (isLinePlaying) return;

            ShowNextLine();
        }
    }

    // O clique do botão também respeita a trava.
    public void OnClickNext()
    {
        // 4. A mesma trava se aplica ao botão da UI.
        if (isLinePlaying) return;

        ShowNextLine();
    }

    public void StartDialogue(DialogueSequence sequence)
    {
        currentSequence = sequence;
        index = 0;
        dialogueUI.SetActive(true);
        ShowNextLine();
    }

    // 5. Renomeamos a função para refletir melhor sua ação.
    public void ShowNextLine()
    {
        if (!isLinePlaying)
        {
            if (index >= currentSequence.lines.Length)
            {
                EndDialogue();
                return;
            }

            // 6. Em vez de mostrar o texto na hora, iniciamos a Corrotina que faz isso ao longo do tempo.
            StartCoroutine(TypeLine());
        }
    }

    // 7. Esta é a Corrotina que cria o efeito e implementa a trava.
    IEnumerator TypeLine()
    {
        isLinePlaying = true; // ATIVA A TRAVA
        int dialogueCount = 0;
        DialogueLine line = currentSequence.lines[index];
        speakerNameText.text = line.speakerName;
        /*        portraitImage.sprite = line.portrait;
        */
        sentenceText.text = ""; // Limpa o texto antes de começar a digitar

        // Loop que "digita" a frase letra por letra
        foreach (char letter in line.sentence.ToCharArray())
        {
            dialogueCount++;
            sentenceText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        index++; // Prepara para a próxima linha
        isLinePlaying = false; // DESATIVA A TRAVA, permitindo o próximo input
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }
}