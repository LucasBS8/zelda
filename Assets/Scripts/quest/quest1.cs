using System.Collections.Generic;
using System.Linq; // Importante para usar Linq
using TMPro;
using UnityEngine;

public class quest1 : MonoBehaviour
{
    // Popule esta lista no Inspector do Unity com os 3 slimes da quest.
    public List<GameObject> slimes;

    public GameObject cavQuest1;
    public GameObject cavQuest2;
    public TextMeshProUGUI questText;

    private bool questCompleted = false;
    [SerializeField] private int totalSlimes;

    void Start()
    {
        // Garante que o cavaleiro da próxima quest comece desativado.
        if (cavQuest2 != null)
        {
        }

        // Guarda o número total de slimes no início
        totalSlimes = slimes.Count;
        UpdateQuestText();
    }

    void Update()
    {
        // Se a quest já foi completada, não faz mais nada.
        // Isso é uma otimização para não rodar a verificação desnecessariamente.
        if (questCompleted)
        {
            return;
        }

        // Verifica se algum slime na lista foi destruído.
        CheckForDefeatedSlimes();
    }

    private void CheckForDefeatedSlimes()
    {
        // Usando a função Count de Linq para contar quantos slimes ainda são válidos (não nulos).
        // Um GameObject se torna 'null' quando é destruído.
        int slimesRemaining = slimes.Count(s => s != null);

        // Atualiza o texto da UI
        UpdateQuestText(totalSlimes - slimesRemaining);

        // Se nenhum slime restar, a quest está completa.
        if (slimesRemaining == 0)
        {
            CompleteQuest();
        }
    }

    void UpdateQuestText(int slimesDefeated = 0)
    {
        if (questText != null)
        {
            questText.text = $"Slimes derrotados: {slimesDefeated} / {totalSlimes}";
        }
    }

    private void CompleteQuest()
    {
        questCompleted = true;

        if (cavQuest2 != null)
        {
            cavQuest1.SetActive(false);
            cavQuest2.SetActive(true);
        }

        Debug.Log("Quest Completed: Defeat all slimes");

        // Desativa este script para não continuar rodando o Update.
        this.enabled = false;
    }
}