using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Throwable : Freezable
{
    [Tooltip("Rotation offset while being held")]
    public Vector3 holdRotation;

    [Header("Dealing Damage")]
    [Tooltip("At what speed does this object inflict damage?")]
    [SerializeField] float damageVelocity;
    [Tooltip("Print the velocity as a reference for the above variable")]
    [SerializeField] bool printObjectVelocity;
    [Tooltip("How much damage does it deal")]
    [SerializeField] float thrownDamage;
    [HideInInspector] public bool beenThrown;

    [Header("Taking Damage")]
    [Tooltip("What layers cause the object to take damage?")]
    [SerializeField] LayerMask takesDamageMask;
    [Tooltip("How much damage does the object take upon impact?")]
    [SerializeField] float impactHealthLoss;
    [SerializeField] bool useDraggedInHealthManager;
    [SerializeField] HealthManager draggedInHM;
    Rigidbody rb;
    public bool isLightObject;

    [Header("AUTOFILLED DO NOT CHANGE")]
    public bool beingHeld;  
    public Transform originalParent;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalParent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (printObjectVelocity)
        {
            print(gameObject.name + " Velocity: " + rb.velocity.magnitude);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject colOb = collision.gameObject;
        if (beenThrown && rb.velocity.magnitude > damageVelocity && colOb.transform.root != transform.root)//invalid if colliding objects are part of the same root object
        {
            if (takesDamageMask == (takesDamageMask | 1 << colOb.layer))
            {
                HealthManager hm = null;
                if (useDraggedInHealthManager)
                {
                    hm = draggedInHM;
                }
                else if (TryGetComponent(out HealthManager health))
                {
                    hm = health;
                }
                if(hm != null)
                {
                    print(gameObject.name+" collided with "+colOb.name + "at speed "+rb.velocity.magnitude);
                    hm.HealthChange(-impactHealthLoss);
                }               
            }
            if (colOb.layer != LayerMask.NameToLayer("Player"))
            {
                if (colOb.TryGetComponent(out HealthManager hlth))
                {
                    hlth.HealthChange(-thrownDamage);
                }
                if (colOb.TryGetComponent(out Ragdoll rd))
                {
                    rd.StartRagdoll();
                }
            }

        }
    }
}
