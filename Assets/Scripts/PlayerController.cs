using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody b;
    bool grounded;
    int presses;
    bool doublePressed;
    [SerializeField] float doublePressWait;
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

    [Header("Flight")]
    bool isFlying;
    [SerializeField] bool flight;
    [SerializeField] float flightSpeed;
    [SerializeField] float riseSpeed;
    [SerializeField] float lowerSpeed;

    [Header("Laser Vision")]
    [SerializeField] float laserDamage;
    [SerializeField] bool laserVision;
    [SerializeField] Transform eyeball1;
    [SerializeField] Transform eyeball2;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask LaserLayers;
    [SerializeField] Vector3 directionOffset;
    [SerializeField] GameObject laserEffect;
    List<GameObject> spawnedEffects = new List<GameObject>();
    LineRenderer laser1;
    LineRenderer laser2;
    Animator ani;
    public static HealthManager playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        laser1 = eyeball1.GetComponent<LineRenderer>();
        laser2 = eyeball2.GetComponent<LineRenderer>();
        ani = GetComponentInChildren<Animator>();
        Application.targetFrameRate = 60;
        playerHealth = GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Inputs();
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
            
    }

    void GroundMovement()
    {
        WASDmovement(moveSpeed);
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
            bool hit = Physics.Raycast(eyeball.position, direction.normalized, out RaycastHit ray, maxDistance, LaserLayers);
            if (hit)
            {
                laser.SetPosition(1, ray.point);
                GameObject effect = Instantiate(laserEffect,ray.point,Quaternion.Euler(0,0,0));
                spawnedEffects.Add(effect);
                Invoke("DestroyOldestEffect",3f);
                if(TryGetComponent(out HealthManager health))
                {
                    health.HealthChange(-laserDamage);
                }
            }   
            else
            {
                laser.SetPosition(1, eyeball.position);
            }
            Debug.DrawRay(eyeball.position, direction.normalized);
        }
        else
        {
            laser.SetPosition(1, eyeball.position);
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
