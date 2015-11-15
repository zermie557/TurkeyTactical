using UnityEngine;
using System.Collections;

public class HeadShot : MonoBehaviour
{
	public float headVelocity = 80f;
	bool isCharging = false;
		
	PlayerControl playCntrl;
		

	// Use this for initialization
	void Awake ()
	{
		playCntrl = GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{	
		//If(headShotUnlocked)	
		if(Input.GetAxis ("FirePri_1") < 0.01)
			isCharging = true;
		
		if(Input.GetAxis ("FirePri_1") > 0.2 && isCharging && HeadToBodyDistance () < 3.85)
		{
			HeadLaunch ();
			isCharging = false;
		}
		
	}
	
	void HeadLaunch()
	{		
		float x = Input.GetAxis("X Aim_1");
		float y = Input.GetAxis ("Y Aim_1");
		
		if(Mathf.Abs (x) < 0.25f && Mathf.Abs(y) < 0.25f)
			return;
		
		float angle = Mathf.Atan2(y, x);
		
		float xVel = Mathf.Cos (angle);
		float yVel = Mathf.Sin (angle);
		
		GetComponent<Rigidbody2D>().velocity = new Vector2( headVelocity * xVel, headVelocity * yVel );
	}
	
	void Flip()
	{
		playCntrl.facingRight = !playCntrl.facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	public float HeadToBodyDistance()
	{
		Transform playerBody = GameObject.Find ("PlayerBody").transform;
		
		Vector3 bodyPosition = transform.InverseTransformPoint (playerBody.position);
		float bodyXPosition = bodyPosition.x;
		float bodyYPosition = bodyPosition.y;
		
		return Mathf.Sqrt ((bodyXPosition * bodyXPosition) + (bodyYPosition * bodyYPosition));
	}
}
