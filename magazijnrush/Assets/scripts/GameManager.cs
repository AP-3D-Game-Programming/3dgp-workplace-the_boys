using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text;

public class GameManager : MonoBehaviour
{
    [Header("Crate Spawning")]
    public List<GameObject> cratePrefabs;   // Alle mogelijke crate prefabs
    public Transform spawnParent;           // Parent met spawnpoints
    public int maxCrates = 5;               // Hoeveel crates tegelijk
    public float gameDuration = 60f;        // Spelduur in seconden

    [Header("Order Settings")]
    public int maxItemsPerOrder = 5;        // Max aantal items per bestelling

    [Header("UI")]
    public TextMeshProUGUI scoreText;       // Score UI
    public TextMeshProUGUI timerText;       // Timer UI
    public TextMeshProUGUI orderText;       // Order UI

    private List<Transform> spawnPoints = new List<Transform>();
    private int score = 0;
    private float remainingTime;
    public bool gameActive = false;

    private Dictionary<string, int> currentOrder = new Dictionary<string, int>();

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (Transform child in spawnParent)
            spawnPoints.Add(child);

        UpdateScoreUI();
        UpdateTimerUI();
        UpdateOrderUI();
    }

    void Update()
    {
        if (!gameActive) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }

        UpdateTimerUI();
        CheckRespawn();
    }

    public void StartGame()
    {
        if (gameActive) return;

        gameActive = true;
        remainingTime = gameDuration;

        score = 0;
        UpdateScoreUI();

        SpawnAllCrates();
        GenerateRandomOrder();
    }

    void SpawnAllCrates()
    {
        List<Transform> shuffled = new List<Transform>(spawnPoints);
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
            crate.tag = "SpawnedPickup";
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

        // Verwijder alle crates
        foreach (GameObject crate in GameObject.FindGameObjectsWithTag("SpawnedPickup"))
            Destroy(crate);

        // Leeg order
        currentOrder.Clear();
        UpdateOrderUI();
    }

    // ---------------- ORDER LOGICA ----------------

    void GenerateRandomOrder()
    {
        currentOrder.Clear();

        // Haal alle crates op het veld
        GameObject[] activeCrates = GameObject.FindGameObjectsWithTag("SpawnedPickup");
        if (activeCrates.Length == 0) return;

        // Unieke item types
        List<string> availableTypes = new List<string>();
        foreach (var crate in activeCrates)
        {
            string itemName = crate.name.Replace("(Clone)", "").Trim();
            if (!availableTypes.Contains(itemName))
                availableTypes.Add(itemName);
        }

        if (availableTypes.Count == 0) return;

        int totalItemsLeft = Mathf.Min(maxItemsPerOrder, activeCrates.Length);

        // Shuffle types
        List<string> shuffledTypes = new List<string>(availableTypes);
        for (int i = 0; i < shuffledTypes.Count; i++)
        {
            int randIndex = Random.Range(i, shuffledTypes.Count);
            (shuffledTypes[i], shuffledTypes[randIndex]) = (shuffledTypes[randIndex], shuffledTypes[i]);
        }

        foreach (string type in shuffledTypes)
        {
            if (totalItemsLeft <= 0) break;

            int availableCount = 0;
            foreach (var crate in activeCrates)
                if (crate.name.Contains(type))
                    availableCount++;

            int count = Random.Range(1, Mathf.Min(availableCount, totalItemsLeft) + 1);
            currentOrder[type] = count;

            totalItemsLeft -= count;
        }

        UpdateOrderUI();
    }

    void UpdateOrderUI()
    {
        if (orderText == null) return;

        if (!gameActive)
        {
            orderText.text = "";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Order:");

        foreach (var kv in currentOrder)
            sb.AppendLine($"• {kv.Key} x{kv.Value}");

        orderText.text = sb.ToString();
    }

    public bool TryDeliverItem(string itemName)
    {
        if (!currentOrder.ContainsKey(itemName))
            return false; // Niet in de order → fout item

        currentOrder[itemName]--;

        if (currentOrder[itemName] <= 0)
            currentOrder.Remove(itemName);

        UpdateOrderUI();

        // Als order leeg is → order voltooid
        if (currentOrder.Count == 0)
        {
            AddScore(5); // Beloning voor volledige order

            // Verwijder alle overgebleven crates
            foreach (GameObject crate in GameObject.FindGameObjectsWithTag("SpawnedPickup"))
                Destroy(crate);

            // Spawn verse crates voor nieuwe order
            SpawnAllCrates();

            // Genereer nieuwe order
            GenerateRandomOrder();
        }

        return true;
    }
}
