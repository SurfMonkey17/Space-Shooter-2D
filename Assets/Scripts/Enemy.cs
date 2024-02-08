using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    [SerializeField]
    private float _enemySpeed = 4.0f;

    private Player _player;

    private Animator _anim;

    [SerializeField]
    private GameObject _enemyFirePrefab;
    
    //private AudioClip _explosionSound; (unused?)
    private AudioSource _audioSource;

    private float _fireRate = 3.0f;
    private float _canFire = -1;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        
        
        //null check player
        if (_player == null)
        {
            Debug.LogError("The Player is Null.");
        }
       
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
           Debug.LogError("The animator is Null.");
        }
        
        _audioSource = GetComponent<AudioSource>(); 
        
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource for enemy explosion is null");

        }      
    }

      void Update()
    {
        {
            CalculateMovment();
        }
        
        if(Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_enemyFirePrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }            
        }             
    }

    void CalculateMovment()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -4.5f)
        {
            float randomX = Random.Range(-8f, 8f); //variable to use anytime we go off screen and want to spawn randomly along x-axis. 
            transform.position = new Vector3(randomX, 6, 0);
        }
    }

    public void DestroyEnemy()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0;
        _audioSource.Play();
        Destroy(gameObject, 2.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.transform.name);

      
        if (other.tag == "Player")
        {
           
            Player player = other.transform.GetComponent<Player>();
            
            if(player != null)
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            Destroy(this.gameObject, 2.0f);
        }
                
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.Score(10); 
            }
                      
            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.0f);
        }             
    }
}