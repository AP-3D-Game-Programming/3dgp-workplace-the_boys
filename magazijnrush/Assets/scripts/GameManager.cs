using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text;

public class GameManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gatherItemsText = "Gather these items!";

    [Header("Crate Spawning")]
    public List<GameObject> cratePrefabs;   // Mogelijke crate prefabs
    public Transform spawnParent;           // Parent voor spawnpoints
    public int maxCrates = 5;               // Max crates tegelijk
    public float gameDuration = 60f;        // Spelduur in seconden

    [Header("Order Settings")]
    public int maxItemsPerOrder = 5;        // Max items per order

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI instructiontext;

    private List<Transform> spawnPoints = new List<Transform>();
    private Dictionary<string, int> currentOrder = new Dictionary<string, int>();

    private int score = 0;
    private float remainingTime;
    public bool gameActive = false;

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
        if (instructiontext != null)
                    instructiontext.gameObject.SetActive(false);
    }

    // ---------------- SPAWN LOGICA ----------------
    void SpawnAllCrates()
    {
        var shuffledPoints = new List<Transform>(spawnPoints);
        ShuffleList(shuffledPoints);

        for (int i = 0; i < maxCrates && i < shuffledPoints.Count; i++)
        {
            Transform point = shuffledPoints[i];
            GameObject prefab = cratePrefabs[Random.Range(0, cratePrefabs.Count)];
            GameObject crate = Instantiate(prefab, point.position, point.rotation);
            crate.tag = "SpawnedPickup";
        }
    }

    void CheckRespawn()
    {
        if (GameObject.FindGameObjectsWithTag("SpawnedPickup").Length == 0)
            SpawnAllCrates();
    }

    // ---------------- SCORE & TIMER ----------------
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
        // Timer blijft zichtbaar, geen extra actie nodig

        ClearAllCrates();
        currentOrder.Clear();
        UpdateOrderUI();
    }

    // ---------------- ORDER LOGICA ----------------
    void GenerateRandomOrder()
    {
        currentOrder.Clear();
        GameObject[] activeCrates = GameObject.FindGameObjectsWithTag("SpawnedPickup");
        if (activeCrates.Length == 0) return;

        // Unieke types op het veld
        List<string> availableTypes = new List<string>();
        foreach (var crate in activeCrates)
        {
            string itemName = crate.name.Replace("(Clone)", "").Trim();
            if (!availableTypes.Contains(itemName))
                availableTypes.Add(itemName);
        }

        if (availableTypes.Count == 0) return;

        int totalItemsLeft = Mathf.Min(maxItemsPerOrder, activeCrates.Length);
        ShuffleList(availableTypes);

        foreach (string type in availableTypes)
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

        orderText.text = !gameActive ? "" : BuildOrderString();
    }

    string BuildOrderString()
{
    StringBuilder sb = new StringBuilder();
    sb.AppendLine(gatherItemsText);
    sb.AppendLine();
    sb.AppendLine("Order:");

    foreach (var kv in currentOrder)
        sb.AppendLine($"• {kv.Key} x{kv.Value}");

    return sb.ToString();
}

    public bool TryDeliverItem(string itemName)
    {
        if (!currentOrder.ContainsKey(itemName))
            return false;

        currentOrder[itemName]--;
        if (currentOrder[itemName] <= 0)
            currentOrder.Remove(itemName);

        UpdateOrderUI();

        if (currentOrder.Count == 0)
        {
            AddScore(5);
            ClearAllCrates();
            SpawnAllCrates();
            GenerateRandomOrder();
        }

        return true;
    }

    void ClearAllCrates()
    {
        foreach (var crate in GameObject.FindGameObjectsWithTag("SpawnedPickup"))
            Destroy(crate);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }
}
