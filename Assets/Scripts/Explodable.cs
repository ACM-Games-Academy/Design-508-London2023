using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Explodable : MonoBehaviour
{
    [Header("Visuals:")]
    [SerializeField] GameObject explosion;
    [Header("Physical Properties:")]
    [SerializeField] float explosionDamage;
    [SerializeField] float explosionForce;
    [SerializeField] float affectedRadius;
    [SerializeField] LayerMask affectedLayers;
    bool playMode;

    // Start is called before the first frame update
    void Start()
    {
        playMode = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, affectedRadius);
    }

    private void Update()
    {

    }
    public void Explode()
    {
        GameObject blast = Instantiate(explosion);
        blast.transform.position = transform.position;
        Destroy(gameObject);

        Collider[] nearby = Physics.OverlapSphere(transform.position, affectedRadius, affectedLayers);
        foreach (Collider col in nearby)
        {
            if (col.TryGetComponent(out HealthManager health))
            {
                health.HealthChange(-explosionDamage);
            }
            if (col.TryGetComponent(out Rigidbody rb))
            {
                print("a");
                print(rb.gameObject.name);
                rb.AddExplosionForce(explosionForce*Time.deltaTime, transform.position, affectedRadius);
            }
        }
    }
}
