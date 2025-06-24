using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private GameManager gameManager;
    private NavMeshAgent agent;
    public Transform[] WayPoints;
    private Playsound playSound;
    private AudioSource audioSource;


    [Header("Variables")]
    [SerializeField] private int hp = 3;
    [SerializeField] private int idWayPoint;
    private Vector3 destination;
    [SerializeField] private EnemyState state;
    [SerializeField] private float distanceToAttack;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float distanceToAlert;
    [SerializeField] private float timeToEngage;
    [SerializeField] private float alertTimer;
    [SerializeField] private float patrolTimer;
    [SerializeField] private float switchWaypointTimer;
    [SerializeField] private float minRandToDrop;


    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> dropsList = new List<GameObject>();

    [Header("Bools")]
    private bool isWalk;
    private bool isAlert;
    public bool isAttack;
    [SerializeField] private bool isPlayerVisible;
    private bool isDead;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        playSound = GetComponent<Playsound>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        agent.isStopped = false;
    }

    void Update()
    {
        if (isDead)
            return;

        isWalk= agent.velocity.magnitude > 0.1f;
         animator.SetBool("isWalk", isWalk);
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceFromPlayer <= distanceToAlert)
        {
            isAlert = true;
            animator.SetBool("isAlert", true);
            LookAtPlayer();
            alertTimer += Time.deltaTime;
            if (alertTimer >= timeToEngage)
            {
                WalkToPlayer(); alertTimer = 0;
            }

        }
        else if (distanceFromPlayer > distanceToAlert)
        {
            patrolTimer += Time.deltaTime;
            if(patrolTimer >= switchWaypointTimer)
            {
                StartPatrol();
            }
               
           
        }



        if (isAlert && distanceFromPlayer <= distanceToAttack) 
        {
            isWalk = false;
            animator.SetTrigger("attack");
        }
    }

    private bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            // Compara se a distância até o destino é muito pequena (ex: 0.1 unidades)
            return agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
        return false;
    }



    private void WalkToPlayer()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.transform.position);
           isWalk = true;
           
        }
    }

    private void StartPatrol()
    {
        if (WayPoints.Length == 0 || agent == null) return;

        int randIndex;
        do
        {
            randIndex = Random.Range(0, WayPoints.Length);
        } while (randIndex == idWayPoint && WayPoints.Length > 1);

        idWayPoint = randIndex;
        destination = WayPoints[idWayPoint].position;
        agent.SetDestination(destination);
        isWalk = true;
        patrolTimer = 0;

        switchWaypointTimer = Random.Range(4f, 7f);

       
    }

    private void Drop()
    {
        int willDrop = Random.Range(0, 100);
        if (willDrop > minRandToDrop && dropsList.Count > 0)
        {
            int randomIndex = Random.Range(0, dropsList.Count);
            GameObject drop = dropsList[randomIndex];
            Instantiate(drop, transform.position + Vector3.up, Quaternion.identity);
        }
    }
    private void GetHit(int amount)
    {
        if (isDead)
            return;

        playSound.Play(1, 1, 0.9f, 1.1f);
        hp -= amount;
        if (hp > 0 && !isDead)
        {
            
            Debug.Log("som chamado");
            animator.SetTrigger("getHit");
           
        }
        else
        {

            playSound.Play(2, 1, 0.9f, 1.1f);
            isDead = true;
            animator.SetTrigger("die");
            Drop();
           
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

  

  private void Attack()
    {
        
        if (!isAttack)
        {
            isAttack = true;
            animator.SetTrigger("attack");
            
        }

    }

    void AttackSound()
    {
        playSound.Play(0, 1, 0.9f, 1.1f);
    }

    void AttackIsDone()
    {
        isAttack = false;
    }
    

    private void LookAtPlayer()
    {
        Vector3 lookDirection = (gameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, gameManager.slimeLookAtSpeed * Time.deltaTime);
    }
}
