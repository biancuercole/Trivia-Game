using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://uljrheyookexdvvzvzns.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVsanJoZXlvb2tleGR2dnp2em5zIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI1NjQ1MjksImV4cCI6MjA0ODE0MDUyOX0.USQ8d_7qlGsbmQT5VixpP1q5v-DqeBRY0DTrLzRj3AY"; //COMPLETAR

    Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public static int SelectedTriviaId { get; private set; } 

    public static TriviaSelection Instance { get; private set; } 

    public DatabaseManager databaseManager;

    async void Start()
    {
        Instance = this; 
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();
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

    public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;

        string selectedTrivia = _dropdown.options[selectedIndex].text;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex+1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void backButton()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
