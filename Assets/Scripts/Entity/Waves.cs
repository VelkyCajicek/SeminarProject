using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public GameObject Player;
    public GameObject EnemyNormal;
    public GameObject EnemySpider;
    public GameObject EnemyHarkonnen;
    public GameObject Ambient;
    public GameObject Entities;
    public GameObject VirtualCamera;

    int cycle = 0;
    public int waveNum = 0;
    private const int waveEnemyAdd = 5;

    //For Current Wave
    private int waveEnemySpawnRate = 0;
    private int waveEnemyMaxNum = 0;
    private int waveEnemiesKilled = 0;
    void Start()
    {
        advanceWave();
    }
    void FixedUpdate()
    {
        if (waveNum == 0) return;
        cycle++;
        if (cycle >= waveEnemySpawnRate && Entities.GetComponentsInChildren<Enemy>().Length <= waveEnemyMaxNum)
        {
            //TODO HARKONNENS
            spawnEnemy((Random.Range(0, 100f) > 0) ? EnemySpider : EnemyNormal);//Spawns a random enemy
            cycle = 0;
        }
    }
    public int getEnemiesByWave()
    {
        return (int) Mathf.Floor((Mathf.Pow((waveNum + 2),2))/10f+waveEnemyAdd);
    }
    public int getEnemySpawnRateByWave()
    {
        return 4000 / waveEnemyMaxNum;
    }
    public void advanceWave()
    {
        waveNum++;
        waveEnemiesKilled = 0;
        waveEnemyMaxNum = getEnemiesByWave();
        waveEnemySpawnRate = getEnemySpawnRateByWave();
        cycle = waveEnemySpawnRate-1000;
        Debug.Log($"Wave Advanced: {waveNum}_{waveEnemySpawnRate}_{waveEnemyMaxNum}");
    }
    public void enemyKilledByPlayer()
    {
        waveEnemiesKilled++;
        Debug.Log($"Enemy Killed: {waveEnemiesKilled} out of {waveEnemyMaxNum}");
        if (waveEnemiesKilled >= waveEnemyMaxNum)
        {
            advanceWave();
        }
    }
    public void updateWaveDisplay()
    {
        int enemiesRemaining = waveEnemyMaxNum - waveEnemiesKilled;
        //Update current wave num
        //Update enemies in wave remaining - maybe progress bar???
    }
    public void spawnEnemy(GameObject EnemyObject)
    {
        GameObject go = GameObject.Instantiate(EnemyObject);
        Enemy enemyScript = go.GetComponent<Enemy>();
        go.SetActive(true);
        enemyScript.thisObject = go;
        enemyScript.transform.parent = Entities.transform;
        enemyScript.spawnPos = getEnemySpawnPos(enemyScript.objectCollider.bounds.size);
        Debug.Log(enemyScript.objectCollider.bounds.size);
        enemyScript.Spawn();
        Debug.Log($"Enemy Spawned");
    }
    public Vector2 getEnemySpawnPos(Vector2 enemySize)
    {
        CameraZoom cameraZoom = VirtualCamera.GetComponent<CameraZoom>();
        Player playerScript = Player.GetComponent<Player>();
        float virtualCameraSize = cameraZoom.maxZoom * 1.77f;
        Vector2 spawnPos = playerScript.objectCollider.bounds.center;
        int spawnSide = (Random.Range(0, 100f) > 50) ? 1 : -1;
        spawnPos += new Vector2((virtualCameraSize + enemySize.x)*spawnSide, 0);

        RaycastHit2D hit = Physics2D.Raycast(spawnPos, -Vector2.up);
        if (hit.collider == null)
        {
            spawnPos += new Vector2((virtualCameraSize + enemySize.x) * spawnSide * -2, 0);
        }
        spawnPos = getGroundFromPos(spawnPos,enemySize);

        return spawnPos;
    }
    public Vector2 getGroundFromPos(Vector2 pos, Vector2 enemySize)
    {
        Vector2 hitLeft = Vector2.zero;
        Vector2 hitRight = Vector2.zero;

        RaycastHit2D hit = Physics2D.Raycast(pos + new Vector2(enemySize.x, 1000), -Vector2.up);
        if (hit.collider != null)
        {
            hitRight = hit.point;
        }
        RaycastHit2D hit2 = Physics2D.Raycast(pos + new Vector2(-enemySize.x, 1000), -Vector2.up);
        if (hit2.collider != null)
        {
            hitLeft = hit2.point;
        }
        return new Vector2(pos.x, (hitLeft.y > hitRight.y) ? hitLeft.y : hitRight.y + enemySize.y);
    }
}
