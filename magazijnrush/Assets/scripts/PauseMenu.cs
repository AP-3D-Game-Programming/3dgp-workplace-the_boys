using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Maak deze variabele in de Inspector leeg en sleep je 'PauzeMenu' UI-paneel er naartoe
    public GameObject pauseMenuUI;

    // Houdt de huidige staat van het spel bij
    public static bool GameIsPaused = false;

    void Update()
    {
        // Controleer of de 'Escape' toets is ingedrukt
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume(); // Als het spel gepauzeerd is, hervat het dan
            }
            else
            {
                Pause(); // Als het spel loopt, pauzeer het dan
            }
        }
    }

    public void Resume()
    {
        Debug.Log("Resume-knop is geklikt en wordt nu uitgevoerd.");
        // Verberg de UI van het pauzemenu
        pauseMenuUI.SetActive(false);
        // Zet de tijdsschaal terug naar normaal (1 = normale snelheid)
        Time.timeScale = 1f;
        // Markeer het spel als niet gepauzeerd
        GameIsPaused = false;
        // Ontgrendel de muiscursor (als je die verborgen had)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        // Toon de UI van het pauzemenu
        pauseMenuUI.SetActive(true);
        // Zet de tijdsschaal op 0 (alles stopt met bewegen/werken)
        Time.timeScale = 0f;
        // Markeer het spel als gepauzeerd
        GameIsPaused = true;
        // Toon de muiscursor zodat de speler kan klikken
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Load Menu-knop is geklikt! Scene wordt geladen.");
        // Zorg ervoor dat de tijdsschaal eerst weer op 1 staat, anders is het hoofdmenu ook 'bevroren'
        Time.timeScale = 1f;
        // Laad Scene 0 (ervan uitgaande dat dit je hoofdmenu is)
        SceneManager.LoadScene(0);
    }
}