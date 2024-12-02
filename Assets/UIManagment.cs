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
    [SerializeField] TextMeshProUGUI _scoreText;
    string _correctAnswer;
    private List<string> _answers = new List<string>();
    public Button[] _buttons = new Button[3];
    [SerializeField] Button _backButton;
    [SerializeField] Button _nextButton;

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
        _nextButton.gameObject.SetActive(false);
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
                TimeOver();
            }
           
            _timerText.text = "Tiempo: " + GameManager.Instance.timer.ToString("f0");
        }
    }

    private void TimeOver()
    {
        Debug.Log("Tiempo agotado");
        startTimer = false; 
        _nextButton.gameObject.SetActive(true); 
        SceneManager.LoadScene("Results");
    }

    public void OnButtonClick(int buttonIndex)
    {
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        _correctAnswer = GameManager.Instance.responseList[GameManager.Instance.randomQuestionIndex].CorrectOption;

        if (selectedAnswer == _correctAnswer)
        {
            Debug.Log("¡Respuesta correcta!");
            ChangeButtonColor(buttonIndex, Color.green);
            GameManager.Instance._answers.Clear();
            _nextButton.gameObject.SetActive(true);
            CorrectAnswer();
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Int�ntalo de nuevo.");
            startTimer = false;            
            ChangeButtonColor(buttonIndex, Color.red);
            Invoke("RestoreButtonColor", 2f);
            SceneManager.LoadScene("Results");
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

    public void NextButtonOnClick()
    {
        RestoreButtonColor();
        Invoke("NextQuestion", 1f);
    }

    private void NextQuestion()
    {
        queryCalled = false;
    }

    public void backButton()
    {
        Destroy(GameManager.Instance);
        Destroy(UIManagment.Instance);
        SceneManager.LoadScene("LoginScene");
    }

    private void CorrectAnswer()
    {
        if(startTimer)
        {
            startTimer = false;
            Debug.Log("Tiempo detenido");
            GameManager.Instance.timeLeft = Mathf.RoundToInt(10 - GameManager.Instance.timer);

            GameManager.Instance.points = Mathf.RoundToInt(10 - GameManager.Instance.timeLeft);
            GameManager.Instance.AddPoints(GameManager.Instance.points);
            _scoreText.text = "Puntos: " + GameManager.Instance.GetTotalPoints();
            GameManager.Instance.correctAnswers += 1;
        }
    }
}
