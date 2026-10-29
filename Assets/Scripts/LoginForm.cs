using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Indispensable per a gestionar el focus de la UI (EventSystem)
using TMPro;

namespace CEVideojocs.Bloc2
{
    public class LoginForm : MonoBehaviour
    {
        [Header("Components d'Entrada (UI)")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _emailInput;

        [Header("Controls i Feedback")]
        [SerializeField] private Button _submitButton;
        [SerializeField] private TextMeshProUGUI _statusText;

        [Header("Configuració de Colors (Feedback)")]
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private Color _errorColor = Color.red;

        private EventSystem _system;

        private void Awake()
        {
            // Guardem la referència a l'EventSystem de l'escena
            _system = EventSystem.current;

            // 1. Vinculació del botó
            if (_submitButton != null)
            {
                _submitButton.onClick.AddListener(OnSubmitButtonClicked);
            }

            // 2. Detectar Enter/Intro directament des dels Input Fields de TMP
            // L'esdeveniment 'onEndEdit' s'activa quan l'usuari prem Enter o perd el foc
            if (_usernameInput != null)
            {
                _usernameInput.onEndEdit.AddListener(OnInputEndEdit);
            }

            if (_emailInput != null)
            {
                _emailInput.onEndEdit.AddListener(OnInputEndEdit);
            }
        }

        private void Start()
        {
            if (_statusText != null)
            {
                _statusText.text = string.Empty;
            }

            // Opcional: Fer que el primer camp s'activi automàticament en obrir la pantalla
            if (_usernameInput != null)
            {
                _usernameInput.Select();
            }
        }

        private void Update()
        {
            // Detectar la tecla Tabulador per alternar entre camps
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                HandleTabNavigation();
            }
        }

        /// <summary>
        /// Canvia el foc del teclat entre el camp d'usuari i el de correu al prémer Tab.
        /// </summary>
        private void HandleTabNavigation()
        {
            if (_system == null) return;

            // Si estem al camp d'usuari, passem al de correu
            if (_usernameInput.isFocused)
            {
                _emailInput.Select();
            }
            // Si estem al camp de correu, tornem al d'usuari (o si es prem Shift+Tab)
            else if (_emailInput.isFocused)
            {
                _usernameInput.Select();
            }
        }

        /// <summary>
        /// S'executa quan l'usuari prem Intro/Enter dins de qualsevol InputField.
        /// </summary>
        private void OnInputEndEdit(string text)
        {
            // Comprovem si l'edició ha acabat perquè s'ha premut la tecla Enter/Submit
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // Si estàvem al primer camp, al prémer Enter passem al segon
                if (_usernameInput.isFocused)
                {
                    _emailInput.Select();
                }
                // Si estàvem al darrer camp (email), enviem el formulari directament
                else
                {
                    OnSubmitButtonClicked();
                }
            }
        }

        private void OnSubmitButtonClicked()
        {
            string username = _usernameInput.text.Trim();
            string email = _emailInput.text.Trim();

            // Validació de camps buits
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                ShowFeedback("Error: Tots els camps són obligatoris.", _errorColor);
                return;
            }

            // Validació de la '@'
            if (!email.Contains("@"))
            {
                ShowFeedback("Error: El correu electrònic ha d'incloure una '@'.", _errorColor);
                return;
            }

            // Èxit
            ShowFeedback($"Registre completat amb èxit! Benvingut/da, {username}.", _successColor);
            ClearForm();
        }

        private void ShowFeedback(string message, Color color)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
                _statusText.color = color;
            }
        }

        private void ClearForm()
        {
            _usernameInput.text = string.Empty;
            _emailInput.text = string.Empty;
        }

        private void OnDestroy()
        {
            if (_submitButton != null) _submitButton.onClick.RemoveListener(OnSubmitButtonClicked);
            if (_usernameInput != null) _usernameInput.onEndEdit.RemoveListener(OnInputEndEdit);
            if (_emailInput != null) _emailInput.onEndEdit.RemoveListener(OnInputEndEdit);
        }
    }
}