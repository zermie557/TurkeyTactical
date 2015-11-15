using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
	//Variables for standing up straight
	float correctRange;
	float rotateSpeed = 10f;
	float fixAngleVal = 0.01f;
	
	//Variables for left and right movement
	public float maxSpeed = 10f;
	public float maxRunAccel = 25f;
	public float maxAirAccel = 10f;
	public float punchSpeed = 2f;
	public bool facingRight = true;
	float move;
	
	
	Transform playerHead;
	
	//Variables for checking for ground
	public bool grounded;
	Transform groundCheck;
	float groundRadius = 0.45f;
	public LayerMask whatIsGround;
	
	//Hanging variables
	public bool canHang = false;
	Transform hangCheck;
	float hangRadius = 0.55f;
	public bool isHanging = false;
	public LayerMask whatIsHangable;
	float defSpringDist = 1.125f;		//remember to update these as the values are updated in SpingJoint2D of PlayerHead
	float defSpringFreq = 2f;			// " " "
	public float hangDist = 2.5f;
	public float hangDamp = 0.5f;
	public float hangFreq = 3f;
	public float swingDistAdj = 0.15f;
	public float swingForce = 20f;
	
	
	//Variables for jumping
	public float jumpForce = 500f;
	int jumpNumber = 0;
	int possibleJumps = 2;
	
	//Variables for x,y coordinates of player head to player body
	public float globalAngleDeg;
	
	//Variable for calling animator
	Animator anim;

	
	
	void Start ()
	{
		anim = GetComponent<Animator>();
		groundCheck = GameObject.FindGameObjectWithTag ("GroundCheck").transform;
		hangCheck = GameObject.Find ("HangCheck").transform;
		playerHead = GameObject.Find ("PlayerHead").transform;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{		
		HorizontalVelocityCap ();
		Movement ();
		Hang ();
		
		correctRange = Mathf.Abs ( transform.rotation.z );
		
		JumpCounter();
		
		Jumping();
		
	}
	
	void Update ()
	{
		//Set animator controller Facing Right value
		anim.SetBool ("Facing Right", facingRight);
		
		//Checks if the player is on the ground
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
		//Check if the player can hang
		canHang = Physics2D.OverlapCircle(hangCheck.position, hangRadius, whatIsHangable);
	}
	
	
	// ######## Method Definitions #######
	
	//Cap for velocity
	void HorizontalVelocityCap()
	{
		Rigidbody2D playerBod = GetComponent<Rigidbody2D>();
		//Get horizontal velocity
		float horiVel = playerBod.velocity.x;
		//Cap the players horizontal velocity
		playerBod.velocity = new Vector2(Mathf.Clamp (horiVel, -maxSpeed, maxSpeed), playerBod.velocity.y);
	}
	
	//Method for movement
	void Movement()
	{
		move = Input.GetAxis("Horizontal_1");
		
		if(!isHanging)
		{
			if(move > 0.1 || move < -0.1)
				GetComponent<Rigidbody2D>().AddForce (new Vector2 (maxRunAccel * move, 0));
		}
		else if(isHanging /*&& Boolean for unlocking this ability*/)
		{
			ApplySwingForce ();
		}
		
		if(grounded && !isHanging)
		{
			if( move > 0 && !facingRight)
				facingRight = !facingRight;
			else if( move < 0 && facingRight)
				facingRight = !facingRight;
		}
	}
	
	//Method for flipping character sprite
	void Flip()
	{
		facingRight = !facingRight;
		//Vector3 theScale = transform.localScale;
		//theScale.x *= -1;
		//transform.localScale = theScale;
		//Vector3 headScale = playerHead.localScale;
		//headScale.x *= -1;
		//GameObject.Find ("PlayerHead").transform.localScale = headScale;
	}
	
	//Method to determine if possible number of jumps is reached
	void JumpCounter()
	{
		if(Input.GetButtonDown("Jump_1"))
			jumpNumber++;
		
		if(grounded)
			jumpNumber = 0;
	}
	
	//Method for jumping
	void Jumping()
	{
		if ( ( grounded || jumpNumber < possibleJumps ) && Input.GetButtonDown("Jump_1") )
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2( GetComponent<Rigidbody2D>().velocity.x, 0);
			GetComponent<Rigidbody2D>().AddForce (new Vector2( 0, jumpForce));
		}
	}
	
	//Method for hanging
	void Hang()
	{	
		SpringJoint2D neckSpring = playerHead.GetComponent<SpringJoint2D>();
		Rigidbody2D headRigBod = playerHead.GetComponent<Rigidbody2D>();
		
		if(isHanging)
		{
			//Rotate player's body
			//transform.eulerAngles = new Vector3(0, 0, globalAngleDeg);
			
			//Adjust frequency so spring is stiff
			neckSpring.frequency = hangFreq;
			
			//If action buttun is pressed, zip to default distance
			if(Input.GetButton("Action"))
				neckSpring.distance = hangDist;
				
			//If the action button is not pressed, can increase and decrease spring distance
			if(Input.GetAxis ("Vertical") > 0.5)
				neckSpring.distance -= swingDistAdj;
			else if(Input.GetAxis ("Vertical") < -0.5 && neckSpring.distance <= 8)
				neckSpring.distance += swingDistAdj;
		} else
		{
			neckSpring.distance = defSpringDist;
			neckSpring.frequency = defSpringFreq;
		}
		
		//If the player can hang and the hang trigger was held long enough, then hang.	
		if(canHang && Input.GetAxis("FirePri_1") == 1)
			{
				headRigBod.isKinematic = true;		
				isHanging = true;
			}
		else
			{
				
				headRigBod.isKinematic = false;
				
				if(transform.rotation.z != 0 && !isHanging)
				{
					if( correctRange < fixAngleVal )
					{
						transform.rotation = Quaternion.identity;
					}
													 
					Quaternion q = Quaternion.AngleAxis(0, Vector3.forward);
					transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
					
				}
				
				isHanging = false;
			}
	}
	
	//Method for applying acceleration while hanging
	void ApplySwingForce()
		{
			Vector3 headToBody = transform.InverseTransformPoint (playerHead.position);
			float xHeadToBody = headToBody.x;
			float yHeadToBody = headToBody.y;
					
			//Return angles and normalize vectors
			float angle = Mathf.Atan2(yHeadToBody, xHeadToBody);
			float globalAngle = angle - (Mathf.PI/2);
			float xDirection = Mathf.Cos (globalAngle);
			float yDirection = Mathf.Sin (globalAngle);
			globalAngleDeg = globalAngle * Mathf.Rad2Deg;
					
			//Apply force
			if(globalAngleDeg > -90)
			{
				if(move > 0.5)
					GetComponent<Rigidbody2D>().AddForce ( new Vector2( xDirection * swingForce, yDirection * swingForce ));
				else if(move < -0.5)
					GetComponent<Rigidbody2D>().AddForce ( new Vector2( xDirection * ( swingForce * -1 ), yDirection * ( swingForce * -1 )));
			}
		}
	
			
}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	

