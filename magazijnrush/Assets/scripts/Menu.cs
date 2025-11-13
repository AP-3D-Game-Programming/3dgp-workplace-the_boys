using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void TutorialButton ()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayButton ()
    { 
        SceneManager.LoadScene(2);
    }
}
