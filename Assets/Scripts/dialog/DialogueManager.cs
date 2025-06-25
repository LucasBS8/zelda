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
    public float typingSpeed = 0.04f; // Velocidade do efeito m�quina de escrever

    // 2. A nossa "trava". Se for true, o di�logo est� ocupado e n�o aceita input.
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
            // 3. Se a linha est� sendo "digitada", este input ser� ignorado.
            if (isLinePlaying) return;

            ShowNextLine();
        }
    }

    // O clique do bot�o tamb�m respeita a trava.
    public void OnClickNext()
    {
        // 4. A mesma trava se aplica ao bot�o da UI.
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

    // 5. Renomeamos a fun��o para refletir melhor sua a��o.
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

    // 7. Esta � a Corrotina que cria o efeito e implementa a trava.
    IEnumerator TypeLine()
    {
        isLinePlaying = true; // ATIVA A TRAVA
        int dialogueCount = 0;
        DialogueLine line = currentSequence.lines[index];
        speakerNameText.text = line.speakerName;
        /*        portraitImage.sprite = line.portrait;
        */
        sentenceText.text = ""; // Limpa o texto antes de come�ar a digitar

        // Loop que "digita" a frase letra por letra
        foreach (char letter in line.sentence.ToCharArray())
        {
            dialogueCount++;
            sentenceText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        index++; // Prepara para a pr�xima linha
        isLinePlaying = false; // DESATIVA A TRAVA, permitindo o pr�ximo input
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
    }
}