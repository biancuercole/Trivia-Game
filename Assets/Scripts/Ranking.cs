using UnityEngine;
using Supabase;
using System.Threading.Tasks;
using Supabase.Interfaces;
using Postgrest.Models;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Ranking: MonoBehaviour
{
    string supabaseUrl = "https://uljrheyookexdvvzvzns.supabase.co";
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVsanJoZXlvb2tleGR2dnp2em5zIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI1NjQ1MjksImV4cCI6MjA0ODE0MDUyOX0.USQ8d_7qlGsbmQT5VixpP1q5v-DqeBRY0DTrLzRj3AY";
    public Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    List<intentos> intentosList = new List<intentos>();
    List<usuarios> usuariosList = new List<usuarios>();
    [SerializeField] TMP_Dropdown _dropdown;
    [SerializeField] TMP_Text generalRankingText; 
    [SerializeField] TMP_Text categoryRankingText; 

    public static int SelectedTriviaId { get; private set; }
    public static Ranking Instance { get; private set; }

    public DatabaseManager databaseManager;

    async void Start()
    {
        Instance = this;
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();

        await LoadIntentosData();
        await LoadUsuariosData();

        // Suscribirse al evento OnValueChanged del Dropdown
        _dropdown.onValueChanged.AddListener(OnCategoryDropdownValueChanged);

        // Mostrar el ranking general al inicio
        ShowGeneralRanking();
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
        }
    }

    async Task LoadIntentosData()
    {
        var response = await clientSupabase
            .From<intentos>()
            .Select("*")
            .Get();

        if (response != null)
        {
            intentosList = response.Models.ToList();
        }
    }

    async Task LoadUsuariosData()
    {
        var response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();

        if (response != null)
        {
            usuariosList = response.Models.ToList();
        }
    }

    void PopulateDropdown()
    {
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }


    void ShowGeneralRanking()
    {
        var sortedUsers = intentosList.OrderByDescending(x => x.puntaje).Take(7);;

        string generalRankingText = "";

    foreach (var intento in sortedUsers)
    {
        var user = usuariosList.FirstOrDefault(u => u.id == intento.id_usuario);
        if (user != null)
        {
            generalRankingText += $"{user.username,-20} {intento.puntaje,5}\n";  
        }
    }
        // muestra el texto en el UI
        this.generalRankingText.text = generalRankingText;
    }


    void ShowCategoryRanking(string category)
    {
        // obtiene la categoría seleccionada del Dropdown
        var selectedCategory = trivias.FirstOrDefault(t => t.category == category);

        if (selectedCategory != null)
        {
            // filtra los intentos por la categoría seleccionada y ordena por puntaje descendente
            var categoryUsers = intentosList.Where(x => x.id_categoria == selectedCategory.id).OrderByDescending(x => x.puntaje).Take(7);

            string categoryRankingText = "";
        foreach (var intento in categoryUsers)
        {
            var user = usuariosList.FirstOrDefault(u => u.id == intento.id_usuario);
            if (user != null)
            {
                categoryRankingText += $"{user.username,-20} {intento.puntaje,5}\n";
            }
        }
            // muestra el texto en el UI
            this.categoryRankingText.text = categoryRankingText;
        }
    }

    // Método llamado cuando se cambia la selección en el Dropdown
    void OnCategoryDropdownValueChanged(int index)
    {
        string selectedCategory = _dropdown.options[index].text;
        ShowCategoryRanking(selectedCategory);
    }


     public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}