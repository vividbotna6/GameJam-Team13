using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public void CloseApp()
    {
        Application.Quit();
        Debug.Log("Application has Quit");
    }
}
