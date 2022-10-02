using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class controlling AI of an Entity
public class AIAgent : MonoBehaviour
{
    // Distance from target to start attacking instead of pursuing
    public float attackDistance;
    public float shootingDistance;
    public SpriteRenderer gunSprite;

    // Agro Zones
    public AlertArea alertArea;
    public HotZone hotzone;

    public GameObject bulletPrefab;

    public float bulletForce = 20.0f;

    public float cooldown = 1.0f;
    public float initalShotDelay = 0.0f;

    public ParticleSystem bloodParticle;

    [HideInInspector]
    public float cooldownCounter;

    private bool dead = false;
    private GameObject _target;
    private EnemyStateMachine _enemyFSM;
    private WeaponManager weaponManager;
    private Animator animator;

    [HideInInspector] public PathfindingAgent pathfindingAgent;

    private Entity _entity;

    public void SetTarget(GameObject target) { _target = target; }
    public GameObject GetTarget() { return _target; }

    public Entity GetEntity() { return _entity; }

    // Start is called before the first frame update
    void Start()
    {
        _entity = gameObject.GetComponent<Entity>();
        _enemyFSM = new EnemyStateMachine(this);
        pathfindingAgent = gameObject.GetComponent<PathfindingAgent>();
        weaponManager = GetComponent<WeaponManager>();
        animator = GetComponent<Animator>();

        cooldownCounter = initalShotDelay;
    }

    public void Reset()
    {
        _target = null;
        pathfindingAgent.path = null;
        pathfindingAgent.pathfindCounter = 0.0f;
        hotzone.gameObject.SetActive(false);
        alertArea.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!dead)
        {
            if (cooldownCounter > 0.0f)
            {
                cooldownCounter -= Time.deltaTime;
            }
            else
            {
                cooldownCounter = 0.0f;
            }

            if (GetTarget() != null)
            {
                Vector2 aimDir = GetTarget().transform.position - _entity.transform.position;
                aimDir.Normalize();
                float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
                gunSprite.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            UpdateAgent();
        }
        _entity.Move(Vector2.zero);
    }

    // Update agent AI (can be called in this classes Update or in another one the AI owns)
    public void UpdateAgent()
    {
        _enemyFSM.UpdateFSM();
    }

    public void Attack()
    {
        Vector2 aimDir = GetTarget().transform.position - _entity.transform.position;
        aimDir.Normalize();
        weaponManager.GetEquipedWeapon().Attack(aimDir);
    }

    public void PlayDeathAnimation()
    {
        if (!dead)
        {
            StartCoroutine(OnDead());
        }
    }

    IEnumerator OnDead()
    {
        dead = true;
        Collider2D[] hitboxes = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D hitbox in hitboxes)
        {
            if (hitbox.gameObject.layer == LayerMask.NameToLayer("HitBox"))
            {
                hitbox.gameObject.SetActive(false);
            }
        }
        FindObjectOfType<EnemyManager>().OnEnemyDied();
        animator.SetBool("Hit", true);
        yield return new WaitForSecondsRealtime(0.3f);
        animator.SetBool("Hit", false);
        animator.SetBool("Death", true);
        if (bloodParticle != null)
        {
            Instantiate(bloodParticle.gameObject, transform.position, Quaternion.identity);
        }
        yield return new WaitForSecondsRealtime(0.2f);
        if (bloodParticle != null)
        {
            Instantiate(bloodParticle.gameObject, transform.position, Quaternion.identity);
        }
    }

    public GunWeapon GetGun()
    {
        if (weaponManager.GetEquipedWeapon() is GunWeapon)
        {
            GunWeapon gun = weaponManager.GetEquipedWeapon() as GunWeapon;
            return gun;
        }
        else
        {
            return null;
        }
    }

    public bool IsDead() { return dead; }
}
