using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.U2D;

public class PlayerCombat : MonoBehaviour
{
    public SlowMo slowMo;
    public Slider UISlider;
    public Transform arrow;
    public Vector2 dashLimits;
    public float dashSpeed;
    public float fullChargeTime;
    public CameraFollow cam;

    private float chargeRatio = 0.0f;
    private float chargeTimer;
    private bool charging;
    private bool chargingForward;
    private bool dashing;
    private bool slashing;
    private bool fireDown = false;
    private bool fireReleased = false;
    private bool gameEnded = false;

    private PlayerController controller;
    private Animator animator;
    private WeaponManager weaponManager;
    private Entity entity;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        weaponManager = GetComponent<WeaponManager>();
        entity = GetComponent<Entity>();

        charging = false;
        chargingForward = true;
        dashing = false;
        slashing = false;
        chargeTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            Vector2 aimDirection = controller.GetAimDirection();
            arrow.position = new Vector3(transform.position.x + aimDirection.x * 1.5f, transform.position.y + aimDirection.y * 1.5f, 0.0f);
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

            if (charging)
            {
                arrow.gameObject.SetActive(true);
                if (chargingForward)
                {
                    chargeTimer += Time.unscaledDeltaTime;
                    if (chargeTimer >= fullChargeTime)
                    {
                        chargeTimer = fullChargeTime;
                        chargingForward = false;
                    }
                }
                else
                {
                    chargeTimer -= Time.unscaledDeltaTime;
                    if (chargeTimer <= 0.0f)
                    {
                        chargeTimer = 0.0f;
                        chargingForward = true;
                    }
                }
            }
            else
            {
                arrow.gameObject.SetActive(false);
                chargeTimer = 0.0f;
                chargingForward = true;
            }

            chargeRatio = chargeTimer / fullChargeTime;
            UISlider.value = chargeRatio;

            if (fireDown)
            {
                if (!dashing)
                {
                    ChargeAttack();
                    fireDown = false;
                }
            }
            else if (fireReleased)
            {
                ReleaseAttack();
                fireReleased = false;
            }

            Weapon weapon = weaponManager.GetEquipedWeapon();
            if (dashing && !slashing && weapon is MeleeWeapon)
            {
                MeleeWeapon sword = weapon as MeleeWeapon;
                CircleCollider2D slashCollider = sword.slashScan;
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, slashCollider.radius, sword.hitMask);

                foreach (Collider2D colldier in colliders)
                {
                    if (!colldier.gameObject.CompareTag("Player"))
                    {
                        AIAgent enemy = colldier.transform.parent.gameObject.GetComponent<AIAgent>();
                        if (!enemy || !enemy.IsDead())
                        {
                            Vector2 direction = entity.GetVelocity();
                            direction.Normalize();
                            StartCoroutine(Slash(direction));
                            break;
                        }
                    }
                }
            }
        }
    }

    void ChargeAttack()
    {
        //GetComponent<SpriteRenderer>().color = Color.red;
        slowMo.DoSlowMo();
        animator.SetBool("Charge", true);
        charging = true;
    }

    void ReleaseAttack()
    {
        charging = false;
        dashing = true;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        slowMo.ResumeNormalSpeed();
        Vector2 aimDir = controller.GetAimDirection();
        //GetComponent<SpriteRenderer>().color = Color.white;
        float dashDistance = dashLimits.x + chargeRatio * (dashLimits.y - dashLimits.x);
        float dashTime = dashDistance / dashSpeed;
        GetComponent<Entity>().Dash(aimDir * dashSpeed, dashTime);
        float slashTime = dashTime - 0.1f;
        slashTime = Mathf.Max(0.0f, slashTime);
        yield return new WaitForSeconds(slashTime);
        if (!slashing )
        {
            StartCoroutine(Slash(aimDir));
        }
    }

    IEnumerator Slash(Vector2 aimDir)
    {
        slashing = true;
        animator.SetBool("Charge", false);
        animator.SetBool("Slash", true);
        weaponManager.GetEquipedWeapon().Attack(aimDir);
        yield return new WaitForSeconds(weaponManager.GetEquipedWeapon().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - 0.2f);
        animator.SetBool("Slash", false);
        dashing = false;
        slashing = false;
    }

    public void PlayDeathAnimation()
    {
        if (!gameEnded)
        {
            StartCoroutine(OnDead());
        }
    }

    IEnumerator OnDead()
    {
        gameEnded = true;
        Destroy(weaponManager.GetEquipedWeapon().gameObject);
        Destroy(arrow.gameObject);
        slowMo.DoSlowMo();
        cam.GetComponent<CameraFollow>().enabled = true;
        cam.GetComponent<PixelPerfectCamera>().refResolutionX = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionX * 0.5f);
        cam.GetComponent<PixelPerfectCamera>().refResolutionY = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionY * 0.5f);
        animator.SetTrigger("Dead");
        yield return new WaitForSecondsRealtime(1.5f);
        GetComponent<SpriteRenderer>().enabled = false;
        entity.SetVelocity(Vector2.zero);
        yield return new WaitForSecondsRealtime(0.7f);
        //yield return new WaitForSecondsRealtime(100.0f);
        FindObjectOfType<GameManager>().LoseGame();
        Destroy(gameObject);
    }

    public void PlayWinAnimation()
    {
        if (!gameEnded)
        {
            StartCoroutine(OnWin());
        }
    }

    IEnumerator OnWin()
    {
        gameEnded = true;
        slowMo.DoSlowMo(0.3f);
        cam.GetComponent<PixelPerfectCamera>().refResolutionX = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionX * 0.5f);
        cam.GetComponent<PixelPerfectCamera>().refResolutionY = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionY * 0.5f);
        yield return new WaitForSecondsRealtime(2.0f);
        FindObjectOfType<GameManager>().WinGame();
    }

    public void FireInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fireDown = true;
        }

        if (context.canceled)
        {
            fireReleased = true;
            fireDown = false;
        }
    }

    public void CycleInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float cycleDirection = context.ReadValue<float>();
            bool cycleBack = cycleDirection < 0;
            weaponManager.CycleWeapon(cycleBack);
        }
    }

    public bool HasGameEnded()
    {
        return gameEnded;
    }
}
