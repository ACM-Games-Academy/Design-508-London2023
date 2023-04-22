using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Explodable : MonoBehaviour
{
    [Header("Visuals:")]
    [SerializeField] GameObject explosion;
    [Header("Explosion Properties:")]
    [SerializeField] float blastDamage;
    public float force;
    [SerializeField] float upwardModifier;
    [SerializeField] float affectedRadius;
    [SerializeField] bool showRadiusInEditor;
    [SerializeField] Vector3 radiusOffset;
    [SerializeField] LayerMask affectedLayers;
    enum modes { destroy, disable, stayActive}
    [SerializeField] modes destructionMode;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDrawGizmos()
    {
        if (showRadiusInEditor)
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawSphere(transform.position + radiusOffset, affectedRadius);
        }
    }

    private void Update()
    {

    }
    public void Explode()
    {
        GameObject blast = Instantiate(explosion);
        blast.transform.position = transform.position;
        if(TryGetComponent(out Breakable brk))
        {
            brk.BreakIt();
        }
        if(destructionMode == modes.destroy)
        {
            Destroy(gameObject);
        }
        else if(destructionMode == modes.disable)
        {
            gameObject.SetActive(false);
        }
        Collider[] nearby = Physics.OverlapSphere(transform.position + radiusOffset, affectedRadius, affectedLayers);
        foreach (Collider col in nearby)
        {
            if (col.TryGetComponent(out HealthManager health))
            {
                health.HealthChange(-blastDamage);
            }
            if (col.TryGetComponent(out Ragdoll rd) && !col.TryGetComponent(out PlayerController pc))
            {
                rd.StartRagdoll();
                if (rd.GetComponentInParent<HealthManager>() != null)
                {
                    rd.GetComponentInParent<HealthManager>().HealthChange(-blastDamage);
                }
            }
            if (col.TryGetComponent(out Rigidbody rb))
            {
                ExplosionForce(rb);
            }
        }
    }

    public void ExplosionForce(Rigidbody rb)
    {
        rb.AddExplosionForce(force * Time.deltaTime, transform.position + radiusOffset, affectedRadius,upwardModifier,ForceMode.Impulse);
    }
}
