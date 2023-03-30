using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //input variables
    private float horizontal; //horizontal input
    private float vertical; //vertical input
    private bool jumpPress; //jump input
    private bool jumpKeyDown; //jump key is pressed down
    private bool grapplePress; //grapple input
    private bool grappleKeyDown; //grapple key pressed down
    private bool grappleKeyUp; //grapple key released
    
    //player stats
    //Horizontal Movement
    [SerializeField] private float maxSpeed = 12f; //max player speed
    [SerializeField] private float acceleration = 15f; //player acceleration
    [SerializeField] private float decceleration = 10f; //player decceleration
    [SerializeField] private float airAcceleration = 8f; //acceleration when airborne
    [SerializeField] private float airDecceleration = 2.0f; //decceleration when airborne
    //Jumping
    [SerializeField] private float jumpStrength = 30f; //strength of jump
    [SerializeField] private float wallJumpYStrength = 20f; //wall jump y force
    [SerializeField] private float wallJumpXStrength = 10f; //wall jump x force
    [SerializeField] private float grappleJumpStrength = 15f; //grapple jump force
    [SerializeField] private float wallJumpWindow = 0.1f; //window of time to input wall jump
    [SerializeField] private float jumpCut = 0.7f; //jump cut strength
    [SerializeField] private float jumpBuffer = 0.2f; //landing jump buffer time
    [SerializeField] private float jumpFallGravMult = 1.5f; //fall gravity multiplier when jumping
    [SerializeField] private float coyoteTime = 0.1f; //coyote time
    [SerializeField] private float apexAccelMultiplier = 3f; //jump apex acceleration multiplier
    [SerializeField] private float apexSpeedMultiplier = 1.2f; //apex maxspeed multiplier
    [SerializeField] private float apexModThreshold = 2.0f; //vertical speed threshold for apex modifiers
    //Falling
    [SerializeField] private float fallSpeed = 15f; //default max fall speed
    [SerializeField] private float fastFallSpeed = 20f; //fast fall speed
    [SerializeField] private float wallSlideSpeed = 10f; //wall slide speed
    //Grappling
    [SerializeField] private float grappleMaxDistance = 6f; //grappling hook max length
    [SerializeField] private float swingStrength = 10f; //swinging power
    [SerializeField] private float swingWindow = 0.2f; //distance to max grapple that swing control activates


    
    //logic variables
    //Player State
    private bool facingRight = true; //facing right or left
    private bool isJumping; //is player in the air from jump
    private bool jumpIsCut; //jump has been cut
    private bool isWallSliding; //player is wall sliding
    private bool lastWallRight; //direction of last wall player slid on
    private bool isGrappling; //player is grappling
    //Collision
    private bool isGrounded; //is player touching ground
    private bool isTouchingFront; //is front of player touching wall
    //Timers
    private float lastJumpTime; //time since last jump input
    private float lastGroundedTime; //time since last ground touch
    private float lastWallTime; //time since last wall touch
    private float lastJumpKeyDown; //time since jump key last went down
    private float lastGrappleTime; //time since grapple was last attached
    private float lastGrappleKeyDown; //time since grapple key last went down
    //Physics
    private float defaultGravity; //default player gravity
    private float currentFallSpeed; //players current max fallspeed
    
    //GameObjects
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform frontCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LineRenderer grappleLine;
    [SerializeField] private DistanceJoint2D grappleJoint;
    

    // Start is called before the first frame update
    void Start()
    {
        defaultGravity = rb.gravityScale; //save default gravity
        //set falling variables to negative
        fallSpeed *= -1;
        fastFallSpeed *= -1;
        wallSlideSpeed *= -1;
        currentFallSpeed = fallSpeed; //set default fall speed
        //disable grapple elements
        grappleLine.enabled = false;
        grappleJoint.enabled = false;
        //set max grapple distance
        grappleJoint.distance = grappleMaxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        //store horizontal player input
        horizontal = Input.GetAxisRaw("Horizontal");
        //store vertical player input
        vertical = Input.GetAxisRaw("Vertical");
        //store jump input
        jumpPress = Input.GetKey("space");
        //store jumpKeyDown input
        jumpKeyDown = Input.GetKeyDown("space");
        //store grapple input
        grapplePress = Input.GetKey(KeyCode.E);
        //store grappleKeyDown
        grappleKeyDown = Input.GetKeyDown(KeyCode.E);

        //update timer variables
        lastJumpTime += Time.deltaTime;
        lastGroundedTime += Time.deltaTime;
        lastJumpKeyDown += Time.deltaTime;
        lastWallTime += Time.deltaTime;
        lastGrappleTime += Time.deltaTime;
        lastGrappleKeyDown += Time.deltaTime;

        //reset input timer variables
        if (jumpPress) {lastJumpTime = 0;}
        if (jumpKeyDown) {lastJumpKeyDown = 0;}
        if (isGrappling) {lastGrappleTime = 0;}
        if (grappleKeyDown) {lastGrappleKeyDown = 0;}

        //update sprite left/right orientation
        Flip();


        //DEBUGGING
        //Debug.Log("jumpPress: " + jumpPress);
        //Debug.Log("isGrounded: " + isGrounded);
        //Debug.Log("isJumping: " + isJumping);
        //Debug.Log("JumpisCut: " + jumpIsCut);
        //Debug.Log("isTouchingFront: " + isTouchingFront);
        //Debug.Log("lastGrappleTime: " + lastGrappleTime);
        //Debug.Log("lastJumpKeyDown: " + lastJumpKeyDown);
        //if(jumpKeyDown) {Debug.Log("jumpKeyDown!");}

        //make sure fallSpeed is negative (for inspector edits)
        if(currentFallSpeed >= 0) {currentFallSpeed *= -1;}

    }


    private void FixedUpdate()
    {
        //update Grounded state
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        //update wall touch state
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, 0.2f, groundLayer);

        //clamp velocity Y to fallSpeed (max value is arbitrarily high)
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, currentFallSpeed, 1000));

        //reset jump variables when touching ground
        if (isGrounded) 
        {
            isJumping = false;
            jumpIsCut = false;
            lastGroundedTime = 0;
            //this fixes baby/super jump issues
            //-will probably cause its own issues later :\
            rb.velocity = new Vector2 (rb.velocity.x, 0);
        }

        //reset wall variables
        if (isTouchingFront)
        {
            lastWallTime = 0;

            //save orientation of wall (for wall jumping)
            lastWallRight = (transform.localScale.x > 0) ? true : false;
        }

        //cancel grapple
        if (!isGrappling)
        {
            //disable grapple components
            grappleJoint.enabled = false;
            grappleLine.enabled = false;
        }

        //enable player movement
        Run();
        Jump();
        WallSlide();
        WallJump();
        Grapple();

    }


    //flip player sprite left and right
    private void Flip()
    {
        //right now this is based solely on current player input
        //maybe change to be velocity based???? -yea
        if (facingRight && horizontal < 0 || !facingRight && horizontal > 0)
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    
    //check if player is grounded
    //TRY USING GETMASK IF THIS DOESNT WORK
    /*
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    */

    //Run Function
    //horizontal input manager
    private void Run()
    {
        //swinging
        //(only activates near max grapple distance)
        if (isGrappling && !isGrounded)
        {
            //player to grapple point vector
            Vector2 swingDirection = (grappleJoint.connectedAnchor - (Vector2)transform.position);
            //save player distance to grapple point
            float distance = swingDirection.magnitude;
            //normalize vector (store only direction)
            swingDirection = swingDirection.normalized;

            //TEST pushing player slightly away from grapple point
            //TRY THIS: instead of moving player, just increase max grapple distance over time
            //need to stop translating at max distance
            //TRY THIS: do something similar at swingdirection.y < 0
            //--have distance slowly decrease 
            if (swingDirection.y > 0 && distance < grappleMaxDistance)
            {
                transform.Translate(swingDirection * -1 * .1f);
            }
            

            //activate swing controls if player is near max grapple distance
            if (Mathf.Abs(distance) > grappleMaxDistance - swingWindow)
            {
                if (horizontal > 0)
                {
                    //vector perpendicular right to grapple orientation
                    swingDirection = new Vector2 (swingDirection.y, swingDirection.x * -1);
                }
                else if (horizontal < 0)
                {
                    //vector perpendicular left to grapple orientation
                    swingDirection = new Vector2 (swingDirection.y * -1, swingDirection.x);
                }
                else {swingDirection = new Vector2 (0, 0);}
            
                //add swing force perpendicular to grapple
                rb.AddForce(swingDirection * swingStrength, ForceMode2D.Force);
                return; //do not activate run if swinging
            }
        }



        //Player run
        //maxspeed in direction
        float targetSpeed = horizontal * maxSpeed;
        //if inputting in direction: accelerate, else deccelerate
        float accelRate;

        //use different accel/deccel values depending on if airborne
        if(isGrounded)
        {
            accelRate = (Mathf.Abs(targetSpeed) > .01f) ? acceleration : decceleration;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > .01f) ? airAcceleration : airDecceleration;
        }

        //modify speed/acceleration at apex of jump
        //MIGHT NEED TO CHANGE -running off platforms accelerates
        if(!isGrounded && Mathf.Abs(rb.velocity.y) < apexModThreshold)
        {
            accelRate *= apexAccelMultiplier;
            targetSpeed *= apexSpeedMultiplier;
        }

        //conserve momentum        
        if(Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
		{
			//Prevent any deceleration from happening, or in other words conserve current momentum
			accelRate = 0; 
		}
        

        //difference between target speed and current speed
        float speedDiff = targetSpeed - rb.velocity.x;
        //calculate x force to apply to player
        float movement = speedDiff * accelRate;

        //apply movement force to player rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }

    
    private void Jump()
    {
        //if player is grounded -jump
        //if (isGrounded && jumpPress && !isJumping)
        //if (lastGroundedTime < coyoteTime && jumpPress && !isJumping)
        if (lastGroundedTime < coyoteTime && lastJumpKeyDown < jumpBuffer && !isJumping)
        {
            //reset y speed
            //-makes coyote time consistent -might cause problems later
            rb.velocity = new Vector2 (rb.velocity.x, 0);
            //apply upward force
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            //Debug.Log("Jump at " + Time.realtimeSinceStartup);
            //player is jumping
            isJumping = true;
        }

        //grapple jump
        //USING WALLJUMPWINDOW RN -MIGHT NEED CHANGE
        if (!isGrounded && !isTouchingFront && lastGrappleTime < wallJumpWindow && lastJumpKeyDown < jumpBuffer)
        {

            //TRY THESE
            //TEST
            //isJumping = true;
            //rb.velocity = rb.velocity * 1.05f;

            //if y velocity is negative, set to 0
            //otherwise, add jumpForce to current y momentum
            //ensures minimum height gain
            if (rb.velocity.y < 0) 
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            //add grapple jump force
            rb.AddForce(Vector2.up * grappleJumpStrength, ForceMode2D.Impulse);
            //cancel grapple
            isGrappling = false;
        }


        //cut jump off early
        if (!jumpIsCut && !jumpPress && rb.velocity.y > 0 && isJumping)
        {
            //cut jump force --two different methods -addForce -set velocity
            //rb.AddForce(Vector2.down *rb.velocity.y * (1-jumpCut), ForceMode2D.Impulse);
            rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y * (1-jumpCut));
            jumpIsCut = true; //jump has been cut
        }

        //THIS NEEDS WORK
        //modify fall gravity when jumping
        if(isJumping && rb.velocity.y < 0)
        {
            rb.gravityScale = defaultGravity * jumpFallGravMult;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }


    //wall slide
    private void WallSlide()
    {
        //if player is airborne, touching a wall, and holding a direction -> wallslide
        if (!isGrounded && isTouchingFront && horizontal != 0)
        {
            //set wall sliding bool to true
            isWallSliding = true;
            //change max fall speed
            currentFallSpeed = wallSlideSpeed;

            //TRY THIS
            //rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            //set wall sliding bool to false
            isWallSliding = false;
            //change max fall speed
            currentFallSpeed = fallSpeed;
        }
    }



    private void WallJump()
    {
        //THIS NEEDS WORK 
        //-jumpbuffer makes things wonky,
        if(lastWallTime < wallJumpWindow && !isGrounded && lastJumpKeyDown < jumpBuffer)
        {
            Vector2 wallJumpForce;

            //reset speed
            rb.velocity = new Vector2 (0, 0);
            //set wall jump force to left or right depending on orientation
            if(lastWallRight)
            {
                wallJumpForce = new Vector2 (wallJumpXStrength * -1, wallJumpYStrength);
            }
            else
            {
                wallJumpForce = new Vector2 (wallJumpXStrength, wallJumpYStrength);
            }

            //THIS NEEDS WORK
            //-use perpendicular swing vectors instead of just x
            //remove y force if grappling
            if (isGrappling)
            {
                wallJumpForce = wallJumpForce * Vector2.right;
            }

            //apply wall jump force
            rb.AddForce(wallJumpForce, ForceMode2D.Impulse);

        }
    }


    //grappling hook
    private void Grapple()
    {
        //store player aim
        Vector2 aimDirection = new Vector2 (horizontal, vertical);
        //set line start position to player
        grappleLine.SetPosition(0, transform.position);

        //if grapple is pressed
        //USING WALL JUMP WINDOW -MIGHT NEED CHANGE
        if(lastGrappleKeyDown < wallJumpWindow && !isGrappling)
        {
            //cast a ray in grapple direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, grappleMaxDistance, groundLayer);

            if (hit.collider != null) //if grapple target is found
            {
                isGrappling = true; //player is grappling

                grappleJoint.enabled = true; //enable grapple joint
                grappleJoint.connectedAnchor = hit.point; //set grapple point

                grappleLine.enabled = true; //enable line
                grappleLine.SetPosition(1, hit.point); //set endpoint
            }

        }
        else if (!grapplePress) //end grapple
        {
            isGrappling = false; //reset isGrappling
            //grapple cancel performed in FixedUpdate
        }

    }

    private void Swing()
    {
        if (isGrappling && !isGrounded) 
        {
            //float swingForce = swingStrength * horizontal;
            //player to grapple point vector
            Vector2 swingDirection = (grappleJoint.connectedAnchor - (Vector2)transform.position).normalized;
            if (horizontal > 0)
            {
                //vector perpendicular right to grapple orientation
                swingDirection = new Vector2 (swingDirection.y, swingDirection.x * -1);
            }
            else if (horizontal < 0)
            {
                //vector perpendicular left to grapple orientation
                swingDirection = new Vector2 (swingDirection.y * -1, swingDirection.x);
            }
            
            rb.AddForce(swingDirection * swingStrength, ForceMode2D.Force);
        }
    }

//store highest speed variable, have it decay over time
//when jumping off grapple use variable to set speed
//so all x speed is not lost when swinging vertically
//OR multiply current x speed by multiplier when jumping off grapple
}
