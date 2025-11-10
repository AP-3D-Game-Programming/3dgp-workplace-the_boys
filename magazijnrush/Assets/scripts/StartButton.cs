using UnityEngine;
using TMPro;

public class StartButton : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public TextMeshProUGUI interactText; // Sleep hier je StartButton text in
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            if (GameManager.Instance != null && !GameManager.Instance.gameActive)
            {
                GameManager.Instance.StartGame();

                // Verberg tekst zodra het spel start
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

            // Alleen tekst tonen als het spel niet actief is
            if (interactText != null && GameManager.Instance != null && !GameManager.Instance.gameActive)
                interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // Verberg tekst
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}
