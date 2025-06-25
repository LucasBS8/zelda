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
    public ParticleSystem rainParticle;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("Drop item")]
    public GameObject gemPrefab;
    public int percDrop = 100;

    [Header("Keys and gate")]
    public int keyAmount;
    public GameObject gate;

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
            gate.SetActive(false);
        }
    }

    public void SceneGameplay()
    {
        SceneManager.LoadScene("cena antiga");
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
