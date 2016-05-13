﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //smoothing for leveling out turns
    public float smooth = 2.0F;
    float initialZRotation = 0;
    Animator anim;
    
    public float speed = 5f;
    public float turnSpeed = 60f;
    public float vertSpeed = 30f;
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
       
       
        //animation handling
        Animate();
       

    } 
    public void GetMovement()
    {

       
           //movevec is transform.forward (magnitude of 1) * the axis (between 1 and -1)
            Vector3 moveVec = transform.forward * Input.GetAxis("Vertical");
            //get the amount to increment position by
            increment += speed / 100;
            //increment player position and lerp movement
            transform.position = Vector3.Lerp(transform.position, transform.position + moveVec, increment);
        
    }
    public void GetTurning()
    {
        Vector3 currentRotation = transform.eulerAngles;
        Vector3 newRotation = currentRotation;
       
        // float turnAmount = Input.GetAxis("Horizontal") * turnSpeed;
        float turnY = Input.GetAxis("Mouse X") * turnSpeed;
        float turnX = Input.GetAxis("Mouse Y") * vertSpeed;
        //approaches 1 climbing straight up, -1 diving straight down
        float dragonAngle = Vector3.Dot(transform.forward, Vector3.up);
     
        //player steering up
        if(turnX < 0)
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
            //newRotation.x = 0;
        }
       

        newRotation.y += turnY;
        //newRotation.z = 0;//.eulerAngles.z = 0;
        //if not turning up or down
        /*
        if(turnX == 0)
        {
            newRotation.x = 0;
        }
        */
       
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newRotation, Time.deltaTime * smooth);
        
       






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
