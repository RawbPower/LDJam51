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
    private bool slashScanned = false;

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
        chargingForward = false;
        dashing = false;
        slashing = false;
        chargeTimer = 0.0f;
        fireReleased = false;
        fireDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        weaponManager.GetEquipedWeapon().SetFacingFront(controller.IsFacingForward());

        if (!gameEnded)
        {
            Vector2 aimDirection = controller.GetAimDirection();
            arrow.position = new Vector3(transform.position.x + aimDirection.x * 1.5f, transform.position.y + aimDirection.y * 1.5f, 0.0f);
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

            if (slowMo.IsSlowMo() && !fireDown)
            {
                fireReleased = true;
                slashing = false;
                dashing = false;
            }

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

            if (!slashing && !dashing)
            {
                if (fireDown)
                {
                    if (!dashing)
                    {
                        ChargeAttack();
                        //fireDown = false;
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
                                Debug.Log("Slash Scan");
                                slashScanned = true; ;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    void ChargeAttack()
    {
        //GetComponent<SpriteRenderer>().color = Color.red;
        if (!charging)
        {
            slowMo.DoSlowMo();
            GetComponent<AudioManager>().Play("Unsheath");
            animator.SetBool("Charge", true);
            charging = true;
        }
    }

    void ReleaseAttack()
    {
        charging = false;
        dashing = true;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        slashScanned = false;
        animator.SetBool("Charge", false);
        animator.SetBool("Dash", true);
        slowMo.ResumeNormalSpeed();
        Vector2 aimDir = controller.GetAimDirection();
        //GetComponent<SpriteRenderer>().color = Color.white;
        float dashDistance = dashLimits.x + chargeRatio * (dashLimits.y - dashLimits.x);
        float dashTime = dashDistance / dashSpeed;
        GetComponent<Entity>().Dash(aimDir * dashSpeed, dashTime);
        float slashTime = dashTime - 0.1f;
        slashTime = Mathf.Max(0.0f, slashTime);
        yield return new WaitForSeconds(slashTime);
        if (!slashing && !slashScanned)
        {
            StartCoroutine(Slash(aimDir));
        }
        slashScanned = false;
    }

    IEnumerator Slash(Vector2 aimDir)
    {
        slashing = true;
        GetComponent<AudioManager>().Play("Slash");
        animator.SetBool("Dash", false);
        animator.SetTrigger("Slash");
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90.0f;
        if (controller.IsFacingForward())
        {
            angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90.0f;
        }
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

        weaponManager.GetEquipedWeapon().Attack(aimDir);
        yield return new WaitForSeconds(0.4f);
        transform.rotation = Quaternion.identity;
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
        Debug.Log("Game Ended from death");
        GetComponent<AudioManager>().Play("Hit");
        Destroy(weaponManager.GetEquipedWeapon().gameObject);
        Destroy(arrow.gameObject);
        slowMo.SetCompleted(true);
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
        slowMo.ResumeNormalSpeed();
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
        slowMo.SetCompleted(true);
        gameEnded = true;
        Debug.Log("Game Ended from win");
        slowMo.DoSlowMo(0.3f);
        cam.GetComponent<CameraFollow>().enabled = true;
        cam.GetComponent<PixelPerfectCamera>().refResolutionX = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionX * 0.5f);
        cam.GetComponent<PixelPerfectCamera>().refResolutionY = (int)(cam.GetComponent<PixelPerfectCamera>().refResolutionY * 0.5f);
        yield return new WaitForSecondsRealtime(2.0f);
        FindObjectOfType<GameManager>().WinGame(slowMo.GetTimeLeft());
        slowMo.ResumeNormalSpeed();
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
