using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SupabaseManager : MonoBehaviour
{
    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;
    [SerializeField] GameObject playB;

    string supabaseUrl = "https://uljrheyookexdvvzvzns.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVsanJoZXlvb2tleGR2dnp2em5zIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzI1NjQ1MjksImV4cCI6MjA0ODE0MDUyOX0.USQ8d_7qlGsbmQT5VixpP1q5v-DqeBRY0DTrLzRj3AY"; //COMPLETAR

    Supabase.Client clientSupabase;

    private usuarios _usuarios = new usuarios();

    private void Start()
    {
        playB.SetActive(false);
    }

    //INICIAR SESIÓN
    public async void UserLogin()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // prueba
        var test_response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);

        // filtro seg�n datos de login
        var login_password = await clientSupabase
          .From<usuarios>()
          .Select("password")
          .Where(usuarios => usuarios.username == _userIDInput.text)
          .Get();

        if (login_password.Models.Count > 0)
        {
            if (login_password.Model.password.Equals(_userPassInput.text))
            {
                _stateText.text = "LOGIN SUCCESSFUL";
                _stateText.color = Color.green;
                playB.SetActive(true);
            }
            else
            {
                _stateText.text = "WRONG PASSWORD";
                _stateText.color = Color.red;
            }
        } else //si se inicia con una cuenta que no existe
        {
            _stateText.text = "UNVALID USERNAME OR PASSWORD";
            _stateText.color = Color.red;
        }
    }

    //INSERTAR NUEVO USUARIO
    public async void InsertarNuevoUsuario()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // Verificar si el usuario ya existe
        var usuarioExistente = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Where(usuarios => usuarios.username == _userIDInput.text)
            .Get();
        if (usuarioExistente.Models.Count > 0) // Si el usuario ya existe
        {
            _stateText.text = "Nombre ya existente";
            _stateText.color = Color.red;
            return; // Salir de la función
        }

        // Consultar el último id utilizado
        var ultimoId = await clientSupabase
            .From<usuarios>()
            .Select("id")
            .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el último id
            .Get();

        int nuevoId = 1; // Valor predeterminado si la tabla está vacía

        if (ultimoId.Models.Count > 0)
        {
            nuevoId = ultimoId.Models[0].id + 1; // Incrementar el último id
        }

        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new usuarios
        {
            id = nuevoId,
            username = _userIDInput.text,
            age = Random.Range(0, 100), 
            password = _userPassInput.text,
        };

        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<usuarios>()
            .Insert(new[] { nuevoUsuario });

        // Verificar el estado de la inserción
        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
            playB.SetActive(true);
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            _stateText.text = resultado.ResponseMessage.ToString();
            _stateText.color = Color.red;
        }
    }

    public void playButton()
    {
        SceneManager.LoadScene("TriviaSelectScene"); 
    }
}

