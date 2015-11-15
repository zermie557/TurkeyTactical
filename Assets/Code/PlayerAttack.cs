using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

	Transform attackCheck;
	public float attackRadius = 0.5f;
	public float attackForce = 25f;
	public bool canPunch;
	public bool isPunching;				//Keep public
	float punchResetTime = 0.25f;
	public float punchTimerStart = 0;	//Makeprivate
	public float punchTimer;			//MakePrivate
	public bool punchTimerOn = false;	//MakePrivate
	public LayerMask whatIsPunchable;
	
	PlayerControl playerCtrl;
	public bool facingRight;
	
	// Use this for initialization
	void Awake ()
	{
		attackCheck = GameObject.FindGameObjectWithTag ("AttackCheck").transform;
		playerCtrl = GetComponent<PlayerControl>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		FlipAttackCheck();
		canPunch = Physics2D.OverlapCircle (attackCheck.position, attackRadius, whatIsPunchable);
		Punch ();
	}
	
	void FixedUpdate ()
	{
	
	}
	
	void Punch()
	{		
		if(canPunch && Input.GetButtonUp ("Punch") && !punchTimerOn)
		{
			isPunching = true;
			punchTimerStart = Time.time;
			punchTimerOn = true;
			canPunch = false;
		}
		
		punchTimer = Time.time - punchTimerStart;
		
		if(isPunching && (punchResetTime < (punchTimer)) && punchTimerOn)
		{
			isPunching = false;
			punchTimerOn = false;
		}
	}
	
	void FlipAttackCheck()
	{
		facingRight = playerCtrl.facingRight;
		
		//Debug.Log (attackCheck.position.x);
		
		if((facingRight && attackCheck.localPosition.x < 0) || (!facingRight && attackCheck.localPosition.x > 0))
		{
			Vector3 thePosition = attackCheck.localPosition;
			thePosition.x *= -1;
			attackCheck.localPosition = thePosition;
		}
	}
}
