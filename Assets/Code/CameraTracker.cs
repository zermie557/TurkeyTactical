using UnityEngine;
using System.Collections;

public class CameraTracker : MonoBehaviour 
{
	public float xMargin = 4f;
	public float yMargin = 2f;
	public float xSmooth = 1f;
	public float ySmooth = 1f;
	//public Vector2 maxXAndY;
	//public Vector2 minXAndY;
	public float currentXDif;
	public float currentYDif;
	
	private Transform player;
	
	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag ("Player").transform;
	}	
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		currentXDif = Mathf.Abs(transform.position.x - player.position.x);
		currentYDif = Mathf.Abs(transform.position.y - player.position.y);
		
		TrackPlayer();
	}
	
	void TrackPlayer()
	{
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		
		if(CheckXMargin() == true)
			targetX = Mathf.Lerp (transform.position.x, player.position.x, xSmooth * Time.deltaTime);
		
		if(CheckYMargin() == true)
			targetY = Mathf.Lerp (transform.position.y, player.position.y, ySmooth * Time.deltaTime);
		
		//targetX = Mathf.Clamp( targetX, minXAndY.x, maxXAndY.x);
		//targetY = Mathf.Clamp( targetY, minXAndY.y, maxXAndY.y);
		
		transform.position = new Vector3(targetX, targetY, transform.position.z);	
	}
	
	bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
	}
	
	bool CheckYMargin()
	{
		return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
	}
}
