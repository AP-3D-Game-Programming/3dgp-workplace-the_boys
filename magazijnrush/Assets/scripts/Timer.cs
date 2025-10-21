using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; 
    public float startTime = 60f;     
    private float elapsedTime;

    void Start()
    {
        elapsedTime = startTime;
    }

    void Update()
    {
        if (elapsedTime > 0f)
        {
            elapsedTime -= Time.deltaTime;
            if (elapsedTime < 0f)
                elapsedTime = 0f; // op 0 zetten
        }

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}