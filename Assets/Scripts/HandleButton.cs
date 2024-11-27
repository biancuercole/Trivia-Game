using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HandleButton : MonoBehaviour
{
    public void backButton()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
