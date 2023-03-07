using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Rigidbody b;
    bool grounded;
    int presses;
    bool doublePressed;
    List<GameObject> spawnedEffects = new List<GameObject>();
    LineRenderer laser1;
    LineRenderer laser2;
    Animator ani;
    public static HealthManager playerHealth;

    [SerializeField] float doublePressWait;
    public bool disableInputs;
    [Header("[POWER SETTINGS]")]
    [SerializeField] float maxEnergy;
    [SerializeField] float energyRegenRate;
    public static float energy;
    [SerializeField] bool laserVision;
    [SerializeField] bool superSpeed;
    [SerializeField] bool flight;
    bool regen;

    [Header("[Physics Properties]")]
    [SerializeField] LayerMask GroundLayers;
    [SerializeField] float GroundRaycastLength;
    [SerializeField] float jumpForce;
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float fallSpeed;

    [Header("[Rotation Properties]")]
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform guy;
    Transform cam;

    [Header("[PUNCH]")]
    [SerializeField] float punchTime;
    [SerializeField] float punchCooldown;
    [SerializeField] float punchForce;
    [SerializeField] BoxCollider punchCollider;
    [SerializeField] float punchWaitTime;
    MeleeHitbox meleeScript;
    bool canPunch;

    [Header("[FLIGHT]")]
    [SerializeField] float flightSpeed;
    [SerializeField] float riseSpeed;
    [SerializeField] float lowerSpeed;
    [SerializeField] float flightDrain;
    bool isFlying;

    [Header("[LASER VISION]")]
    [SerializeField] float angleDiff;
    [SerializeField] Transform head;
    [SerializeField] Transform eyeball1;
    [SerializeField] Transform eyeball2;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask LaserLayers;
    [SerializeField] Vector3 directionOffset;
    [SerializeField] GameObject laserEffect;
    [SerializeField] Transform crosshair;
    [SerializeField] float laserDrain;
    Vector3 hitpoint1;
    Vector3 hitpoint2;
    Vector3 laserMidpoint;

    [Header("[SUPER SPEED]")] 
    [SerializeField] float speedMultiplier;

    [Header("[Aim Constraint]")]
    [SerializeField] MultiAimConstraint headAimConstraint;
    [SerializeField] Transform laserVisionTarget;
    [SerializeField] float WeightLerpSpeed;

    [Header("[DAMAGE VALUES]")]
    [SerializeField] float laserDamage;
    [SerializeField] float punchDamage;

    [Header("[Death]")]
    [SerializeField] GameObject bloodEffect;
    [SerializeField] Transform bloodSpawn;

    // Start is called before the first frame update
    void Start()
    {
        //GET COMPONENTING
        b = GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        laser1 = eyeball1.GetComponent<LineRenderer>();
        laser2 = eyeball2.GetComponent<LineRenderer>();
        ani = GetComponentInChildren<Animator>();
        playerHealth = GetComponent<HealthManager>();
        meleeScript = punchCollider.GetComponent<MeleeHitbox>();

        //OTHER STUFF
        meleeScript.meleeDamage = punchDamage;
        meleeScript.force = punchForce;
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        canPunch = true;
        energy = maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        regen = true;
        GroundCheck();
        crosshair.gameObject.SetActive(laserVision);
        if (!disableInputs)
        {
            Inputs();
        }
        if (regen)
        {
            Invoke("EnergyRegen",3);
        }
    }

    void WASDmovement(float speed)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            ani.SetBool("moving", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            guy.rotation = Quaternion.Slerp(guy.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Vector3 moveDir = targetRotation * Vector3.forward;
            b.AddForce(moveDir.normalized * speed * 100 * Time.deltaTime, ForceMode.Force);
        }
        else
        {
            ani.SetBool("moving", false);
        }
    }
    private void FixedUpdate()
    {
        if (!disableInputs)
        {
            //JUMPING
            if (grounded && Input.GetKeyDown("space") && playerHealth.health != 0)
            {
                //b.AddForce(Vector3.up * jumpForce * 100 * Time.deltaTime, ForceMode.Impulse);
                b.velocity = new Vector2(0, jumpForce * Time.deltaTime);
            }
            //Activating Flight
            else if (!grounded && Input.GetKeyDown("space") && flight && energy >= 0)
            {
                isFlying = true;
                ani.SetBool("flying", true);
            }
        }
    }
    void Inputs()
    {
        //PLAYER MOVEMENT

            //sprinting
            if (Input.GetKeyDown("left shift"))
            {
                if (superSpeed)
                {
                    sprintMultiplier *= speedMultiplier;
                    Time.timeScale /= speedMultiplier;
                    Application.targetFrameRate = 120;
            }
                moveSpeed *= sprintMultiplier;
                flightSpeed *= sprintMultiplier;
                ani.SetBool("sprinting", true);
            }
            if (Input.GetKeyUp("left shift"))
            {
                moveSpeed /= sprintMultiplier;
                flightSpeed /= sprintMultiplier;
                ani.SetBool("sprinting", false);
                if (superSpeed)
                {
                    sprintMultiplier /= speedMultiplier;
                    Time.timeScale *= speedMultiplier;
                    Application.targetFrameRate = 120;
            }
        }

            //regular movement
            if (!isFlying)
            {
                GroundMovement();
                if (!grounded)
                {
                    b.AddForce(Vector3.down * fallSpeed * 100 * Time.deltaTime, ForceMode.Force);
                }
            }
            //flight movement
            else
            {
                Flying();
            }
            if (laserVision)
            {
                hitpoint1 = LaserVision(eyeball1,laser1);
                hitpoint2 = LaserVision(eyeball2, laser2);
                
                laserMidpoint = hitpoint1 + (hitpoint2 - hitpoint1) / 2;
                crosshair.position = Camera.main.WorldToScreenPoint(laserMidpoint);               
            }
            if (ani.GetBool("punch"))
            {
                Vector3 direction = cam.rotation * Vector3.forward + directionOffset;
                if (Vector3.Angle(head.forward, direction) > angleDiff)
                {
                    guy.rotation = Quaternion.Slerp(guy.rotation, Quaternion.Euler(0, cam.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
                }
            }
            if((Input.GetKeyDown("r") || Input.GetMouseButtonDown(1)) && canPunch)
            {
                canPunch = false;
                ani.SetBool("punch", true);
                Invoke("Punch", punchWaitTime);
            }        
    }

    void GroundMovement()
    {
        WASDmovement(moveSpeed);
    }
    void Punch()
    {
        punchCollider.enabled = true;
        Invoke("DisablePunch", punchTime);
    }

    void DisablePunch()
    {
        punchCollider.enabled = false;
        ani.SetBool("punch", false);        
        Invoke("EnablePunch", punchCooldown);
    }

    void EnablePunch()
    {
        canPunch = true;
    }

    void Flying()
    {
        EnergyDrain(flightDrain);
        b.useGravity = false;
        //WASD controls
        WASDmovement(flightSpeed);
        if (Input.GetButtonDown("Jump"))
        {
            presses += 1;
            Invoke("AwaitSecondPress", doublePressWait);
        }
        //rising of player height
        if (Input.GetButton("Jump"))
        {
            b.AddForce(Vector3.up * riseSpeed * 100 * Time.deltaTime, ForceMode.Force);
        }
        else if (Input.GetButton("Crouch"))
        {
            b.AddForce(Vector3.down * lowerSpeed * 100 * Time.deltaTime, ForceMode.Force);
        }
        //Deactivating Flight
        if (grounded || doublePressed || energy <= 0)
        {
            doublePressed = false;
            isFlying = false;
            b.useGravity = true;
            ani.SetBool("flying", false);
        }
    }

    Vector3 LaserVision(Transform eyeball,LineRenderer laser)
    {
        laser.SetPosition(0, eyeball.position);

        //laser raycast
        Vector3 direction = cam.rotation * Vector3.forward + directionOffset;
        bool hit = Physics.Raycast(eyeball.position, direction.normalized, out RaycastHit ray, maxDistance, LaserLayers);

        
        //when firing laser
        if (Input.GetMouseButton(0))
        {
            regen = false;
            if (hit && energy >= 0)
            {
                EnergyDrain(laserDrain / 2);                
                laser.SetPosition(1, ray.point);
                GameObject effect = Instantiate(laserEffect,ray.point,Quaternion.Euler(0,0,0));
                spawnedEffects.Add(effect);
                headAimConstraint.weight = Mathf.Lerp(headAimConstraint.weight, 1, WeightLerpSpeed * Time.deltaTime);
                laserVisionTarget.position = ray.point;
                Invoke("DestroyOldestEffect",3f);
                if(ray.collider.gameObject.TryGetComponent(out HealthManager health))
                {
                    health.HealthChange(-laserDamage*Time.deltaTime);
                }

                float angle = head.localRotation.eulerAngles.y;             
                if (Vector3.Angle(head.forward, direction) > angleDiff)
                {
                    guy.rotation = Quaternion.Slerp(guy.rotation, Quaternion.Euler(0, cam.eulerAngles.y, 0),rotationSpeed*Time.deltaTime/2);
                }
            }   
            else
            {
                laser.SetPosition(1, eyeball.position);
                headAimConstraint.weight = Mathf.Lerp(headAimConstraint.weight, 0, WeightLerpSpeed * Time.deltaTime);
            }
            Debug.DrawRay(eyeball.position, direction.normalized);
        }
        else
        {
            laser.SetPosition(1, eyeball.position);
            headAimConstraint.weight = Mathf.Lerp(headAimConstraint.weight,0, WeightLerpSpeed * Time.deltaTime);
        }
        if (hit)
        {
            return ray.point;
        }
        else
        {
            return laser.GetPosition(1);
        }
    }
    


    void AwaitSecondPress()
    {
        if(presses % 2 == 0)
        {
            doublePressed = true;
        }
        else
        {
            presses = 0;
        }
    }

    void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, GroundRaycastLength, GroundLayers);
        ani.SetBool("airborn", !grounded);
        Debug.DrawRay(transform.position, Vector3.down*GroundRaycastLength, Color.magenta);

        //Death Event
        if (playerHealth.health <= 0 && grounded)
        {
            b.isKinematic = true;
            //reload scene or whatever
        }
    }


    void DestroyOldestEffect()
    {
        Destroy(spawnedEffects[0]);
        spawnedEffects.Remove(spawnedEffects[0]);
    }


    public void Die()
    {
        ani.SetBool("dead", true);
        disableInputs = true;
        laser1.enabled = false;
        laser2.enabled = false;
        GameObject blood = Instantiate(bloodEffect,bloodSpawn.position,bloodSpawn.rotation,bloodSpawn);
        if (isFlying)
        {
            isFlying = false;
            b.useGravity = true;
        }
    }

    public void Dialogue()
    {
        disableInputs = !disableInputs;
    }

    public void EnergyDrain(float rate)
    {
        regen = false;
        energy -= (Time.deltaTime * rate);
    }

    public void EnergyRegen()
    {
        energy += (Time.deltaTime * energyRegenRate);
    }

}
