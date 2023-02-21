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
    [SerializeField] float doublePressWait;
    [SerializeField] TextMeshProUGUI healthText;
    [Header("Physics Properties")]
    [SerializeField] LayerMask GroundLayers;
    [SerializeField] float GroundRaycastLength;
    [SerializeField] float jumpForce;
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier;

    [Header("Rotation Properties")]
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform guy;
    Transform cam;

    [Header("Punch")]
    [SerializeField] float punchTime;
    [SerializeField] float punchCooldown;
    [SerializeField] BoxCollider punchCollider;
    bool canPunch;
    bool punching;

    [Header("Flight")]
    bool isFlying;
    [SerializeField] bool flight;
    [SerializeField] float flightSpeed;
    [SerializeField] float riseSpeed;
    [SerializeField] float lowerSpeed;

    [Header("Laser Vision")]
    [SerializeField] bool laserVision;
    [SerializeField] float angleDiff;
    [SerializeField] Transform head;
    [SerializeField] Transform eyeball1;
    [SerializeField] Transform eyeball2;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask LaserLayers;
    [SerializeField] Vector3 directionOffset;
    [SerializeField] GameObject laserEffect;

    [Header("Aim Constraint")]
    [SerializeField] MultiAimConstraint headAimConstraint;
    [SerializeField] Transform laserVisionTarget;
    [SerializeField] float WeightLerpSpeed;

    [Header("Damage Values")]
    [SerializeField] float laserDamage;
    [SerializeField] float punchDamage;

    List<GameObject> spawnedEffects = new List<GameObject>();
    LineRenderer laser1;
    LineRenderer laser2;
    Animator ani;
    public static HealthManager playerHealth;
    
    // Start is called before the first frame update
    void Awake()
    {
        //GET COMPONENTING
        b = GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        laser1 = eyeball1.GetComponent<LineRenderer>();
        laser2 = eyeball2.GetComponent<LineRenderer>();
        ani = GetComponentInChildren<Animator>();
        playerHealth = GetComponent<HealthManager>();
        punchCollider.GetComponent<MeleeHitbox>().damage = punchDamage;

        //OTHER STUFF
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        canPunch = true;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Inputs();
        healthText.text = Mathf.RoundToInt(playerHealth.health).ToString();
    }
    private void FixedUpdate()
    {
        //JUMPING
        if (grounded && Input.GetKeyDown("space"))
        {
            //b.AddForce(Vector3.up * jumpForce * 100 * Time.deltaTime, ForceMode.Impulse);
            b.velocity = new Vector2(0, jumpForce * Time.deltaTime);
        }
        //Activating Flight
        else if (!grounded && Input.GetKeyDown("space") && flight)
        {
            isFlying = true;
            ani.SetBool("flying", true);
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

    void Inputs()
    {
        //PLAYER MOVEMENT

            //sprinting
            if (Input.GetKeyDown("left shift"))
            {
                moveSpeed *= sprintMultiplier;
                flightSpeed *= sprintMultiplier;
                ani.SetBool("sprinting", true);
            }
            if (Input.GetKeyUp("left shift"))
            {
                moveSpeed /= sprintMultiplier;
                flightSpeed /= sprintMultiplier;
            ani.SetBool("sprinting", false);
            }

            //regular movement
            if (!isFlying)
            {
                GroundMovement();
            }
            //flight movement
            else
            {
                Flying();
            }
            if (laserVision)
            {
                LaserVision(eyeball1,laser1);
                LaserVision(eyeball2, laser2);
            }
            ani.SetBool("punch",punching);
            punchCollider.enabled = punching;
            if((Input.GetKeyDown("r") || Input.GetMouseButtonDown(1)) && canPunch)
            {
                punching = true;
                Invoke("DisablePunch",punchTime);
            }
            
    }

    void GroundMovement()
    {
        WASDmovement(moveSpeed);
    }

    void DisablePunch()
    {
        punching = false;
        canPunch = false;
        Invoke("EnablePunch", punchCooldown);
    }

    void EnablePunch()
    {
        canPunch = true;
    }

    void Flying()
    {
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
        if (grounded || doublePressed)
        {
            doublePressed = false;
            isFlying = false;
            b.useGravity = true;
            ani.SetBool("flying", false);
        }
    }

    void LaserVision(Transform eyeball,LineRenderer laser)
    {
        laser.SetPosition(0, eyeball.position);
        //when firing laser
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = cam.rotation * Vector3.forward + directionOffset;
            //Vector3 maxDirection = Quaternion.AngleAxis(angleLimit, Vector3.up) * guy.forward;
            //Vector3 minDirection = Quaternion.AngleAxis(-angleLimit, Vector3.up) * guy.forward;
            bool hit = Physics.Raycast(eyeball.position, direction.normalized, out RaycastHit ray, maxDistance, LaserLayers);
            if (hit)
            {
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
    }


    void DestroyOldestEffect()
    {
        Destroy(spawnedEffects[0]);
        spawnedEffects.Remove(spawnedEffects[0]);
    }
}
