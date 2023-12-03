
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2;
    private float _speedBoost = 2.5f;
    private bool _isSpeedPowerupActive = false;
   
    [SerializeField] 
    private GameObject _laserPrefab;

           
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private int _ammoCount = 15;
   
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false; 

    [SerializeField]
    private GameObject _tripleshotprefab;

    [SerializeField]
    private bool _isShieldActive = false;    
    [SerializeField]
    private GameObject _shield;
    [SerializeField]
    private int _shieldPower = 0;
    private Renderer _shieldRenderer;
    
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    
    [SerializeField]
    private int _score;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private AudioClip _emptyAmmoSoundClip;

    [SerializeField]
    private AudioSource _audioSource;

    private bool _isInvincible = false;
    private float _invincibleDuration = 1.0f;
    private float _invincibilityTimer = 0; 


         
    
    // Start is called before the first frame update
    void Start()
    {
        _shieldRenderer = _shield.GetComponent<Renderer>();

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
        if (_isInvincible)
        {
            _invincibilityTimer -= Time.deltaTime;
            if(_invincibilityTimer <=0)
            {
                _isInvincible = false;
            }
        }

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

        
        if(Input.GetKeyDown(KeyCode.LeftShift) && _isSpeedPowerupActive == false)
        {
            _speed += _speedBoost;                
            
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            _speed -= _speedBoost;

        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;


        if (_isTripleShotActive == true)
        {
           Instantiate(_tripleshotprefab, transform.position, Quaternion.identity);
            _audioSource.clip = _laserSoundClip;
        }

        else if (_ammoCount > 0)
        {
           Object.Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
           _ammoCount -= 1;
           _uiManager.UpdateAmmoCount(_ammoCount);
            _audioSource.clip = _laserSoundClip;
        }

        else if (_ammoCount == 0 && _isTripleShotActive == false)
        {
            _audioSource.clip = _emptyAmmoSoundClip;
        }
               
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isInvincible)
            return;

        if (_isShieldActive == true)        
        {
            if (_shieldPower == 3)
            {               
                _shieldRenderer.material.color = Color.yellow;
                _shieldPower = 2;              
            }

            else if (_shieldPower == 2)
            {
                _shieldRenderer.material.color = Color.red;
                _shieldPower = 1;
             }

            else if (_shieldPower == 1)
            {
                _isShieldActive = false;
                _shield.SetActive(false);              
            }

            return;

        }

        _lives--;

        _isInvincible = true;
        _invincibilityTimer = _invincibleDuration;

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
        _isSpeedPowerupActive = true;
        _speed  *= _speedMultiplier;
        StartCoroutine(SpeedPowerupPowerDown());
    }

    IEnumerator SpeedPowerupPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedPowerupActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldPowerup()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
        _shieldPower = 3;
        _shieldRenderer.material.color = Color.white;
    }
    
    public void AmmoPowerup()
    {
        _ammoCount = 15;
        _uiManager.UpdateAmmoCount(_ammoCount);
        _audioSource.Stop();
    }

    public void Score(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
        
    }

    public void OneUpPowerup()
    {
        if (_lives < 3)
        {
            _lives += 1;
        }

        else
        {
            _lives = 3;
        }
       
        _uiManager.UpdateLives(_lives);
    }
   
}


