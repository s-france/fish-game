using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    //State Booleans
    bool groundState;
    bool jumpState;
    bool grappleState;

    //Player collision state [touchingLeft, touchingRight]
    int[] collisionState;



    // Start is called before the first frame update
    void Start()
    {
        //get player gameobject!!!
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Update Player State
    void StateUpdate()
    {
        if (isGrounded())
        {
            groundState = true;
        }

        if (isJumping())
        {
            jumpState = true;
        }

        if (isGrappling())
        {
            grappleState = true;
        }
    }


    ///Player State Check Functions:

    //check Ground/Air State (also bottom collision state)
    bool isGrounded()
    {

    }

    //check Jump/Fall State
    bool isJumping()
    {

    }

    //check Grappling States
    bool isGrappling()
    {

    }





}
