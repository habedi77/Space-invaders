using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum Gamestate
{
	Menue,Start,InProgress,Lost,Won
}

public class GameController : MonoBehaviour
{
	private int _score;
	private int _enemiesAlive;
	private Animator[] _animators;
	private GameObject[,] _enemies;
	private Vector2 _topRight;
	private Vector2 _bottomLeft;
	private Vector2 _movment;
	private bool _game;
	
	private float _delay;
	private float _speedUp;
	private Gamestate _gamestate;
	private int _backgoundAudioStep;
	
	
	public GameObject[] Enemies;
	public GameObject EnemyExpolsion;
	public GameObject PlayerExplosion;
	public GameObject BoltExplosion;
	public float ExplosionTime;
	public float BoltExplosionTime;
	public LayerMask BunkerLayer;
	
	//ENEMIES CONFIG
	public Boundary EnemiesBound;
	public Vector2 EnemySpeed;
	public Transform SpawnTopLeft;
	public Vector2 Gap;
	public Vector2 NoOfEnemies;
	public float EnemyFireRate;
	public float MaxDelay = 1.6f;
	public float MinDelay = 1.6f;
	
	
	//SFX
	public AudioSource Explosion;
	public AudioSource BackgroundAudioSource;
	public AudioClip BackGroundStep1;
	public AudioClip BackGroundStep2;
	public AudioClip BackGroundStep3;
	public AudioClip BackGroundStep4;
	//TODO other sfx

	public UIController UiController;
	
	void Start ()
	{

		_game = true;
		_score = 0;
		_gamestate = Gamestate.InProgress;
		_movment  = new Vector2(1,0);
		_topRight = SpawnTopLeft.position;
		_bottomLeft = SpawnTopLeft.position;
		_backgoundAudioStep = 1;
		SpawnEnemies((int)NoOfEnemies.x,(int)NoOfEnemies.y);
		UpdateBorder();
		MangeEnemyFiring();
		UpdateScreenMessage();
		StartCoroutine(MoveAll());
		
	}



	private void SpawnEnemies(int rows, int cols)
	{
		Vector2 pos = SpawnTopLeft.position;
		int i;
		_enemiesAlive = rows * cols;
		
		
		_speedUp = 1f / _enemiesAlive;
		_delay = MaxDelay;
		
		_animators = new Animator[_enemiesAlive];
		_enemies = new GameObject[rows,cols];
		for (int row = 0; row < rows; row++)
		{
			
			for (int col = 0; col < cols; col++)
			{
				i = row * cols + col;
				_enemies[row,col] =  Instantiate(Enemies[row/2], pos, Quaternion.identity);
				_animators[i] = _enemies[row,col].GetComponentInChildren<Animator>();
				_enemies[row, col].GetComponent<EnemyScript>().Controller = this;
				pos.x += Gap.x;
			}
			pos.y -= Gap.y;
			pos.x = SpawnTopLeft.position.x;
		}
	}
	
	public IEnumerator ReportBoltHit(GameObject bolt,GameObject other)
	{
		if (other.CompareTag("Enemy") && bolt.CompareTag("Bolt"))
		{
			KillAlien(other);
			Destroy(Instantiate(EnemyExpolsion,
				other.transform.position, other.transform.rotation),ExplosionTime);
			DestroyBolt(bolt);
			
			_enemiesAlive--;
			UpdateSpeed();
			if (_enemiesAlive == 0)
				_gamestate = Gamestate.Won;
			Explosion.Play();
			yield return null;
			MangeEnemyFiring();
		}
		else if ( other.CompareTag("Player") && !bolt.CompareTag("Bolt") && _gamestate != Gamestate.Won )
		{
//			Destroy(bolt);
			DestroyBolt(bolt);
			KillPlayer(other);
		}
		else if (other.CompareTag("Bunker Block"))
		{
//			Debug.Log("Bunker hit");
			RaycastHit2D[] hits = Physics2D.CircleCastAll(other.transform.position,3f,Vector2.zero,0,BunkerLayer);
			foreach (RaycastHit2D hit in hits)
			{
//				Debug.Log("hit: "+hit.transform.gameObject);
				if( Random.value < .5)
					Destroy(hit.transform.gameObject);
			}

			Destroy(Instantiate(BoltExplosion,bolt.transform.position,bolt.transform.rotation),
				BoltExplosionTime);
//			Destroy(bolt);
			DestroyBolt(bolt);
		}
		else if (other.CompareTag("Enemy Bolt") && bolt.CompareTag("Bolt"))
		{
			//TODO Destroy enemy bolt
			DestroyBolt(other);
//			Destroy(bolt);
			Destroy(Instantiate(BoltExplosion, bolt.transform.position, bolt.transform.rotation),
				BoltExplosionTime);
			DestroyBolt(bolt);
			
		}
		
	}

	private void DestroyBolt(GameObject bolt)
	{
		//TODO bolt expolsion
//		bolt.GetComponent<BoxCollider2D>().enabled = false;
		bolt.transform.position = new Vector2(-90,50);
//		bolt.GetComponent<SpriteRenderer>().enabled = false;
		Destroy(bolt,0.4f);
	}
	
	public void ReportAlienHit(GameObject alien, GameObject other)
	{
		if (other.CompareTag("Player"))
		{
			KillPlayer(other);
		}

		if (other.CompareTag("Bunker Block"))
		{
			Destroy(other);
		}
	}

	private void KillAlien(GameObject alien)
	{
		_score += alien.GetComponent<EnemyScript>().ScoreOnKill;
		
		Destroy(alien.transform.root.gameObject);
		UpdateScreenMessage();
	}
	
	private void KillPlayer(GameObject player)
	{
		_gamestate = Gamestate.Lost;
		Destroy(Instantiate(PlayerExplosion,
				player.transform.position, player.transform.rotation), ExplosionTime * 4);
		
//		player.transform.root.gameObject.SetActive(false);
		player.GetComponent<SpriteRenderer>().enabled = false;
//		player.GetComponent<BoxCollider2D>().enabled = false;
	}
	
	private void UpdateBorder()
	{
		for (int i=0;i<NoOfEnemies.x;i++)
		{
			for (int j=0;j<NoOfEnemies.y;j++)
			{
				if (!_enemies[i,j])
					continue;
				_topRight = _enemies[i,j].transform.position;
				_bottomLeft = _enemies[i,j].transform.position;
				break;
			}
		}

		for (int i=0;i<NoOfEnemies.x;i++)
		{
			for (int j=0;j<NoOfEnemies.y;j++)
			{
				if (!_enemies[i,j])
					continue;
				//			Debug.Log("ENM: "+enm.transform.position);
				if (_enemies[i,j].transform.position.x > _topRight.x)
					_topRight.x = _enemies[i,j].transform.position.x;
				if (_enemies[i,j].transform.position.x < _bottomLeft.x)
					_bottomLeft.x = _enemies[i,j].transform.position.x;

				if (_enemies[i,j].transform.position.y > _topRight.y)
					_topRight.y = _enemies[i,j].transform.position.y;
				if (_enemies[i,j].transform.position.y < _bottomLeft.y)
					_bottomLeft.y = _enemies[i,j].transform.position.y;
			}
		}
		
		
		_topRight.x += EnemySpeed.x * _movment.x;
		_bottomLeft.x += EnemySpeed.x * _movment.x;
	}

	private IEnumerator MoveAll()
	{
		Rigidbody2D rb;
		while (_gamestate == Gamestate.InProgress)
		{
			PlayBackGroundAudio();
			UpdateBorder();			
			if (_topRight.x > EnemiesBound.XMax && _movment.x > 0)
			{
				_movment.x *= -1;
				_movment.y = -1;
			}
			else if (_bottomLeft.x < EnemiesBound.XMin && _movment.x < 0)
			{
				_movment.x *= -1;
				_movment.y = -1;
			}
			Vector2 sp = new Vector2(
				_movment.y != 0 ? 0 : EnemySpeed.x*_movment.x,
				EnemySpeed.y*_movment.y);
			for (int i=0;i<NoOfEnemies.x;i++)
			{
				for (int j=0;j<NoOfEnemies.y;j++)
				{
					if(!_enemies[i,j])
						continue;
					rb = _enemies[i,j].GetComponent<Rigidbody2D>();
					rb.position += sp;
				}

			}
			UpdateBorder();
			_movment.y = 0;
			
			yield return new WaitForSeconds(_delay);
		}

		UpdateScreenMessage();
		MangeEnemyFiring();

	}

	private void PlayBackGroundAudio()
	{
		switch (_backgoundAudioStep)
		{
			case 1:
				_backgoundAudioStep++;
				BackgroundAudioSource.clip = BackGroundStep1;
				break;
			case 2:
				_backgoundAudioStep++;
				BackgroundAudioSource.clip = BackGroundStep2;
				break;
			case 3:
				_backgoundAudioStep++;
				BackgroundAudioSource.clip = BackGroundStep3;
				break;
			case 4:
				_backgoundAudioStep = 1;
				BackgroundAudioSource.clip = BackGroundStep4;
				break;
			default:
				Debug.Log("Error in BG music step");
				_backgoundAudioStep = 1;
				break;
		}
		BackgroundAudioSource.Play();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
//		if (_enemies != null) 
//			foreach (GameObject enm in _enemies)
//			{
//				if(!enm)
//					continue;
//				Gizmos.DrawWireSphere(enm.transform.position,0.05f);	
//			}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_topRight,.1f);
		Gizmos.DrawWireSphere(_bottomLeft,.1f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube((_topRight+_bottomLeft)*.5f,_topRight-_bottomLeft);
		
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(
			new Vector2(
				(EnemiesBound.XMax+EnemiesBound.XMin)/2,
				(EnemiesBound.YMax+EnemiesBound.YMin)/2),
			new Vector2(
				EnemiesBound.XMax-EnemiesBound.XMin,
				EnemiesBound.YMax-EnemiesBound.YMin)
				);
	}

	private void UpdateSpeed()
	{
		foreach (Animator an in _animators)
		{
//			Debug.Log(an);
			if(an)
				an.speed *= 1.05f;
		}
		
//		_delay = MaxDelay - EasingFunction.
//			         EaseInCubic(0, MaxDelay, 1 - _enemiesAlive*_speedUp );
//		_delay = Mathf.Lerp(MaxDelay, MinDelay, );
		_delay = myEaseFunc(MaxDelay, MinDelay, 1 - _enemiesAlive * _speedUp);
//		Debug.Log(_delay);
	}

	private float myEaseFunc(float max, float min, float progress)
	{
		return Mathf.Pow(progress,1.5f)*(min - max) + max;
	}
	
	private void UpdateScreenMessage()
	{
		UiController.UpdateScreenMessage(_gamestate,_score);
	}

	private void MangeEnemyFiring()
	{
		int rows = (int) NoOfEnemies.x;
		int cols = (int) NoOfEnemies.y;
		for(int j =0;j<cols; j++)
			for (int i = rows - 1; i >= 0; i--)
			{
				if (_enemies[i, j])
				{
					_enemies[i, j].GetComponent<EnemyScript>().fireChance = 
						_gamestate != Gamestate.InProgress ? 0 : EnemyFireRate;
					break;
				}
			}
	}
	
}
