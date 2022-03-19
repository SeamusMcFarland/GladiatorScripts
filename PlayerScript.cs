using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    Rigidbody rb;
    float maxSpeed; // can change if both axis are being held
    float typeSpeed;
    float acceleration;
    float jumpSpeed;

    const float MAX_MOUSE_Y = 60f;
    float verticalRotation;
    float horizontalRotation;

    GameObject camObj;

    const float VERTICAL_MOUSE_SENSITIVITY = 1f;
    const float HORIZONTAL_MOUSE_SENSITIVITY = 1.5f;

    bool grounded;
    float stateAcceleration;

    bool sprinting;
    float energy;
    float maxEnergy;
    const float SPRINT_MODIFIER = 1.6f;
    float energyTimer;
    const float ENERGY_TIMER_DELAY = 0.3f;
    const float MINIMUM_ENERGY_PERCENTAGE = 0.15f;

    bool shiftK;
    bool leftMDown;
    bool rightMDown;
    bool bothMDown;

    public Image staminaBarRemaining;
    public Image staminaBarOuter;
    public Image staminaBarInner;
    float savedStaminaX;
    bool staminaFilled;

    public Image healthBar;
    float savedHealthX;

    Vector3 relativeVelocity;

    ArmScript armS;

    BloodEffectScript beS;
    float health;
    float maxHealth;
    HelmetCoverScript hcS;

    bool dead;

    float frameRegulator;

    public WinLoseCanvasScript wlcS;
    bool ended;

    int enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        beS = GameObject.FindGameObjectWithTag("BloodEffect").GetComponent<BloodEffectScript>();
        armS = GetComponentInChildren<ArmScript>();
        hcS = GetComponentInChildren<HelmetCoverScript>();

        maxHealth = 10f;
        health = maxHealth;
        staminaFilled = false;
        SetStaminaBarEnabled(false);
        savedStaminaX = staminaBarRemaining.rectTransform.localPosition.x;
        savedHealthX = healthBar.rectTransform.localPosition.x;

        maxEnergy = 5f;
        energy = maxEnergy;

        camObj = GetComponentInChildren<Camera>().gameObject;

        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        typeSpeed = 3f;
        jumpSpeed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        frameRegulator = Time.deltaTime / 0.015f;
        if (!dead)
        {
            CheckInputs();
            CheckSprinting();
            CheckEnergy();
            RefreshStaminaBar();
            RefreshHealthBar();
            CheckVelocity();
            CheckVision();
            CheckAttack();
            CheckJump();
        }
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (!ended)
        {
            if (dead)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                wlcS.ActivateButtons(false);
                ended = true;
            }
            else if (enemiesRemaining <= 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                wlcS.ActivateButtons(true);
                ended = true;
            }
        }
    }

    private void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            shiftK = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shiftK = false;

        if (Input.GetMouseButtonDown(0))
            leftMDown = true;
        else if (Input.GetMouseButtonUp(0))
            leftMDown = false;

        if (Input.GetMouseButtonDown(1))
            rightMDown = true;
        else if (Input.GetMouseButtonUp(1))
            rightMDown = false;

        if (rightMDown && leftMDown)
            bothMDown = true;
        else
            bothMDown = false;
    }

    private void CheckSprinting()
    {
        if ((energy / maxEnergy) > MINIMUM_ENERGY_PERCENTAGE && shiftK)
            sprinting = true;
        else if (energy < 0 || !shiftK)
            sprinting = false;

        if (sprinting)
            SetStaminaBarEnabled(true);
    }

    private void CheckEnergy()
    {
        if (sprinting)
        {
            energyTimer = ENERGY_TIMER_DELAY;
            energy -= Time.deltaTime;
        }

        if (energyTimer < 0)
        {
            if (energy < maxEnergy)
                energy += Time.deltaTime;
        }
        else
        {
            energyTimer -= Time.deltaTime;
        }
    }

    private void RefreshStaminaBar()
    {
        if (energy >= maxEnergy - 0.01f)
        {
            SetStaminaBarEnabled(false);
            staminaFilled = true;
        }
        else
        {
            staminaBarRemaining.transform.localScale = new Vector2(energy / maxEnergy, staminaBarRemaining.transform.localScale.y);
            staminaBarRemaining.rectTransform.localPosition = new Vector2(savedStaminaX - ((50f / maxEnergy) * (maxEnergy - energy)), staminaBarRemaining.rectTransform.localPosition.y);
        }
    }

    private void RefreshHealthBar()
    {
        healthBar.transform.localScale = new Vector2(health / maxHealth, healthBar.transform.localScale.y);
        healthBar.rectTransform.localPosition = new Vector2(savedHealthX - ((50f / maxHealth) * (maxHealth - health)), healthBar.rectTransform.localPosition.y);
    }

    private void SetStaminaBarEnabled(bool e)
    {
        staminaBarRemaining.enabled = e;
        staminaBarOuter.enabled = e;
        staminaBarInner.enabled = e;
    }

    private void CheckVelocity()
    {
        relativeVelocity = transform.InverseTransformDirection(rb.velocity);

        maxSpeed = typeSpeed;

        if (sprinting)
            maxSpeed *= SPRINT_MODIFIER;

        if (Mathf.Abs(relativeVelocity.x) < 0.01f && Mathf.Abs(relativeVelocity.z) < 0.01f)
            maxSpeed /= 1.41f; // 1.41 is roughly sqrt of 2

        if (grounded)
            acceleration = 0.5f;
        else
            acceleration = 0.05f;

        acceleration *= frameRegulator;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            if (Input.GetAxis("Horizontal") > 0.01f)
            {
                if (relativeVelocity.x < maxSpeed)
                    rb.velocity = transform.TransformDirection(new Vector3(relativeVelocity.x + acceleration, relativeVelocity.y, relativeVelocity.z));
            }
            else
            {
                if (relativeVelocity.x > -maxSpeed)
                    rb.velocity = transform.TransformDirection(new Vector3(relativeVelocity.x - acceleration, relativeVelocity.y, relativeVelocity.z));
            }
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f)
        {
            if (Input.GetAxis("Vertical") > 0.01f)
            {
                if (relativeVelocity.z < maxSpeed)
                    rb.velocity = transform.TransformDirection(new Vector3(relativeVelocity.x, relativeVelocity.y, relativeVelocity.z + acceleration));
            }
            else
            {
                if (relativeVelocity.z > -maxSpeed)
                    rb.velocity = transform.TransformDirection(new Vector3(relativeVelocity.x, relativeVelocity.y, relativeVelocity.z - acceleration));
            }
        }
    }

    private void CheckVision()
    {
        CheckVerticalMouse();
        CheckHorizontalMouse();
        camObj.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.localRotation = Quaternion.Euler(0, horizontalRotation, 0);
    }

    private void CheckAttack()
    {
        CheckIfRaise();
        CheckIfHack();
    }

    private void CheckIfRaise()
    {
        if (bothMDown)
        {
            if (armS.GetRaised() && armS.GetTypeHacking() == 0) // raised but not yet hacking
                armS.ResetArm();
            armS.RaiseHack(3);
        }
        else if (leftMDown)
            armS.RaiseHack(1);
        else if (rightMDown)
            armS.RaiseHack(2);
    }

    private void CheckIfHack()
    {
        if (armS.GetRaised())
        {
            switch (armS.GetAttackType())
            {
                case 1:
                    if (!leftMDown)
                        armS.Hack();
                    break;
                case 2:
                    if (!rightMDown)
                        armS.Hack();
                    break;
                case 3:
                    if (!bothMDown)
                        armS.Hack();
                    break;
            }
        }
    }

    private void CheckVerticalMouse()
    {
        if (-Input.GetAxis("Mouse Y") * VERTICAL_MOUSE_SENSITIVITY > 0)
        {
            if (-Input.GetAxis("Mouse Y") * VERTICAL_MOUSE_SENSITIVITY + verticalRotation < MAX_MOUSE_Y)
            {
                verticalRotation += -Input.GetAxis("Mouse Y") * VERTICAL_MOUSE_SENSITIVITY;
            }
            else
            {
                verticalRotation = MAX_MOUSE_Y;
            }
        }
        else
        {
            if (-Input.GetAxis("Mouse Y") * VERTICAL_MOUSE_SENSITIVITY + verticalRotation > -MAX_MOUSE_Y)
            {
                verticalRotation += -Input.GetAxis("Mouse Y") * VERTICAL_MOUSE_SENSITIVITY;

            }
            else
            {
                verticalRotation = -MAX_MOUSE_Y;
            }
        }
    }

    private void CheckHorizontalMouse()
    {
        horizontalRotation += Input.GetAxis("Mouse X") * HORIZONTAL_MOUSE_SENSITIVITY;
    }

    private void CheckJump()
    {
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }
    }

    public void SetGrounded(bool b)
    {
        grounded = b;
    }

    public void Wound(float damage)
    {
        beS.Bleed(transform.position);
        health -= damage;
        if (health <= 0)
            Death();
    }

    private void Death()
    {
        if (!dead)
        {
            dead = true;
            hcS.SetToRed();
        }
    }

    public void RemoveFromHitbox(GameObject o)
    {
        armS.ForwardRemoveFromCollider(o);
    }

    public void IncrementEnemyDead()
    {
        enemiesRemaining--;
    }

    public void SetEnemiesRemaining(int n)
    {
        enemiesRemaining = n;
    }

    public bool GetDead()
    {
        return dead;
    }

    public float GetHealth()
    {
        return health;
    }

}
