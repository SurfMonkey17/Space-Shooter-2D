using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _ammoCountText;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite [] _liveSprites;

    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;

    private GameManager _gameManager;

    private Slider _thrusterSlider;
    private float _maxThrusterPower = 100f;
    private float _currentThrusterPower;
    

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _ammoCountText.text = "Ammo Count: " + 15;
        _currentThrusterPower = _maxThrusterPower;
       
        
        if (_gameManager == null)
        { 
            Debug.LogError("GameManager is null."); 
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];
        
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }
    
    public void UpdateAmmoCount(int ammoCount)
    {
        _ammoCountText.text = "Ammo Count: " + ammoCount.ToString();
    }
   
    
    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _restartText.gameObject.SetActive(true);
         
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }

     
    }

    
}
