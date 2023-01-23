using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool GameOver { get; private set; }
    public GameObject gameoverUI;
    private bool restartGame;

    private Rigidbody playerRb;
    private GameObject focalPoint;
    public GameObject powerupIndicator;
    private float powerUpStrength = 15.0f;
    public float speed = 5.0f;
    public bool hasPowerUp;
    public PowerUpType currentPowerUp = PowerUpType.None;

    // PowerUp_Rockets
    public GameObject rocketsUI;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    // PowerUp_Smash
    public GameObject smashUI;
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;

    bool smashing = false;
    float floorY;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        GameOver = false;
        restartGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameOver)
        {
            if (transform.position.y < -10)
            {
                //playerRb.useGravity = false;
                playerRb.isKinematic = true;
                GameOver = true;
                StartCoroutine(GameOverRoutine());
            }
            else
            {
                float forwardInput = Input.GetAxis("Vertical");
                playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
                powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

                if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
                {
                    LaunchRockets();
                }

                if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
                {
                    smashing = true;
                    StartCoroutine(Smash());
                }
            }
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            Destroy(other.gameObject);
            powerupIndicator.gameObject.SetActive(true);
            CheckPowerupUI();

            if(powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }  

    IEnumerator GameOverRoutine()
    {
        gameoverUI.SetActive(true);

        while (!restartGame)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                restartGame = true;                
            }
            yield return null;
        }

        gameoverUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerUp = false;
        currentPowerUp = PowerUpType.None;
        CheckPowerupUI();
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
        
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        // Store the y position before taking off
        floorY = transform.position.y;

        // Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while(Time.time < jumpTime)
        {
            // Move the player up while still keeping their x velocity
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // Now move the player down
        while(transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // Cycle through all enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            // Apply an explosion force that originates from our position
            if(enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        // We are no longer smashing, so set the boolean to false;
        smashing = false;
    }

    private void CheckPowerupUI()
    {
        switch (currentPowerUp)
        {
            case PowerUpType.Rockets:
                rocketsUI.SetActive(true);
                smashUI.SetActive(false);
                break;
            case PowerUpType.Smash:
                smashUI.SetActive(true);
                rocketsUI.SetActive(false);
                break;
            default:
                rocketsUI.SetActive(false);
                smashUI.SetActive(false);
                break;
        }
    }
}
