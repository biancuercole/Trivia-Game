using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{    
    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] TextMeshProUGUI _questionText;
    [SerializeField] TextMeshProUGUI _timerText;
    string _correctAnswer;
    private List<string> _answers = new List<string>();
    public Button[] _buttons = new Button[3];
    [SerializeField] Button _backButton;

    public bool queryCalled;
    public bool startTimer;

    private Color _originalButtonColor;

    public static UIManagment Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startTimer = true;
        queryCalled = false;
        _originalButtonColor = _buttons[0].GetComponent<Image>().color;
    }

    void Update()
    {
        StartTimer();

        _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");
        _questionText.text = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].QuestionText;
        GameManager.Instance.CategoryAndQuestionQuery(queryCalled);
    }

    public void StartTimer()
    {
        if (startTimer)
        {
            GameManager.Instance.timer -= Time.deltaTime;
            if (GameManager.Instance.timer <= 0)
            {
                GameManager.Instance.timer = 0;
                Debug.Log("Tiempo agotado");
            }
           
            _timerText.text = "Tiempo: " + GameManager.Instance.timer.ToString("f0");
        }
    }

    public void OnButtonClick(int buttonIndex)
    {
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

        if (selectedAnswer == _correctAnswer)
        {
            Debug.Log("¡Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            Invoke("RestoreButtonColor", 2f);
            GameManager.Instance._answers.Clear();
            Invoke("NextQuestion", 2f);
            startTimer = false;
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Int�ntalo de nuevo.");
            startTimer = false;            
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("RestoreButtonColor", 2f);
        }
    }

    private void ChangeButtonColor(int buttonIndex, Color color)
    {
        Image buttonImage = _buttons[buttonIndex].GetComponent<Image>();
        buttonImage.color = color;
    }

    private void RestoreButtonColor()
    {
        foreach (Button button in _buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = _originalButtonColor;
        }
    }

    private void NextQuestion()
    {
        queryCalled = false;
        GameManager.Instance.randomQuestionIndex = Random.Range(0, GameManager.Instance.responseList.Count);
    }

    public void backButton()
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
        SceneManager.LoadScene("LoginScene");
    }
}
