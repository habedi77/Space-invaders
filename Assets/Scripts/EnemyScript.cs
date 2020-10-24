using UnityEngine;

public class EnemyScript : MonoBehaviour {

	// Use this for initialization

	public GameObject[] BoltsPrefabs;
	public float fireChance;
	public Transform FireTransform;
	public GameController Controller;
	public int ScoreOnKill;

	private float _rand;
	void Start ()
	{
//		Controller = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		_rand = Random.value;
		if (_rand < fireChance)
			Fire();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if( !other.CompareTag("Boundry"))
		Controller.ReportAlienHit(gameObject,other.gameObject);
	}

	private void Fire()
	{
		Instantiate(
			BoltsPrefabs[Random.Range(0, BoltsPrefabs.Length)],
			FireTransform.position,FireTransform.rotation);
	}
	
	
}
