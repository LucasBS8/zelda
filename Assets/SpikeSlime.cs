using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class SpikeSlime : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rigidBody;
    private GameManager gameManager;
    private NavMeshAgent agent;

    [Header("Variables")]
    [SerializeField] private int HP;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float distancetoAttack;

    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if( isDead) { return; }
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        if(distanceFromPlayer >= distancetoAttack)
        {
            WalkToPlayer();
        }
        else
        {   
            BasicAttack();
            agent.ResetPath(); 
            animator.SetBool("walking", false); 
        }
       
    }

    private void WalkToPlayer()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.transform.position);
            animator.SetBool("walking", true);
        }
    }

    private void BasicAttack()
    {
        if (distanceFromPlayer < distancetoAttack)
        {
            animator.SetTrigger("basicAttack");
        }
    }

    private void lookatPlayer()
    {
        Vector3 lookDirection = (gameManager.player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, gameManager.slimeLookAtSpeed * Time.deltaTime);
    }

    private void GetHit(int amount)
    {
        if (isDead)
            return;
        HP -= amount;
        if (HP > 0 && !isDead)
        {
            animator.SetTrigger("getHit");
            
        }
        else
        {
            isDead = true;
            animator.SetTrigger("die");
            
        }
    }

    void DestroySelf()
    {

    Destroy(gameObject); 
    
    }

}
