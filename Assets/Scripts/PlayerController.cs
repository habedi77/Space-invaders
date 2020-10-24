using System;
using UnityEngine;

[Serializable]
public class Boundary
{
	public float XMin, XMax, YMin, YMax;
}

public class PlayerController : MonoBehaviour
{
	public float Speed;
	public Boundary Boundary;

	public GameObject Shot;
	public Transform ShotSpawn;
	public float FireRate;

	
	private float _nextFire;
	private Rigidbody2D _rb;
//	private AudioSource aus;
	
	
	void Start()
	{
		_rb = GetComponent<Rigidbody2D>();
//		aus = GetComponent<AudioSource>();
	}

	
	
	void Update ()
	{
		if (Input.GetButton("Fire1") && Time.time > _nextFire)
		{
			_nextFire = Time.time + FireRate;
			Instantiate(Shot, ShotSpawn.position, ShotSpawn.rotation);
//			aus.Play();
		}
	}

	void FixedUpdate()
	{
//		float moveHorizontal = Input.GetAxis("Horizontal");
//		float moveVertical = Input.GetAxis("Vertical");
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveAcc = Input.acceleration.x;
		if (Mathf.Abs(moveHorizontal) < Mathf.Abs(moveAcc))
			moveHorizontal = moveAcc;
		//Debug.Log(moveHorizontal + " " + moveVertical);
		
		Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0f);
		
		
		_rb.velocity = movement * Speed;

		_rb.position = new Vector3
		(
			Mathf.Clamp(_rb.position.x, Boundary.XMin, Boundary.XMax),
			0.0f,
			Mathf.Clamp(_rb.position.y, Boundary.YMin, Boundary.YMax)
		);

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		
		Gizmos.DrawWireCube(
			new Vector2((Boundary.XMax+Boundary.XMin)/2,0),
		new Vector2(Boundary.XMax-Boundary.XMin,4));
	}
}
