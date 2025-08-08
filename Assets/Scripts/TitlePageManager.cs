using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePageManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
