using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryDestroyOnExit : MonoBehaviour
{

	public GameObject BoltExplosion;
	public float BoltExplosionTime;
	
	
	
	private void _Destroy(GameObject o)
	{
		if (o.CompareTag("Bolt") || o.CompareTag("Enemy Bolt"))
		{
			Destroy(Instantiate(BoltExplosion,o.transform.position,o.transform.rotation),BoltExplosionTime);
		}
		Destroy(o.transform.root.gameObject);
	}
	
//	private void OnCollisionExit(Collision other)
//	{
//		_Destroy(other.gameObject);
//	}
	
	private void OnCollisionExit2D(Collision2D other)
	{
		_Destroy(other.gameObject);
	}
//
//	private void OnTriggerExit(Collider other)
//	{
//		_Destroy(other.gameObject);
//	}

	private void OnTriggerExit2D(Collider2D other)
	{
		_Destroy(other.gameObject);
	}
}
