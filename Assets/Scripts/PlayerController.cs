using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Vector3 direction;
    private ParticleSystem slash;

    private bool isWalk;
    private float horizontal;
    private float vertical;

    [Header("Player Configurations")]
    [SerializeField] private float movementeSpeed;
    [SerializeField] private bool isAttack;
    [SerializeField] private Transform hitBox;
    [SerializeField] private float hitRange = 0.5f;
    [SerializeField] private Collider[] hitInfo;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private int amountDamage;

    void Start()
    {
        slash = GetComponentInChildren<ParticleSystem>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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
        direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude > 0.1)
        {
            isWalk = true;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        else
        {
            isWalk = false;
        }

        controller.Move(direction * movementeSpeed * Time.deltaTime);
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
}
