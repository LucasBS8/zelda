using UnityEngine;

public enum EnemyState
{
    IDLE,
    ALERT,
    PATROL,
    FOLLOW,
    FURY
}
public class GameManager : MonoBehaviour
{
    public Transform player;

    [Header("Slime AI")]
    public Transform[] slimeWayPoints;
    public float slimeIdleWaitTime;
    public float slimeDistanceToAttack = 2.3f;
    public float slimeAlertTime = 3f;
    public float slimeAttackDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
