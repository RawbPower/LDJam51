using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class controlling AI of an Entity
public class AIAgent : MonoBehaviour
{
    // Distance from target to start attacking instead of pursuing
    public float attackDistance;
    public float shootingDistance;

    // Agro Zones
    public AlertArea alertArea;
    public HotZone hotzone;

    public GameObject bulletPrefab;

    public float bulletForce = 20.0f;

    public float cooldown = 1.0f;
    public float initalShotDelay = 0.0f;

    [HideInInspector]
    public float cooldownCounter;

    private GameObject _target;
    private EnemyStateMachine _enemyFSM;
    private WeaponManager weaponManager;

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
        if (cooldownCounter > 0.0f)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        UpdateAgent();
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
}
