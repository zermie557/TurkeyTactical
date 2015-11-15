using UnityEngine;
using System.Collections;

public class Mahp : MonoBehaviour {
	
	public int hitPoints = 20;
	public float attackForce;
	
	PlayerAttack playerAttack;
	public bool isHit;
	bool canBePunched;
	Transform playerTransform;
	
	
	void Awake()
	{
		playerAttack = GameObject.Find("PlayerBody").GetComponent<PlayerAttack>();
		//Debug.Log (GameObject.Find("PlayerBody").GetComponent<PlayerAttack>());
		attackForce = playerAttack.attackForce;
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		isHit = playerAttack.isPunching;
		canBePunched = playerAttack.canPunch;
		//Die ();
	}
	
	void FixedUpdate ()
	{
		ApplyDamage ();
	}
	
	void ApplyDamage()
	{
		//Multiplier for determining which way to launch this enemy. Can be either 1 or -1
		int directionMult;
		
		if(isHit && canBePunched)
		{
			if(IsPlayerOnRight ())
				directionMult = -1;
			else
				directionMult = 1;
			
			GetComponent<Rigidbody2D>().AddForce (new Vector2( attackForce * directionMult, 2 * attackForce));
			
			canBePunched = false;
			hitPoints--;
			
		}
	}
	
	bool IsPlayerOnRight()
	{
		//Difference between this transform and the player's
		float xDif = playerTransform.position.x - transform.position.x;
		if( xDif >= 0 )
			return true;
		else
			return false;
			
	}
	
	void Die()
	{
		if(hitPoints <= 0)
			DestroyImmediate (gameObject);
	}
}
