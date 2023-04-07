using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //player stats
    
    //Horizontal Movement
    public float maxSpeed = 12f; //max player speed
    public float acceleration = 15f; //player acceleration
    public float decceleration = 10f; //player decceleration
    public float airAcceleration = 8f; //acceleration when airborne
    public float airDecceleration = 2.0f; //decceleration when airborne
    
    //Jumping
    public float jumpStrength = 30f; //strength of jump
    public float wallJumpYStrength = 20f; //wall jump y force
    public float wallJumpXStrength = 10f; //wall jump x force
    public float grappleJumpStrength = 15f; //grapple jump force
    public float wallJumpWindow = 0.1f; //window of time to input wall jump
    public float jumpCut = 0.7f; //jump cut strength
    public float jumpBuffer = 0.2f; //landing jump buffer time
    public float jumpFallGravMult = 1.5f; //fall gravity multiplier when jumping
    public float coyoteTime = 0.1f; //coyote time
    public float apexAccelMultiplier = 3f; //jump apex acceleration multiplier
    public float apexSpeedMultiplier = 1.2f; //apex maxspeed multiplier
    public float apexModThreshold = 2.0f; //vertical speed threshold for apex modifiers
    
    //Falling
    public float fallSpeed = 15f; //default max fall speed
    public float fastFallSpeed = 20f; //fast fall speed
    public float wallSlideSpeed = 10f; //wall slide speed
    
    //Grappling
    public float grappleMaxDistance = 6f; //grappling hook max length
    public float swingStrength = 10f; //swinging power
    public float swingWindow = 0.2f; //distance to max grapple that swing control activates




    // Start is called before the first frame update
    void Start()
    {
        
    }

}
