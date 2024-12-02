using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Results : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    void Start()
    {
        ShowResults();
        
    }

    public void ShowResults()
    {
        int finalScore = GameManager.Instance.GetTotalPoints();
        scoreText.text = "Puntaje final: " + finalScore.ToString();
    }
}
