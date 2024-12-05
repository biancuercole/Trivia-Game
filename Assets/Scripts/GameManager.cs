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

    Supabase.Client clientSupabase;

    public int currentTriviaIndex = 0;
    public int _numQuestionAnswered = 0;

    public bool queryCalled;

    public int points;
    public int totalPoints = 0;
    public int correctAnswers = 0;
    public float timer;
    public float timeLeft;
    public float initTime = 10f;

    string supabaseUrl = "https://uljrheyookexdvvzvzns.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVsanJoZXlvb2tleGR2dnp2em5zIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI1NjQ1MjksImV4cCI6MjA0ODE0MDUyOX0.USQ8d_7qlGsbmQT5VixpP1q5v-DqeBRY0DTrLzRj3AY";

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
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
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
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        int userId = SupabaseManager.CurrentUserId;
        int categoryId = TriviaSelection.SelectedTriviaId;
        int finalScore = totalPoints;
        int totalTime = Mathf.RoundToInt(timer);
        int correct = correctAnswers;

        SaveData(userId, categoryId, finalScore, totalTime, correct);
        SceneManager.LoadScene("Results");
    }

    public async void SaveData(int userId, int categoryId, int finalScore, int totalTime, int correct)
    {
        var lastId = await clientSupabase
            .From<intentos>()
            .Select("id")
            .Order(intentos => intentos.id, Postgrest.Constants.Ordering.Descending) // ordena en orden descendente para obtener el último id
            .Get();
        int newId = 1;

        if (lastId.Models.Count > 0)
        {
            newId = lastId.Models[0].id + 1; 
        }

        var newSave = new intentos
        {
            id = newId,
            id_usuario = userId,
            id_categoria = categoryId,
            puntaje = finalScore,
            tiempo = totalTime,
            resp_correct = correct
        };

        // insertar el nuevo intento en Supabase
        var resultado = await clientSupabase
            .From<intentos>()
            .Insert(new[] { newSave });

        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            Debug.Log("Intento guardado correctamente en Supabase.");
        }
        else
        {
            Debug.LogError("Error al guardar el intento en Supabase: " + resultado.ResponseMessage);
        }
    }

    public void AddPoints(int pointsAdded)
    {
        totalPoints += pointsAdded;
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }
}

