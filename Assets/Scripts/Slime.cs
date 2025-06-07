using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    [SerializeField] private int hp = 3;
    private Animator anim;
    private GameManager gameManager;
    private bool isDie;
    [SerializeField] private EnemyState state;

    //AI
    private NavMeshAgent agent;
    private Vector3 destination;
    private int idWayPoint;
    [SerializeField] private bool isPlayerVisible;

    private bool isWalk;
    private bool isAlert;
    public bool isAttack;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        ChangeState(state);
    }

    void Update()
    {
        if (isDie)
            return;
        StateManager();

        if (agent.desiredVelocity.magnitude > 0.1f)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }
        anim.SetBool("isWalk", isWalk);
        anim.SetBool("isAlert", isAlert);
    }

    private void GetHit(int amount)
    {
        if (isDie)
            return;
        hp -= amount;
        if (hp > 0 && !isDie)
        {
            anim.SetTrigger("getHit");
            ChangeState(EnemyState.FURY);
        }
        else
        {
            isDie = true;
            anim.SetTrigger("die");
            StartCoroutine(Died());
        }
    }

    private void StateManager()
    {
        if (gameManager.gameState == GameState.DIE && (state == EnemyState.FURY || state == EnemyState.ALERT))
            ChangeState(EnemyState.IDLE);

        switch (state)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.ALERT:
                break;
            case EnemyState.PATROL:
                break;

            case EnemyState.FOLLOW:
                destination = gameManager.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }
                else
                {
                    StartCoroutine(Idle());
                }
                break;

            case EnemyState.FURY:
                destination = gameManager.player.position;
                agent.destination = destination;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Attack();
                }
                break;
        }
    }
    private void ChangeState(EnemyState newState)
    {
        StopAllCoroutines();
        isAlert = false;

        switch (newState)
        {
            case EnemyState.IDLE:
                agent.stoppingDistance = 0;
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine(Idle());
                break;

            case EnemyState.PATROL:
                agent.stoppingDistance = 0;
                idWayPoint = Random.Range(0, gameManager.slimeWayPoints.Length);
                destination = gameManager.slimeWayPoints[idWayPoint].position;
                agent.destination = destination;
                StartCoroutine(Patrol());
                break;

            case EnemyState.FURY:
                agent.stoppingDistance = gameManager.slimeDistanceToAttack;
                destination = transform.position;
                agent.destination = destination;
                StartCoroutine(Fury());
                break;

            case EnemyState.FOLLOW:
                agent.stoppingDistance = gameManager.slimeDistanceToAttack;
                break;

            case EnemyState.ALERT:
                agent.stoppingDistance = gameManager.slimeDistanceToAttack;
                destination = transform.position;
                agent.destination = destination;
                isAlert = true;
                StartCoroutine(Alert());
                break;

            case EnemyState.DIE:
                destination = transform.position;
                agent.destination = destination;
                break;
        }
        state = newState;
    }

    private int Rand()
    {
        int rand = Random.Range(0, 100);
        return rand;
    }

    //Corrotinas
    private IEnumerator Died()
    {
        if (isDie)
        {
            yield return new WaitUntil(() => agent.remainingDistance <= 0);
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(gameManager.slimeIdleWaitTime);
        StayStill(50);
    }

    private IEnumerator Patrol()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0);
        StayStill(30);
        ChangeState(EnemyState.IDLE);
    }

    private IEnumerator Fury()
    {
        yield return new WaitForSeconds(gameManager.slimeAlertTime);
        if (isPlayerVisible)
        {
            ChangeState(EnemyState.FURY);
        }
        else
        {
            StayStill(10);
        }
    }

    private IEnumerator Alert()
    {
        if (gameManager.gameState != GameState.GAMEPLAY)
        {
            Idle();
        }
        yield return new WaitForSeconds(gameManager.slimeAlertTime);
        if (isPlayerVisible)
        {
            ChangeState(EnemyState.FOLLOW);
        }
        else
        {
            StayStill(10);
        }
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(gameManager.slimeAttackDelay);
        isAttack = false;
    }

    private void StayStill(int stayThreshold)
    {
        if (Rand() <= stayThreshold)
        {
            ChangeState(EnemyState.IDLE);
        }
        else
        {
            ChangeState(EnemyState.PATROL);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (state == EnemyState.PATROL || state == EnemyState.IDLE)
            {
                isPlayerVisible = true;
                ChangeState(EnemyState.ALERT);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAlert = false;
            isAttack = false;
            isPlayerVisible = false;
        }
    }

    private void Attack()
    {
        if (gameManager.gameState != GameState.GAMEPLAY)
            return;
        if (!isAttack)
        {
            isAttack = true;
            anim.SetTrigger("attack");
        }

    }
    private void AttackIsDone() => StartCoroutine(AttackRoutine());

    private void LookToAt()
    {
        Vector3 lookDirection = (gameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, gameManager.slimeLookAtSpeed * Time.deltaTime);
    }
}
