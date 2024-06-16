using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Waves : MonoBehaviour
{
    public GameObject Player;
    public GameObject EnemyNormal;
    public GameObject EnemySpider;
    public GameObject EnemyRanged;
    public GameObject Ambient;
    public GameObject Entities;
    public GameObject VirtualCamera;
    public GameObject ProgressBar;
    public GameObject ProgressBarBorder;
    public GameObject WaveText;

    public GameObject thisObject;
    protected static AudioSource otherSounds;
    public AudioClip[] waveComplete;

    int cycle = 0;
    public int waveNum = 0;
    private const int waveEnemyAdd = 5;

    //For Current Wave
    private int waveEnemySpawnRate = 0;
    private int waveEnemyMaxNum = 0;
    private int waveEnemiesKilled = 0;
    public bool pauseEnemySpawns = false;
    void Start()
    {
        advanceWave();
        cycle = 400;

    }
    void FixedUpdate()
    {
        RectTransform rectTransform = ProgressBar.GetComponent<RectTransform>();
        if (waveNum == 0) return;
        if (!pauseEnemySpawns) cycle++;
        if (cycle >= waveEnemySpawnRate && Entities.GetComponentsInChildren<Enemy>().Length <= waveEnemyMaxNum)
        {
            float rnd = Random.Range(0, 100f);
            spawnEnemy((rnd < 33.3f) ? EnemyNormal : (rnd < 66.6f)?EnemySpider:EnemyRanged);//Spawns a random enemy
            cycle = 0;
        }
    }
    public int getEnemiesByWave()
    {
        return (int) Mathf.Floor((Mathf.Pow((waveNum + 2),2))/10f+waveEnemyAdd);
    }
    public int getEnemySpawnRateByWave()
    {
        return 2000 / waveEnemyMaxNum;
    }
    public void advanceWave()
    {
        waveNum++;
        waveEnemiesKilled = 0;
        waveEnemyMaxNum = getEnemiesByWave();
        waveEnemySpawnRate = getEnemySpawnRateByWave();
        cycle = waveEnemySpawnRate/2;
        updateWaveDisplay();
        if (waveNum != 1) playRandomSound(waveComplete);
    }
    public void enemyKilledByPlayer()
    {
        waveEnemiesKilled++;
        if (waveEnemiesKilled >= waveEnemyMaxNum)
        {
            advanceWave();
        }
        updateWaveDisplay();
    }
    public void updateWaveDisplay()
    {
        int enemiesRemaining = waveEnemyMaxNum - waveEnemiesKilled;
        TextMeshProUGUI textMeshPro = WaveText.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Wave "+ waveNum+":";
        RectTransform borderTransform = ProgressBarBorder.GetComponent<RectTransform>();
        RectTransform rectTransform = ProgressBar.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0.99f * ((float)waveEnemiesKilled / (float)waveEnemyMaxNum), rectTransform.localScale.y, rectTransform.localScale.z);
        float fillSize = rectTransform.sizeDelta.x * rectTransform.localScale.x;
        float borderSize = borderTransform.sizeDelta.x * borderTransform.localScale.x;
        float moveFilling = 340f * (1f - (float) rectTransform.localScale.x);

        ProgressBar.transform.position = new Vector3(-moveFilling + 366f, rectTransform.transform.position.y, rectTransform.transform.position.z);
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
    public void playRandomSound(AudioClip[] sounds, float volume = 1f)
    {
        if (sounds.Length == 0) return;
        AudioClip audio = sounds[Random.Range(0, sounds.Length)];
        playSound(otherSounds, audio);
    }
    public void playSound(AudioSource source, AudioClip sound, float volume = 1f)
    {
        if (source == null)
        {
            source = thisObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
        }

        source.clip = sound;
        source.volume = volume;
        source.Play();
    }
}
