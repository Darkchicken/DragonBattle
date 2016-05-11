using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float smooth = 2.0F;
    //amount dragon should tilt for banking, climbing, and diving
    public float tiltAngle = 30.0F;
    //how far to turn left and right
    public float turnAngle = 60.0F;
    Animator anim;
    
    public float speed = 3f;
    public float turnSpeed = 0.15f;
    float increment;
    //check if flight is inverted
    float invert = 1; //1 is regular, -1 is inverted
    Rigidbody rb;
	// Use this for initialization
	void Start ()
    {
        //get player object's rigidbody
        rb = GetComponent<Rigidbody>();
        //get animator of player's dragon model
        anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //toggle flight inversion
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InvertFlight();
        }
    }
    void FixedUpdate()
    {
        //check if player is moving 
        GetMovement();
        //check if player is turning
        GetTurning();
       
       
        //animation handling
        Animate();
       

    } 
    public void GetMovement()
    {
        //Spacebar by default will make it move forward 
        if (Input.GetButton("Jump"))
        {
            //THE DOT OF TWO PERP VECTORS IS 0
            //ADD 2 VECTORS TO GET THE VEC BETWEEN THEM
            //gives a vector facing right mag of 1
            Vector3 side = transform.right * Input.GetAxis("Horizontal");
            //forward mag of 1
            Vector3 forward = transform.forward/Input.GetAxis("Horizontal");
            //gives a vector exactly 45 degrees
            Vector3 turnVec = (transform.forward + transform.right* Input.GetAxis("Horizontal")).normalized;
           
            //get the amount to increment position by
            increment += speed / 100;
            //increment player position and lerp movement
            //Debug.Log(turnAmount);
            //Debug.Log(moveDir);
            transform.position = Vector3.Lerp(transform.position, transform.position + turnVec, increment);//movedir was transform.forward
        }
    }
    public void GetTurning()
    {
        //get horizontal tilt (banks dragon while turning)
        float tiltAroundZ = -Input.GetAxis("Horizontal") * tiltAngle;
        //physically turns dragon right and left
        float tiltAroundY = -Input.GetAxis("Horizontal") * turnAngle;
        //get vertical tilt (dragon climbs and dives)
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle * invert;
        //create a new quaternion for rotation amount
        Quaternion target = Quaternion.Euler(tiltAroundX, tiltAroundY, tiltAroundZ);
        //apply rotation to transform
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        
    }
    public void Animate()
    {
        if (rb.velocity.magnitude > 0)
        {
            anim.SetBool("flyGlide", true);
        }
        else
        {
            anim.SetBool("flyGlide", false);
        }
       
    }
    public void InvertFlight()
    {
        //swap sign of invert
        invert *= -1;
    }
}
