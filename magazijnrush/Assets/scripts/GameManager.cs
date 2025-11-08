using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Crate Spawning")]
    public List<GameObject> cratePrefabs;       // Alle mogelijke crate prefabs
    public Transform spawnParent;               // Parent met spawnpoints
    public int maxCrates = 5;                   // Hoeveel crates tegelijk
    public float gameDuration = 60f;            // Spelduur in seconden
    public TextMeshProUGUI scoreText;           // UI score
    public TextMeshProUGUI timerText;           // UI timer

    private List<Transform> spawnPoints = new List<Transform>();
    private int score = 0;
    private float remainingTime;
    public bool gameActive = false;

    public static GameManager Instance;

    void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Spawnpoints verzamelen
        foreach (Transform child in spawnParent)
            spawnPoints.Add(child);

        UpdateScoreUI();
        UpdateTimerUI();

        StartGame();
    }

    void Update()
    {
        if (!gameActive) return;

        // Timer aftellen
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }

        UpdateTimerUI();
        CheckRespawn();

        // Controleer respawn alleen voor crates met tag "SpawnedPickup"
    }

    // Start het spel: timer + initial crates spawn
    public void StartGame()
    {
        gameActive = true;
        remainingTime = gameDuration;

        SpawnAllCrates();
    }

    void SpawnAllCrates()
    {
        List<Transform> shuffled = new List<Transform>(spawnPoints);
        // Shuffle spawnpoints voor random positie
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randIndex]) = (shuffled[randIndex], shuffled[i]);
        }

        for (int i = 0; i < maxCrates && i < shuffled.Count; i++)
        {
            Transform point = shuffled[i];
            GameObject prefab = cratePrefabs[Random.Range(0, cratePrefabs.Count)];
            GameObject crate = Instantiate(prefab, point.position, point.rotation);
            crate.tag = "SpawnedPickup"; // Geef de juiste tag
        }
    }

    void CheckRespawn()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("SpawnedPickup");
        if (crates.Length == 0)
        {
            SpawnAllCrates();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
    }

    void EndGame()
    {
        gameActive = false;
        if (timerText != null)
            timerText.text = "Time's Up!";
    }
}