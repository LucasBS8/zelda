using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("NightManager")]
    public Volume postB;
    public int nightRateOverTime;
    public int nightIncrement;
    public int nightIncrementDelay;

    private void Awake()
    {
        rainParticle.Play();
        ChangeGameState(GameState.GAMEPLAY);
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
    }
}
