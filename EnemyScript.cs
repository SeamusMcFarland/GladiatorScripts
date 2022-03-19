
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyScript : MonoBehaviour
{

    public int enemyType;
    float health;
    bool dead;
    public PhysicMaterial bouncyDeathPM;

    GameObject player;
    PlayerScript playerS;

    float xDiff, zDiff;
    float rotation;

    BloodEffectScript beS;

    Rigidbody rb;
    //float maxStep;
    float targetDistance;
    float movementTimer;
    float maxMovementTimer;
    float speed;
    int movementState; // 1 for attacking charge, 2 for circling left, 3 for circling right, 4 for backstep
    float chargeSpeed;

    float attackTimer;

    ArmScript armS;
    float maxAttackTime;
    bool attacking;

    ShieldScript shieldS;

    Collider[] allColliders;

    public BloodyPuddleScript[] bpS;
    AudioControllerScript acS;

    const float BLEED_RATE = 0.005f;

    float frameRegulator;

    GameObject[] enemies;
    GameObject target;
    const float MINIMUM_TARGET_SWITCH = 1f;

    public GameObject enemyMarker;

    ScoreCanvasScript scS;

    AudioSource eSource;

    // Start is called before the first frame update
    void Start()
    {
        eSource = GetComponentInChildren<AudioSource>();
        scS = GameObject.FindGameObjectWithTag("ScoreCanvas").GetComponent<ScoreCanvasScript>();
        acS = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioControllerScript>();
        allColliders = GetComponentsInChildren<Collider>();
        maxAttackTime = 1.5f;
        armS = GetComponentInChildren<ArmScript>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("EnemyLocator");
        playerS = player.GetComponent<PlayerScript>();
        beS = GameObject.FindGameObjectWithTag("BloodEffect").GetComponent<BloodEffectScript>();
        //maxStep = 300f;
        target = player;

        switch (enemyType)
        {
            case 0:
                health = 11f;
                maxAttackTime = 1.9f;
                speed = 4f;
                chargeSpeed = 5f;
                maxMovementTimer = 0.6f;
                shieldS = GetComponentInChildren<ShieldScript>();
                break;
            case 1:
                health = 8f;
                speed = 4f;
                chargeSpeed = 5f;
                maxMovementTimer = 0.5f;
                maxAttackTime = 1.3f;
                break;
            case 2:
                xDiff = Random.Range(-1f, 1f);
                zDiff = Random.Range(-1f, 1f);
                health = 5f;
                speed = 2f;
                chargeSpeed = 2f;
                maxMovementTimer = 0.2f;
                maxAttackTime = 0.7f;
                break;
            case 3:
                health = 10f;
                speed = 4f;
                chargeSpeed = 5f;
                maxMovementTimer = 0.5f;
                maxAttackTime = 1.5f;
                shieldS = GetComponentInChildren<ShieldScript>();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameRegulator = Time.deltaTime / 0.015f;
        if (!dead && !playerS.GetDead())
        {
            PointTowardsTarget();
            if (movementTimer < 0)
                PickMovement();
            else
                movementTimer -= Time.deltaTime;
            Movement();

            CheckAttackBlock();
            if (!armS.transform.gameObject.activeInHierarchy || (shieldS && !shieldS.transform.gameObject.activeInHierarchy))
                Wound(BLEED_RATE * frameRegulator);

            SetTarget();
        }
    }

    private void CheckAttackBlock()
    {
        if (attackTimer < 0)
        {
            if (enemyType == 0 || enemyType == 3)
            {
                if (Random.value < 0.6f)
                    Attack();
                else if (Random.value < 0.8f)
                    Block();
            }
            else
            {
                if (Random.value < 0.92f)
                    Attack();
            }
        }
        else
            attackTimer -= Time.deltaTime;
    }

    private void PointTowardsTarget()
    {
        if (enemyType == 2)
        {
            rotation = (Mathf.Rad2Deg * Mathf.Atan2(xDiff, zDiff));
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
        else
        {
            xDiff = target.transform.position.x - transform.position.x;
            zDiff = target.transform.position.z - transform.position.z;
            rotation = (Mathf.Rad2Deg * Mathf.Atan2(xDiff, zDiff));
            transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    private void PickMovement() // 1 for attacking charge, 2 for circling left, 3 for circling right, 4 for backstep
    {
        movementTimer = maxMovementTimer;
        if (enemyType == 2)
        {
            xDiff = Random.Range(-1f, 1f);
            zDiff = Random.Range(-1f, 1f);
        }
        else
        {
            if (Random.value < 0.6f) // checks if will move
            {
                if (Random.value < 0.8f) // pick a circling direction
                {
                    if (Random.value < 0.5f)
                        movementState = 2;
                    else
                        movementState = 3;
                }
                else if (Random.value < 0.5f)
                    movementState = 1;
                else
                    movementState = 4;
            }
            else
                movementState = 0;
        }
    }

    private void Movement()
    {

        if (enemyType == 2)
        {
            targetDistance = Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(zDiff, 2));
            rb.velocity = new Vector3(speed * xDiff / targetDistance, rb.velocity.y, speed * zDiff / targetDistance);
        }
        else
        {
            targetDistance = Vector2.Distance(new Vector2(target.transform.position.x, target.transform.position.z), new Vector2(transform.position.x, transform.position.z));
            if (targetDistance > 4f) // if too far
                rb.velocity = new Vector3(speed * xDiff / targetDistance, rb.velocity.y, speed * zDiff / targetDistance);
            else if (targetDistance < 1.3f) // if too close
            {
                if (Random.value < 0.5f) // pick circling direction for when out of reach
                    movementState = 2;
                else
                    movementState = 3;

                rb.velocity = new Vector3(speed * xDiff / targetDistance, rb.velocity.y, -speed * zDiff / targetDistance);
            }
            else
            {
                switch (movementState)
                {
                    case 0:
                        rb.velocity = new Vector3(0, rb.velocity.y, 0);
                        break;
                    case 1:
                        rb.velocity = transform.forward * chargeSpeed + new Vector3(0, rb.velocity.y, 0);
                        break;
                    case 2:
                        rb.velocity = transform.right + new Vector3(0, rb.velocity.y, 0);
                        break;
                    case 3:
                        rb.velocity = -transform.right + new Vector3(0, rb.velocity.y, 0);
                        break;
                    case 4:
                        rb.velocity = -transform.forward * chargeSpeed + new Vector3(0, rb.velocity.y, 0);
                        break;
                }
            }
        }
        
        
        /*if (targetDistance * Random.value < 3f) // if close enough to backstep or sidestep (technically could forward step)
        {
            if(Random.value < 0.5f)
                rb.AddForce(Random.Range(maxStep / 2f, maxStep), 0, Random.Range(maxStep / 2f, maxStep));
            else
                rb.AddForce(Random.Range(-maxStep, -maxStep / 2f), 0, Random.Range(-maxStep, -maxStep / 2f));
        }
        else if(Random.value < 0.5f)
        {
            rb.AddForce(maxStep * xDiff / targetDistance, 0, maxStep * zDiff / targetDistance);
        }*/
    }

    private void Attack()
    {
        if (!attacking)
        {
            movementState = 0;
            movementTimer += 0.2f;
            if((enemyType == 0 || enemyType == 3) && shieldS)
                shieldS.SetGuardPosition(2);
            attacking = true;
            attackTimer = Random.Range(maxAttackTime / 2f, maxAttackTime);
            if (armS != null)
            {
                if (enemyType == 1)
                {
                    armS.RaiseHack((int)Random.Range(1f, 4f));
                }
                else
                {
                    if (Random.value < 0.8f)
                        armS.RaiseHack(3);
                    else
                        armS.RaiseHack((int)Random.Range(1f, 3f));
                }
            }
            StartCoroutine("FinishAttack");
        }
    }

    IEnumerator FinishAttack()
    {
        yield return new WaitForSeconds(Random.Range(0, (maxAttackTime / 2f) - 0.1f));
        if(armS != null)
            armS.Hack();
        attacking = false;
        if(enemyType == 0 || enemyType == 3)
            StartCoroutine("DelayResetShield");
    }

    IEnumerator DelayResetShield()
    {
        yield return new WaitForSeconds(0.7f);
        if(shieldS)
            shieldS.SetGuardPosition(0);
    }

    private void Block()
    {
        attackTimer = Random.Range(maxAttackTime / 2f, maxAttackTime);
        if(shieldS)
            shieldS.SetGuardPosition(1);
        StartCoroutine("DelayResetShield");
    }

    public void Wound(float damage)
    {
        if (damage > 0.2f)
            beS.Bleed(transform.position);
        health -= damage;
        if (health <= 0)
        { 
            Death();
        }
    }

    private void Death()
    {
        if (!dead)
        {
            playerS.IncrementEnemyDead();
            scS.IncreaseScore(1);
            acS.PlaySound("Oof", eSource);
            enemyMarker.SetActive(false);
            foreach (BloodyPuddleScript s in bpS)
                StartCoroutine("DelayBleedOut", s);
            dead = true;
            foreach(Collider coll in allColliders)
                coll.material = bouncyDeathPM;
            rb.velocity = new Vector3(Random.Range(-6f, 6f), 6f, Random.Range(-6f, 6f));
            rb.constraints = RigidbodyConstraints.None;
            rb.AddTorque(new Vector3(Random.Range(-100f, 200f), Random.Range(600f, 1000f), Random.Range(1000f, 1500f)));
        }
    }

    public void RemoveFromHitbox(GameObject o)
    {
        armS.ForwardRemoveFromCollider(o);
    }

    IEnumerator DelayBleedOut(BloodyPuddleScript blpuS)
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 4f));
        blpuS.BleedOut(transform.position);
    }

    private void SetTarget()
    {
        if (!target.activeInHierarchy)
            target = player;
        foreach (GameObject e in enemies)
        {
            if (e.activeInHierarchy && e.transform.parent != transform && Vector2.Distance(new Vector2(e.transform.position.x, e.transform.position.z), new Vector2(transform.position.x, transform.position.z)) + MINIMUM_TARGET_SWITCH < Vector2.Distance(new Vector2(target.transform.position.x, target.transform.position.z), new Vector2(transform.position.x, transform.position.z)))
                target = e;
        }
        if (Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(transform.position.x, transform.position.z)) + MINIMUM_TARGET_SWITCH < Vector2.Distance(new Vector2(target.transform.position.x, target.transform.position.z), new Vector2(transform.position.x, transform.position.z)))
            target = player;
    }

}
