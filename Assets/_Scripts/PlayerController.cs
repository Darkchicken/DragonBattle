using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //smoothing for leveling out turns
    public float smooth = 2.0F;
    float turnSmoothing = 30f;
    float initialZRotation = 0;
    Animator anim;
    
    /// <summary>
    /// Dragon Stats
    /// </summary>
    public float speed = 10f;
    public float turnSpeed = 60f;
    public float vertSpeed = 30f;
    float increment;
    //check if flight is inverted
    float invert = 1; //1 is regular, -1 is inverted
    Rigidbody rb;

    float distToGround = 0;
    bool grounded = true;
	// Use this for initialization
	void Start ()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        //get player object's rigidbody
        rb = GetComponent<Rigidbody>();
        //get animator of player's dragon model
        anim = GetComponentInChildren<Animator>();
        initialZRotation = transform.rotation.z;
        
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
        //check if player has activated an attack/ability
        GetAbilities();
        //animation handling
        Animate();
       

    } 
    public void GetMovement()
    {

       
           //movevec is transform.forward (magnitude of 1) * the axis (between 1 and -1)
           // + transform.right (magnitude of 1 )* the horizontal axis (between 1 and -1)
            Vector3 moveVec = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
            //get the amount to increment position by
            increment += speed / 100;
            //increment player position and lerp movement
            transform.position = Vector3.Lerp(transform.position, transform.position + moveVec, increment);
        
    }
    public void GetTurning()
    {
        if(grounded)
        {GetGroundTurning();}
        else if(!grounded)
        {GetAirTurning();}
    }
    public void GetGroundTurning()
    {
        Vector3 currentRotation = transform.eulerAngles;
        Vector3 newRotation = currentRotation;
        //handles horizontal turning
        float turnY = Input.GetAxis("Mouse X") * turnSpeed; 
        //allows dragon to look up and down (doesnt rotate dragon, just camera)
        float turnX = Input.GetAxis("Mouse Y") * vertSpeed;
        //approaches 1 climbing straight up, -1 diving straight down
        float dragonAngle = Vector3.Dot(transform.forward, Vector3.up);
     
       

        /**************
        Handle Steering in vertical direction
        ***************/
        //player steering up
        if (turnX < 0)
        {
            /*
            if (dragonAngle < 0.8)
            { newRotation.x += turnX; }
            */
        }
        //player steering down
        else if (turnX > 0)
        {
            /*
            if (dragonAngle > -0.8)
            { newRotation.x += turnX; }
            */
        }
        //no vertical steering
        else
        {
            // newRotation.x=Mathf.LerpAngle(currentRotation.x, 0, Time.deltaTime* turnSmoothing);// * smooth);

        }

        /**************
        Handle Steering in horizontal direction
        ***************/
        newRotation.y += turnY;


        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newRotation, Time.deltaTime * smooth);
    }
    public void GetAirTurning()
    {
       
        Vector3 currentRotation = transform.eulerAngles;
        Vector3 newRotation = currentRotation;
      
        float turnY = Input.GetAxis("Mouse X") * turnSpeed; //Input.GetAxis("Horizontal") * turnSpeed;//
        float turnX = Input.GetAxis("Mouse Y") * vertSpeed;
        //approaches 1 climbing straight up, -1 diving straight down
        float dragonAngle = Vector3.Dot(transform.forward, Vector3.up);
        //amount to tip dragon to bank turns (need to be moving and turning)
        float turnZ = Input.GetAxis("Mouse X") * Mathf.Abs(Input.GetAxis("Vertical")) *turnSpeed * -1;//-1 to invert turn
        //approaches 1   when level, 0 when sideways
        float dragonTilt = Vector3.Dot(transform.up, Vector3.up);

        /**************
        Handle Steering in vertical direction
        ***************/
        //player steering up
        if (turnX < 0)
        {
            if (dragonAngle < 0.8)
            {newRotation.x += turnX;}    
        }
        //player steering down
        else if (turnX > 0)
        {
            if (dragonAngle > -0.8)
            {newRotation.x += turnX;}
        }
        //no vertical steering
        else
        {
           // newRotation.x=Mathf.LerpAngle(currentRotation.x, 0, Time.deltaTime* turnSmoothing);// * smooth);
            
        }

        /**************
        Handle Steering in horizontal direction
        ***************/
        newRotation.y += turnY;

        /**************
        Handle Banking with horizontal steering
        ***************/
        //if the player is moving and turning
        if (turnZ != 0)
        {
            //as long as the player hasnt tilted further than halfway
            if (dragonTilt > 0.4)
            {
                //add to z rotation
                newRotation.z += turnZ;
            }
        }
        //if the player is no longer moving and turning
        else
        {
            newRotation.z = Mathf.LerpAngle(currentRotation.z, 0, Time.deltaTime* turnSmoothing); 
        }

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newRotation, Time.deltaTime * smooth);

    }
    public void GetAbilities()
    {
        if(Input.GetAxis("RightTrigger") > 0.5)
        {
            anim.SetBool("BreatheFire", true);
        }
        else
        {
            anim.SetBool("BreatheFire", false);
        }
    }
    public void Animate()
    {
        //A button on remote (handles taking off and landing)
        if (Input.GetButtonDown("Fire1"))
        {
            //if the dragon is walking
            if (grounded)
            {
                //play flying animation
                anim.SetBool("Airborne", true);
                grounded = false;
                rb.useGravity = false;
                rb.angularVelocity.Set(0, 0, 0);
                rb.constraints = RigidbodyConstraints.None;
            }
            //if the dragon is flying and near the ground
            else if (!grounded && IsGrounded())
            {
                //play walking animation
                anim.SetBool("Airborne", false);
                grounded = true;
                rb.useGravity = true;
                rb.angularVelocity.Set(0, 0, 0);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            }
        }
        //set velocity variable to vertical axis
        anim.SetFloat("Velocity", Input.GetAxis("Vertical"));
        if(Input.GetAxis("Vertical") == 0)
        {
            anim.SetBool("Moving", false);
        }
        else
        {
            anim.SetBool("Moving", true);
        }
       

    }

    //Creates a raycast to check if the player is near the ground (to prep for landing)
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }

    public void InvertFlight()
    {
        //swap sign of invert
        invert *= -1;
    }
}
