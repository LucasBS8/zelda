using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private ParticleSystem slash;

    private bool isWalk;
    private float horizontal;
    private float vertical;

    [SerializeField] private bool isDead = false;

    public GameObject cam;

    private GameManager gameManager;

    [Header("Player Configurations")]
    [SerializeField] private int hp = 4;
    [SerializeField] private float movementeSpeed;
    [SerializeField] private bool isAttack;
    [SerializeField] private Transform hitBox;
    [SerializeField] private float hitRange = 0.5f;
    [SerializeField] private Collider[] hitInfo;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private int amountDamage;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        slash = GetComponentInChildren<ParticleSystem>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (gameManager.gameState != GameState.GAMEPLAY)
            return;

       
       

        Inputs();
        MoveCharacter();
        UpdateAnimator();

    }

    private void UpdateAnimator()
    {
        anim.SetBool("isWalk", isWalk);
    }

    private void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new(horizontal, 0f, vertical);
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;

        if (moveDirection.magnitude > 0.1f)
        {
            isWalk = true;
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            isWalk = false;
        }
        controller.Move(movementeSpeed * Time.deltaTime * moveDirection.normalized);
    }

    private void Inputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Fire1") && !isAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        isAttack = true;
        anim.SetTrigger("Attack");
        slash.Play();

        hitInfo = Physics.OverlapSphere(hitBox.position, hitRange, hitMask);

        foreach (Collider col in hitInfo)
        {
            col.gameObject.SendMessage("GetHit", amountDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AttackDone()
    {
        isAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (hitBox != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hitBox.position, hitRange);
        }
    }

    private void GetHit(int amount)
    {   
        if(isDead) { return; }

        hp -= amount;
        if (hp > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            isDead = true;
            anim.SetTrigger("Die");
            gameManager.ChangeGameState(GameState.DIE);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TakeDamage"))
            GetHit(1);
    }
}
