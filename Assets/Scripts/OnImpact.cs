using UnityEngine;

public class OnImpact : MonoBehaviour
{

	public GameController Controller;

	private void Start()
	{
		Controller = FindObjectOfType<GameController>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
//		Debug.Log("reporting bolt "+ other);
		if (other)
			StartCoroutine(Controller.ReportBoltHit(gameObject, other.gameObject));
//		else
//			Destroy(gameObject);
	}
}
