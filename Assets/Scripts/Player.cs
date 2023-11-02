
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
   
    [SerializeField] 
    private GameObject _laserPrefab;
        
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;
   
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false; 

    [SerializeField]
    private GameObject _tripleshotprefab;

    [SerializeField]
    private bool _isShieldActive = false;
           
    [SerializeField]
    private GameObject _shield;
    
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    
    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private AudioSource _audioSource;

   

  

      
        
    
    // Start is called before the first frame update
    void Start()
    {
        

    //take the current position = new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
       
        if (_spawnManager == null)
        {
            Debug.Log("The Spawn Manager is null.");
        }

        if (_uiManager == null)
        {
            Debug.Log("The UI Manager is null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is null.");
        }
        else 
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
          FireLaser();
        }

  
    }

    void CalculateMovement() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime);
                       
        //restrict player movement on y axis to -3.8f and 0

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0));
        
        //restrict player movement to between 11 and -11. Player will wrap to other side when limit is reached. 

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }

        
        if(Input.GetKeyDown("left shift"))
        {
            _speed += 5.0f;    //increase thruster/speed

            
            //play thruster animation
        }
        else if(Input.GetKeyUp("left shift")) 
        {
            _speed = 3.5f;

            //stop thruster animation
        }

        

        //thruster return to normal
        //stop thruster animation


    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;


        if (_isTripleShotActive == true)
        {
           Instantiate(_tripleshotprefab, transform.position, Quaternion.identity);
        }

        else
        {
            Object.Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        }

        //play laser fire audio
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shield.SetActive(false);
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }

        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
           _spawnManager.OnPlayerDeath();
           Destroy(this.gameObject);
                                 
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        while (_isTripleShotActive == true)
        {
            yield return new WaitForSeconds(5.0f);
            _isTripleShotActive = false;
        }
    }

    public void SpeedPowerup()
    {
        //_isSpeedPowerupActive = true;
        _speed  *= _speedMultiplier;
        StartCoroutine(SpeedPowerupPowerDown());
    }

    IEnumerator SpeedPowerupPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        //_isSpeedPowerupActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldPowerup()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
    }

    public void Score(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
        
    }
   
}


