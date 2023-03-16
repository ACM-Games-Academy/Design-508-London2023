using UnityEngine;

public class Throwable : MonoBehaviour
{
    public Vector3 holdRotation;
    [Header("At what speed does the object deal damage")]
    [SerializeField] float damageVelocity;
    [SerializeField] bool printObjectVelocity;
    [Header("How much damage does it deal")]
    [SerializeField] float thrownDamage;
    [Header("What layers causes the object to destroy")]
    [SerializeField] LayerMask disableMask;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (printObjectVelocity)
        {
            print(gameObject.name + " velocity: " + rb.velocity.magnitude);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject colOb = collision.gameObject;
        if (rb.velocity.magnitude > damageVelocity)
        {
            if (disableMask == (disableMask | 1 << colOb.layer))
            {
                if (TryGetComponent(out HealthManager health))
                {
                    health.Death();
                }
            }
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
