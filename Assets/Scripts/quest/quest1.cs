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
        // Garante que o cavaleiro da pr�xima quest comece desativado.
        if (cavQuest2 != null)
        {
        }

        // Guarda o n�mero total de slimes no in�cio
        totalSlimes = slimes.Count;
        UpdateQuestText();
    }

    void Update()
    {
        // Se a quest j� foi completada, n�o faz mais nada.
        // Isso � uma otimiza��o para n�o rodar a verifica��o desnecessariamente.
        if (questCompleted)
        {
            return;
        }

        // Verifica se algum slime na lista foi destru�do.
        CheckForDefeatedSlimes();
    }

    private void CheckForDefeatedSlimes()
    {
        // Usando a fun��o Count de Linq para contar quantos slimes ainda s�o v�lidos (n�o nulos).
        // Um GameObject se torna 'null' quando � destru�do.
        int slimesRemaining = slimes.Count(s => s != null);

        // Atualiza o texto da UI
        UpdateQuestText(totalSlimes - slimesRemaining);

        // Se nenhum slime restar, a quest est� completa.
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

        // Desativa este script para n�o continuar rodando o Update.
        this.enabled = false;
    }
}