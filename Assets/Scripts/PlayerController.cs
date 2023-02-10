using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float roticerySpeed;
    [SerializeField] LayerMask switchCastHits;
    List<GameObject> touchingMe = new List<GameObject>();//list of Player Controllers that are touching a disabled Player Controller
    bool touchingYou;
    GameObject friend;
    public bool disabled;

    // Start is called before the first frame update

    void Start()
    {
        if (!disabled)
        {
            GameManager.instance.playerNumber += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!disabled)
        {
            if (Input.GetKey("d"))
            {
                //dtransform.rotation = Quaternion.Euler(Mathf.LerpAngle(transform.rotation.x, 0, 0.01f), Mathf.LerpAngle(transform.rotation.y, 0, 0.01f), transform.rotation.z);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, 0, -roticerySpeed);
            }
            if (Input.GetKey("a"))
            {
                GetComponent<Rigidbody>().angularVelocity = new Vector3(GetComponent<Rigidbody>().angularVelocity.x, 0, roticerySpeed);
            }
            if (Input.GetKey("w"))
            {
                GetComponent<Rigidbody>().angularVelocity = new Vector3(roticerySpeed, 0, GetComponent<Rigidbody>().angularVelocity.z);
            }
            if (Input.GetKey("s"))
            {
                GetComponent<Rigidbody>().angularVelocity = new Vector3(-roticerySpeed, 0, GetComponent<Rigidbody>().angularVelocity.z);
            }
            if (touchingYou && Input.GetKeyDown("e"))
            {
                friend.tag = "Player";
                friend.GetComponent<PlayerController>().disabled = false;
                foreach (Renderer rend in friend.GetComponentsInChildren<Renderer>())
                {
                    rend.material = GameManager.instance.activePlayer;
                }
                gameObject.tag = "Inactive";
                disabled = true;
                touchingMe = new List<GameObject>();
                touchingYou = false;
                friend = null;
                foreach (Renderer rend in GetComponentsInChildren<Renderer>())
                {
                    rend.material = GameManager.instance.inActivePlayer;
                }
            }
        }
    }



    /*void OnMouseDrag()
    {
        float XaxisRotation = Input.GetAxis("Mouse X") * 3 * roticerySpeed;
        float YaxisRotation = Input.GetAxis("Mouse Y") * 3 * roticerySpeed;
        // select the axis by which you want to rotate the GameObject
        transform.RotateAround(Vector3.back, XaxisRotation);
        transform.RotateAround(Vector3.right, YaxisRotation);
        //transform.RotateAround(Vector3.right, YaxisRotation);
    }*/


    private void OnTriggerStay(Collider other)
    {
        GameObject obby = other.gameObject;
        PlayerController friendController = obby.GetComponent<PlayerController>();

        //RAYCAST
        RaycastHit hit;
        bool wallBlock = Physics.Raycast(transform.position, (obby.transform.position - transform.position).normalized, out hit, Mathf.Infinity,switchCastHits);
        Debug.DrawRay(transform.position, (obby.transform.position - transform.position).normalized * hit.distance, Color.red); 
        if (obby.tag == "Inactive" && !disabled && !GameManager.instance.won && friendController.touchingMe.Count < 1)
        {
            print(hit.collider.gameObject);
            //IF THE RAYCAST WAS ABLE TO COLLIDE
            if (hit.collider.gameObject == obby)
            {
                touchingYou = true;
                GameManager.instance.switchPrompt.SetActive(touchingYou);
                friend = obby;
                friendController.touchingMe.Add(gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject obby = other.gameObject;
        PlayerController friendController = obby.GetComponent<PlayerController>();
        if (obby.tag == "Inactive" && !disabled && !GameManager.instance.won)
        {
            touchingYou = false;
            GameManager.instance.switchPrompt.SetActive(touchingYou);
            friend = null;
            friendController.touchingMe.Remove(gameObject);
        }
    }

}
