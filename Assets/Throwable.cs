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
        if(thrown && collision.gameObject != gameObject)
        {
            if(TryGetComponent(out HealthManager health))
            {
                health.Death();
            }
        }
    }
}
