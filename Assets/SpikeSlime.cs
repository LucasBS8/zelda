using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SpikeSlime : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private Rigidbody rigidBody;
    private GameManager gameManager;
    private NavMeshAgent agent;
    [SerializeField] ParticleSystem groundParticles;
    private Playsound playSound;


    [Header("Variables")]
    [SerializeField] private int HP;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float distanceToEngage;
    [SerializeField] private float distancetoAttack;
    [SerializeField] private float iFramesTime;

    [Header ("Bools")]
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private bool inCombat = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool heavyAttackAvailable = false;
    [SerializeField] private bool isWalking = false;


    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private SkinnedMeshRenderer slimeRenderer;
    [SerializeField] private Transform restingPlace;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        slimeRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        playSound = GetComponent<Playsound>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if( isDead) { return; }

        BattleStatus();


    }

    void BattleStatus()
    {
        
        if (HP <= 5) { heavyAttackAvailable = true; }
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceFromPlayer <= distanceToEngage) { inCombat = true; }
        else if (distanceFromPlayer > distanceToEngage) { inCombat = false; }

        
        if( inCombat && distanceFromPlayer > distancetoAttack)
        {
            WalkToPlayer();
        }
        else if(inCombat && distanceFromPlayer <= distancetoAttack) { Attack(); }
        else if (!inCombat)
        {
            agent.SetDestination(restingPlace.transform.position);
            animator.SetBool("walking", false); 
        }

        isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking);
    }

    private void WalkToPlayer()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.transform.position);
            animator.SetBool("walking", true);
        }
    }

    private void Attack()
    {
        if (!heavyAttackAvailable) 
        { 
            animator.SetTrigger("basicAttack");
            

        }
            else if (heavyAttackAvailable)
            {
            
            animator.SetTrigger("heavyAttack");
            }
    }

    void basicAttackSound()
    {
        playSound.Play(0, 1, 0.9f, 1.1f);
    }
    void heavyAttackSound()
    {
        playSound.Play(1, 1, 0.9f, 1.1f);
    }

    private void EmitGroundParticles()
    {
        groundParticles.Play();
    }

   

   // private void lookatPlayer()
    //{
       // Vector3 lookDirection = (gameManager.player.position - transform.position).normalized;
       // Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
       // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, gameManager.slimeLookAtSpeed * Time.deltaTime);
   // }

    private Coroutine iFrameRoutine;

    private void GetHit(int amount)
    {
        if (isDead || isInvincible)
            return;


        HP -= amount;
        playSound.Play(2);
        isInvincible = true;

        if (iFrameRoutine != null)
            StopCoroutine(iFrameRoutine);

        iFrameRoutine = StartCoroutine(FlashRed());

        if (HP > 0)
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

    private IEnumerator FlashRed()
    {
        float elapsed = 0f;
        float blinkInterval = 0.1f; // tempo entre cada piscar

        isInvincible = true;

        while (elapsed < iFramesTime)
        {
            slimeRenderer.enabled = false; // invisível
            yield return new WaitForSeconds(blinkInterval);

            slimeRenderer.enabled = true;  // visível
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2f;
        }

        // Garante que fique visível no final
        slimeRenderer.enabled = true;
        isInvincible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            GetHit(1); // ou o valor adequado
        }
    }


}
