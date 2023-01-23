using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //public GameObject enemyPrefab;
    public List<GameObject> enemyPrefabs;
    private int randomEnemy;
    //public GameObject powerupPrefab;
    public GameObject[] powerupPrefabs;
    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;
    public GameObject bossTextUI;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        SpawnPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if(!PlayerController.GameOver && enemyCount == 0)
        {
            waveNumber++;
            // Spawn a boss every x number of waves
            if(waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
                bossTextUI.SetActive(true);
            }
            else
            {
                bossTextUI.SetActive(false);
                SpawnEnemyWave(waveNumber);
            }

            SpawnPowerup();
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            randomEnemy = Random.Range(0, enemyPrefabs.Count);
            Instantiate(enemyPrefabs[randomEnemy], GenerateSpwanPosition(), enemyPrefabs[randomEnemy].transform.rotation);
        }
    }

    void SpawnPowerup()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpwanPosition(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    private Vector3 GenerateSpwanPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0.5f, spawnPosZ);

        return randomPos;
    }

    void SpawnBossWave(int currentRound)
    {
        int miniEnemiesToSpawn;
        // We don't want to divide by 0
        if (bossRound != 0)
        {
            miniEnemiesToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemiesToSpawn = 1;
        }

        var boss = Instantiate(bossPrefab, GenerateSpwanPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemiesToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpwanPosition(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }
}
