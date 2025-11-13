using UnityEngine;
using TMPro;

public class StartButton : MonoBehaviour
{
    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public TextMeshProUGUI interactText; // Sleep hier je StartButton tekst in

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            if (GameManager.Instance != null && !GameManager.Instance.gameActive)
            {
                // Start het spel
                GameManager.Instance.StartGame();

                // Verberg de tekst zodra het spel start
                if (interactText != null)
                    interactText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            // Toon de tekst altijd als de speler dichtbij is
            if (interactText != null)
                interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // Verberg de tekst als de speler weggaat
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}