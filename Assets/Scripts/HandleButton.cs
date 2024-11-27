using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HandleButton : MonoBehaviour
{
    public void backButton()
    {
        Destroy(GameManager.Instance?.gameObject);
        Destroy(UIManagment.Instance?.gameObject);
        SceneManager.LoadScene("LoginScene");
    }
}
