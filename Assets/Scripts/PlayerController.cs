using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public delegate void freeze();
public delegate void unfreeze();

public class PlayerController : MonoBehaviour
{
    public static event freeze freezeEvent;
    public static event unfreeze unFreezeEvent;

    Rigidbody b;
    bool grounded;
    bool doublePressed;
    List<string> queuedActions = new List<string>();
    List<string> previousActions = new List<string>();

    List<GameObject> spawnedEffects = new List<GameObject>();
    LineRenderer laser1;
    LineRenderer laser2;
    Animator ani;
    public static HealthManager playerHealth;

    [SerializeField][Tooltip("the maximum amount of times between button presses for a combo")] float comboDelay;
    public bool disableInputs;
    [Header("[POWER SETTINGS]")]
    [SerializeField] float maxEnergy;
    [SerializeField] float energyRegenRate;
    public static float energy;
    [SerializeField] bool laserVision;
    [SerializeField] bool superSpeed;
    [SerializeField] bool flight;
    bool regen;
    float currentRegenRate;

    [Header("[Physics Properties]")]
    [SerializeField] LayerMask GroundLayers;
    [SerializeField] float GroundRaycastLength;
    [SerializeField] float jumpForce;
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float fallSpeed;
    bool sprinting;
    bool moving;
    Vector3 directionalInput;

    [Header("[Rotation Properties]")]
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform guy;
    Transform cam;

    [Header("[PUNCH]")]
    [SerializeField] [Tooltip("The time between pressing punch and the hitbox appearing for it")] float punchWaitTime;
    [SerializeField] [Tooltip("How long the punch hitbox appears for")] float punchTime;
    [SerializeField] [Tooltip("The time after punching until you can punch again")]float punchCooldown;
    [SerializeField] [Tooltip("The accelaration that regular punches apply")]float punchAccelaration;
    [SerializeField] [Tooltip("The accelaration that kicks apply")] float kickAccelaration;
    [SerializeField] BoxCollider punchCollider;
    bool punchAxisInUse;

    MeleeHitbox meleeScript;
    bool canPunch;

    [Header("[PICK UP AND THROW]")]
    [SerializeField] float throwAccelaration;
    [SerializeField] Transform holdPosition;
    [SerializeField] float pickupTime;
    GameObject currentlyTouchedPickup;

    enum pickupStates {notholding,pickingUp,holding}
    pickupStates pickUpState;
    float elapsedThrowTime;

    [Header("[FLIGHT]")]
    [SerializeField] float flightSpeed;
    [SerializeField] float riseSpeed;
    [SerializeField] float lowerSpeed;
    [SerializeField] float flightDrain;
    bool isFlying;

    [Header("[ROLL]")]
    [SerializeField] float rollForce;
    [SerializeField] float iTime = 0.5f;
    [SerializeField] float rollCooldown;
    [SerializeField] Vector3 rollColliderOffset;
    bool isRolling;

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
    [SerializeField] float superSpeedMultiplier;
    [SerializeField] GameObject superSpeedEffect;
    [SerializeField] GameObject speedOverlay;
    bool isSpeeding;

    [Header("[Aim Constraint]")]
    [SerializeField] MultiAimConstraint headAimConstraint;
    [SerializeField] Transform laserVisionTarget;
    [SerializeField] float WeightLerpSpeed;

    [Header("[DAMAGE VALUES]")]
    [SerializeField] float laserDamage;
    [SerializeField] float punchDamage;
    [SerializeField] float standingKickDamage;
    [SerializeField] float runningKickDamage;

    [Header("[Death]")]
    [SerializeField] GameObject bloodEffect;
    [SerializeField] Transform bloodSpawn;
    [SerializeField] GameObject deathScreen;

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
        meleeScript.meleeAccelaration = punchAccelaration;
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        crosshair.gameObject.SetActive(laserVision);
        Cursor.visible = false;
        canPunch = true;
        energy = maxEnergy;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {       
        //getting and setting
        GroundCheck();

        //inputs
        if (!disableInputs)
        {
            ButtonInputs();
            DirectionalInputs();
        }

        //regen
        regen = true;
        if (regen)
        {
            currentRegenRate = energyRegenRate;
            Invoke("EnergyRegen",3);
        }
        else
        {
            currentRegenRate = 0;
        }
    }

    private void FixedUpdate()
    {
        ButtonExecution();

        //Rotating the player to the camera direction
        if (ani.GetBool("punch"))
        {
            RotatePlayerToCam();
        }
        //rolling movement
        if (isRolling)
        {
            DirectionalMovement(rollForce, 0f);
        }
        //regular movement
        else if (!isFlying)
        {
            DirectionalMovement(moveSpeed);
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
    }


    void DirectionalMovement(float speed,float minMagnitude = 0.1f)
    {
        if (sprinting && !isRolling)
        {
            if(isSpeeding)
            {
                speed *= superSpeedMultiplier;
            }
            else
            {
                speed *= sprintMultiplier;
            }            
        }
        if (directionalInput.magnitude >= minMagnitude && !punchCollider.enabled)
        {
            ani.SetBool("moving", true);
            float targetAngle = Mathf.Atan2(directionalInput.x, directionalInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
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

    void Flying()
    {
        EnergyDrain(flightDrain);
        b.useGravity = false;
        //Movement
        DirectionalMovement(flightSpeed);
        if (Input.GetButtonDown("Jump") || grounded || energy <= 0)
        {
            queuedActions.Add("flightDeactivate");
        }
        //rising of player height
        if (Input.GetButton("Jump"))
        {
            b.AddForce(Vector3.up * riseSpeed * 100 * Time.deltaTime, ForceMode.Force);
        }
        else if (Input.GetAxis("Crouch") > 0.4f)
        {
            b.AddForce(Vector3.down * lowerSpeed * 100 * Time.deltaTime, ForceMode.Force);
        }
    }

    public IEnumerator Roll()
    {
        isRolling = true;
        ani.Play("Roll");
        playerHealth.canTakeDamage = false;
        disableInputs = true;
        if (TryGetComponent(out CapsuleCollider cpsle))
        {
            cpsle.height /= 2;
            cpsle.center += rollColliderOffset;
        }
        yield return new WaitForSeconds(iTime);
        playerHealth.canTakeDamage = true;
        if (iTime < rollCooldown)
        {
            yield return new WaitForSeconds(rollCooldown - iTime);
        }
        if (TryGetComponent(out CapsuleCollider cpsle1))
        {
            cpsle1.height *= 2;
            cpsle.center -= rollColliderOffset;
        }
        isRolling = false;
        disableInputs = false;
    }

    //RECORDING DIRECTION INPUTS
    void DirectionalInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        directionalInput = new Vector3(horizontal, 0f, vertical).normalized;
    }
    //RECORDING BUTTON INPUTS
    void ButtonInputs()
    {
        //PLAYER MOVEMENT

        //enabling sprinting
        if (Input.GetButtonDown("Sprint") && !queuedActions.Contains("startSprint"))
        {
            queuedActions.Add("startSprint");
        }
        //disabling sprint
        if (Input.GetButtonUp("Sprint") && !queuedActions.Contains("stopSprint"))
        {
            queuedActions.Add("stopSprint");
        }
        //enabling super speed
        if(Input.GetKeyDown("c") && superSpeed)
        {

                
        }
        //JUMPING
        if (Input.GetButtonDown("Jump") && !playerHealth.dead)
        {
            if (grounded && !isFlying)
            {
                queuedActions.Add("jump");
            }
            else if (flight && energy >= 0)
            {
                queuedActions.Add("startFlight");
            }          
        }        
        //PLAYER ACTIONS
        switch (pickUpState)
            {   
                //this runs when the player is picking an object up
                case pickupStates.pickingUp:
                    PickUp();
                    currentlyTouchedPickup.GetComponent<Throwable>().beingHeld = true;
                    break;

                //this runs when the player is holding an object
                case pickupStates.holding:
                    RotatePlayerToCam();
                if (currentlyTouchedPickup == null)
                {
                    ani.SetBool("carrying", false);
                    ani.Play("Idle");
                    pickUpState = pickupStates.notholding;
                    break;
                }
                else if (Input.GetButtonDown("Pickup"))
                {
                    queuedActions.Add("throw");
                }
                //punching
                else if ((Input.GetButtonDown("Punch")) && currentlyTouchedPickup.GetComponent<Throwable>().isLightObject)
                {
                    queuedActions.Add("punch");
                }
                break;


                //this runs when the player isn't holding anything or picking anything up
                case pickupStates.notholding:
                    //laser vision
                    if (laserVision)
                    {
                        hitpoint1 = LaserVision(eyeball1, laser1);
                        hitpoint2 = LaserVision(eyeball2, laser2);

                        laserMidpoint = hitpoint1 + (hitpoint2 - hitpoint1) / 2;
                        crosshair.gameObject.SetActive(true);
                        crosshair.position = Camera.main.WorldToScreenPoint(laserMidpoint);
                    }
                //punching
                    if ((Input.GetAxisRaw("Punch") == 1) && !punchAxisInUse)
                    {
                        queuedActions.Add("punch");
                        punchAxisInUse = true;
                    }
                    else if((Input.GetAxisRaw("Punch") == 0) && punchAxisInUse)
                    {
                        punchAxisInUse = false;
                    }
                    //picking up
                    if (Input.GetButtonDown("Pickup") && currentlyTouchedPickup != null)
                    {
                        queuedActions.Add("pickup");
                    }
                    //initiating roll
                    if (Input.GetButtonDown("Crouch") && grounded && !isFlying)
                    {
                        StartCoroutine(Roll());
                    }
                    break;
            }
    }
    //executing inputs
    void ButtonExecution()
    {       
        //checks how many of an action is in the queue
        bool ActionInQueue(string action, bool remove = true)
        {         
            if (queuedActions.Contains(action))
            {
                if (remove)
                {
                    StoreAsPrevious(action);
                }
                return true;
            }
            return false;
            
        }
        void RemoveActionsFromList(List<string> list, string action)
        {         
            if (list.Contains(action))
            {
                float n = 0;
                foreach (string s in list)
                {
                    if (s == action)
                    {
                        n += 1;
                    }
                }
                for(int x = 0; x < n; x++)
                {
                    list.Remove(action);                   
                }               
                
            }
        }
        void StoreAsPrevious(string action)
        {
            RemoveActionsFromList(queuedActions, action);
            previousActions.Add(action);
            StartCoroutine(ClearActionAfterSeconds(action, comboDelay));
        }
        //cycles through all actions and checks if their inputs have been recently pressed
        if (ActionInQueue("stopSprint"))
        {
            sprinting = false;
            ani.SetBool("sprinting", false);
            if (isSpeeding)
            {
                isSpeeding = false;
                unFreezeEvent();
                speedOverlay.SetActive(false);
                if(currentlyTouchedPickup != null)
                {
                    if (currentlyTouchedPickup.GetComponent<Throwable>().partOfRagdoll)
                    {
                        Drop();
                        queuedActions.Add("pickup");
                    }
                }
            }
        }
        if (ActionInQueue("startSprint",false))
        {
            sprinting = true;
            ani.SetBool("sprinting",true);
            if (previousActions.Contains("startSprint") && superSpeed && directionalInput.magnitude > 0.1f)
            {
                isSpeeding = true;               
                freezeEvent();
                Instantiate(superSpeedEffect, transform.position, transform.rotation);
                speedOverlay.SetActive(true);
                RemoveActionsFromList(previousActions, "startSprint");
                RemoveActionsFromList(queuedActions, "startSprint");
            }
            else
            {
                StoreAsPrevious("startSprint");
            }
            
        }
        if (ActionInQueue("throw"))
        {
            Throw();
        }
        
        //When punch is pressed once
        if (ActionInQueue("punch",false) && canPunch)
        {
            meleeScript.meleeAccelaration = punchAccelaration;
            meleeScript.meleeDamage = punchDamage;
            meleeScript.induceRagdoll = false;
            canPunch = false;
            ani.SetBool("punch", true);
            if (previousActions.Contains("punch"))
            {
                queuedActions.Add("punch2");
            }
            else if (sprinting && directionalInput.magnitude > 0.1f)
            {
                queuedActions.Add("kick");
            }
            else
            {
                switch (pickUpState)
                {
                    case pickupStates.notholding:
                        ani.Play("Right Hook");
                        break;
                    case pickupStates.holding:
                        ani.Play("MeleeWeapon1");
                        break;
                }                
                StoreAsPrevious("punch");
            }
            Invoke("Punch", punchWaitTime);
        }


        //When punch is pressed twice
        if (ActionInQueue("punch2", false))
        {
            if (previousActions.Contains("punch2"))
            {
                queuedActions.Add("kick");
            }
            else
            {
                switch (pickUpState)
                {
                    case pickupStates.notholding:
                        ani.Play("Left Hook");
                        break;
                    case pickupStates.holding:
                        ani.Play("MeleeWeapon3");
                        break;

                }
                StoreAsPrevious("punch2");
                RemoveActionsFromList(queuedActions, "punch");
            }
        }

        //when punch is pressed three times
        if (ActionInQueue("kick"))
        {   
            if(directionalInput.magnitude > 0.1f && sprinting)
            {
                ani.Play("Running Kick");
                meleeScript.meleeDamage = runningKickDamage;
            }
            else
            {
                switch (pickUpState)
                {
                    case pickupStates.notholding:
                        ani.Play("Standing Kick");
                        break;
                    case pickupStates.holding:
                        ani.Play("MeleeWeapon2");//replace with your special swing animations
                        break;
                }
                meleeScript.meleeDamage = standingKickDamage;
            }          
            meleeScript.induceRagdoll = true;
            meleeScript.meleeAccelaration = kickAccelaration;//increasing knockback for testing
            RemoveActionsFromList(queuedActions, "punch");
            RemoveActionsFromList(previousActions, "punch");
            RemoveActionsFromList(queuedActions, "punch2");
            RemoveActionsFromList(previousActions, "punch2");
        }
        if (ActionInQueue("jump"))
        {
            b.velocity = new Vector2(0, jumpForce * Time.deltaTime);
        }
        if (ActionInQueue("pickup"))
        {
            crosshair.gameObject.SetActive(false);
            if (currentlyTouchedPickup.GetComponent<Throwable>().enabled)
            {
                pickUpState = pickupStates.pickingUp;
            }
        }
        if (ActionInQueue("startFlight"))
        {
            isFlying = true;
            ani.SetBool("flying", true);
        }
        if (ActionInQueue("flightDeactivate",false))
        {
            if (previousActions.Contains("flightDeactivate"))
            {                
                isFlying = false;
                b.useGravity = true;
                ani.SetBool("flying", false);
            }
            StoreAsPrevious("flightDeactivate");
        }
    }
    IEnumerator ClearActionAfterSeconds(string action,float time)
    {
        if (previousActions.Contains(action))
        {
            yield return new WaitForSeconds(time);
            previousActions.Remove(action);
        }    
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


    void TogglePhysics(GameObject go,bool toggleState)
    { 
        if(go.TryGetComponent(out Collider collider))
        {
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                c.enabled = toggleState;
            }
        }
        if(go.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = !toggleState;
            rb.useGravity = toggleState;
        }
    }

    void PickUp()
    {
        if (currentlyTouchedPickup != null)
        {
            Throwable t = currentlyTouchedPickup.GetComponent<Throwable>();
            if (t.enabled)
            {               
                //disabling physics
                TogglePhysics(currentlyTouchedPickup, false);
                //setting animator boolean
                ani.SetBool("carrying", !t.isLightObject);
                elapsedThrowTime += Time.deltaTime;
                float elapsedPercentage = elapsedThrowTime / pickupTime;
                currentlyTouchedPickup.transform.SetParent(holdPosition.transform, true);
                currentlyTouchedPickup.transform.position = Vector3.Lerp(currentlyTouchedPickup.transform.position, holdPosition.position, elapsedPercentage);
                currentlyTouchedPickup.transform.localRotation = Quaternion.Lerp(currentlyTouchedPickup.transform.localRotation, Quaternion.Euler(t.holdRotation.x, t.holdRotation.y, t.holdRotation.z), elapsedPercentage);
                if (elapsedPercentage >= 1)
                {
                    pickUpState = pickupStates.holding;
                    currentlyTouchedPickup.transform.SetParent(holdPosition.transform, true);
                }
            }           
        }
        else
        {
            pickUpState = pickupStates.notholding;
        }
    }

    void Drop()
    {
        GameObject ob = currentlyTouchedPickup;
        Throwable ts = ob.GetComponent<Throwable>();

        TogglePhysics(ob, true);
        pickUpState = pickupStates.notholding;
        ob.transform.SetParent(ts.originalParent);
        ani.SetBool("carrying", false);
        ts.beingHeld = false;
        if (ob.TryGetComponent(out Ragdoll rs))
        {
            rs.StartRagdoll();
        }
    }

    void Throw()
    {
        Drop();
        GameObject ob = currentlyTouchedPickup;
        Throwable ts = ob.GetComponent<Throwable>();
        ts.beenThrown = true;
        foreach ( Rigidbody rb in ob.GetComponentsInChildren<Rigidbody>())
        {
            float throwForce = rb.mass * throwAccelaration;
            rb.AddForce(cam.forward * throwForce, ForceMode.Impulse);
        }
        ts.incrementVelocity(cam.forward*throwAccelaration);
        ani.Play("Throw");  
    }





    Vector3 LaserVision(Transform eyeball,LineRenderer laser)
    {
        laser.SetPosition(0, eyeball.position);

        //laser raycast
        Vector3 direction = cam.rotation * Vector3.forward + directionOffset;
        bool hit = Physics.Raycast(eyeball.position, direction.normalized, out RaycastHit ray, maxDistance, LaserLayers);

        
        //when firing laser
        if (Input.GetAxis("Fire1") > 0.4f)
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
    }//despawning old laser effects


    public void Die()
    {
        crosshair.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (TryGetComponent(out Ragdoll rd))
        {
            rd.StartRagdoll();
        }
        else
        {
            ani.SetBool("dead", true);
        }
        disableInputs = true;
        laser1.enabled = false;
        laser2.enabled = false;
        GameObject blood = Instantiate(bloodEffect,bloodSpawn.position,bloodSpawn.rotation,bloodSpawn);
        deathScreen.SetActive(true);
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
        if(energy < 0)
        {
            energy = 0;
        }
    }

    public void EnergyRegen()
    {
        energy += (Time.deltaTime * currentRegenRate);
        if(energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    void RotatePlayerToCam()
    {
        Vector3 direction = cam.rotation * Vector3.forward + directionOffset;
        if (Vector3.Angle(head.forward, direction) > angleDiff)
        {
            guy.rotation = Quaternion.Slerp(guy.rotation, Quaternion.Euler(0, cam.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
        }
    }

    bool NotHolding()
    {
        return pickUpState == pickupStates.notholding;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent( out Throwable t) && NotHolding())
        {
            if (t.enabled)
            {
                currentlyTouchedPickup = t.gameObject;
            }       
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Throwable t) && NotHolding())
        {
            currentlyTouchedPickup = null;
        }
    }
}
