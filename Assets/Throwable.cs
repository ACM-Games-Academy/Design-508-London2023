using UnityEngine;

public class Throwable : MonoBehaviour
{
    public Vector3 holdRotation;
    public bool thrown;
    [SerializeField] LayerMask destroyMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // layermask == (layermask | (1 << layer))
        if (thrown && destroyMask ==  (destroyMask | 1 << collision.gameObject.layer))
        {
            if(TryGetComponent(out HealthManager health))
            {
                health.Death();
            }
        }
    }
}
