using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;

    [SerializeField]
    private int _powerupID;  // 0 = Triple Shot, 1 = Speed, 2 = Shield, 3 = Ammo

    [SerializeField]
    private AudioClip _powerupClip;
    
      
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -4.5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);

            if (player != null)
            {
                switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;

                    case 1:
                        player.SpeedPowerup();
                        break;

                    case 2:
                        player.ShieldPowerup();
                        break;

                    case 3:
                        player.AmmoPowerup();
                        break;

                    case 4: 
                        player.OneUpPowerup();
                        break;

                    default:
                        Debug.Log("Default Value");
                        break;
                }

            }

            Destroy(this.gameObject);
        }
    }

}