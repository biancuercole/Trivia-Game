using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public TriviaManager triviaManager;

    public List<question> responseList;
    private List<int> usedQuestionIndices = new List<int>();
    public int randomQuestionIndex = 0;
    public List<string> _answers = new List<string>();
    public int TotalQuestions
    {
        get { return responseList != null ? responseList.Count : 0; }
    }
    string _correctAnswer;

    public int currentTriviaIndex = 0;
    public int _numQuestionAnswered = 0;

    public bool queryCalled;

    private int _points;
    public float timer;
    public float timeLeft;
    public float initTime = 10f;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timer = initTime;
        queryCalled = false;
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {
             _answers.Clear();

            //si se han mostrado todas las preguntas, reiniciar el registro de índices
            if (usedQuestionIndices.Count >= responseList.Count)
            {
                usedQuestionIndices.Clear();
            }
            //obtener un índice aleatorio que no se haya utilizado
            do
            {
                randomQuestionIndex = Random.Range(0, responseList.Count);
            } while (usedQuestionIndices.Contains(randomQuestionIndex));
            //agregar el índice a la lista de utilizados
            usedQuestionIndices.Add(randomQuestionIndex);

            _correctAnswer = GameManager.Instance.responseList[randomQuestionIndex].CorrectOption;
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer1);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer2);
            _answers.Add(GameManager.Instance.responseList[randomQuestionIndex].Answer3);
            _answers.Shuffle();

            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];
                int index = i; // Captura el valor actual de i en una variable local -- SINO NO FUNCA!
                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            _numQuestionAnswered++;
            timer = initTime;
            UIManagment.Instance.startTimer = true;

            UIManagment.Instance.queryCalled = true;
            //chequear si ya se mostraron todas las preguntas disponibles
            if (_numQuestionAnswered >= TotalQuestions + 1)
            {
                Debug.Log("¡Has respondido todas las preguntas!");
            }
        }
    }
}

