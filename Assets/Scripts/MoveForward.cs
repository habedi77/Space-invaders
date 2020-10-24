using UnityEngine;

public class MoveForward : MonoBehaviour
{

	public float speed;

	private Rigidbody2D rb;
	
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
		rb.velocity = transform.up * speed;
	}
}
