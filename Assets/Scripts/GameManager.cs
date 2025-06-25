using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public enum EnemyState
{
    IDLE,
    ALERT,
    PATROL,
    FOLLOW,
    DIE,
    FURY
}

public enum GameState
{
    GAMEPLAY,
    DIE
}

public class GameManager : MonoBehaviour
{
    public GameState gameState;

    [Header("Player")]
    public Transform player;
    private int gems;

    [Header("Slime AI")]
    public Transform[] slimeWayPoints;
    public float slimeIdleWaitTime;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay;
    public float slimeLookAtSpeed = 2;

    [Header("Rain manager")]
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("Drop item")]
    public GameObject gemPrefab;
    public int percDrop = 100;

    [Header("Keys and gate")]
    public int keyAmount;
    public GameObject gate;
    public ParticleSystem rainParticle;
    public Volume globalVolume;

    [Header("NightManager")]
    public Volume postB;
    public int nightRateOverTime;
    public int nightIncrement;
    public int nightIncrementDelay;

    [Header("UI")]
    public TMP_Text gemsText;

    public int bossSlimesDefeated;

    private void Awake()
    {
        if (rainParticle != null)
        {
            rainParticle.Play();
        }
        ChangeGameState(GameState.GAMEPLAY);
    }

    public void FinishGame()
    {
        bossSlimesDefeated++;

        if (bossSlimesDefeated >= 3)
        {
            SceneManager.LoadScene("GameCompleted");
        }

    }
    private void Start()
    {
        if (gemsText != null)
        {
            gemsText.text = gems.ToString();
        }
    }

    public void OnRain(bool isRain)
    {
        StopCoroutine(nameof(RainManager));
        StopCoroutine(nameof(PostBManager));
        StartCoroutine(RainManager(isRain));
        StartCoroutine(PostBManager(isRain));
    }

    IEnumerator RainManager(bool isRain)
    {
        var emission = rainParticle.emission;
        switch (isRain)
        {
            case true:

                for (float r = emission.rateOverTime.constant; r <= rainRateOverTime; r += rainIncrement)
                {
                    Debug.Log("Rain Enter: " + r);
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve(r);
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                emission.rateOverTime = new ParticleSystem.MinMaxCurve(rainRateOverTime);
                break;

            case false:
                for (float r = emission.rateOverTime.constant; r >= 0; r -= rainIncrement)
                {
                    Debug.Log("Rain Exit: " + r);
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve(r);
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                emission.rateOverTime = 0;
                break;
        }
    }

    IEnumerator PostBManager(bool isNight)
    {
        switch (isNight)
        {
            case true:
                for (float w = postB.weight; w < 1; w += 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 1;
                break;

            case false:
                for (float w = postB.weight; w > 0; w -= 1 * Time.deltaTime)
                {
                    postB.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                postB.weight = 0;
                break;
        }
    }

    public void ChangeGameState(GameState newState)
    {
        gameState = newState;
    }

    public void SetGems(int amount)
    {
        gems += amount;
        gemsText.text = gems.ToString();
    }


    public bool Perc(int p)
    {
        int temp = Random.Range(0, 100);
        bool ret = temp <= p ? true : false;
        return ret;
    }

    public void OpenGate()
    {
        if (keyAmount >= 2)
        {
            if (rainParticle != null)
            {
                rainParticle.gameObject.SetActive(true);
            }

            if (gate != null)
            {
                gate.SetActive(false);
            }

            // Inicia a corrotina para fazer o fade-in do volume
            StartCoroutine(FadeInVolume(1.0f)); // Duração de 1 segundo para o fade
        }
    }

    private IEnumerator FadeInVolume(float duration)
    {
        // Instancia o novo volume aqui, dentro da corrotina
        Volume newVolume = Instantiate(globalVolume);
        newVolume.weight = 0; // Garante que começa com peso zero

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calcula o progresso (t) de 0 para 1 ao longo da duração
            float t = elapsedTime / duration;

            // Aplica o Lerp com o 't' calculado a cada frame
            newVolume.weight = Mathf.Lerp(0f, 1f, t); // Fade de 0 para 1 (ou o valor que desejar)

            // Incrementa o tempo passado
            elapsedTime += Time.deltaTime;

            // Espera até o próximo frame antes de continuar o loop
            yield return null;
        }

        // Garante que o valor final seja exatamente 1 ao final da corrotina
        newVolume.weight = 1f;

        Debug.Log("Fade-in do Volume completo!");
    }

    public void SceneGameplay()
    {
        SceneManager.LoadScene("game1");
    }

    public void SceneMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GAME OVER");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
