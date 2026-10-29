using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace CEVideojocs.Bloc2
{
    public class AuthManager : MonoBehaviour
    {
        [Header("Formulari UI (Repte 1.1)")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private Button _submitButton;
        [SerializeField] private TextMeshProUGUI _statusText;

        [Header("Leaderboard & Scroll View (Repte 1.2)")]
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GameObject _rowPrefab;

        [Header("Configuració de Colors")]
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private Color _errorColor = Color.red;

        private EventSystem _eventSystem;
        private List<string> _registeredUsers = new List<string>();
        
        // Bandera de seguretat per evitar la doble execució en el mateix fotograma
        private bool _isProcessingSubmit = false;

        private void Awake()
        {
            _eventSystem = EventSystem.current;

            // Vinculació dinàmica per codi
            if (_submitButton != null)
            {
                _submitButton.onClick.RemoveAllListeners(); // Netejem per evitar dobles crides
                _submitButton.onClick.AddListener(OnSubmitButtonClicked);
            }

            if (_usernameInput != null)
            {
                _usernameInput.onEndEdit.RemoveAllListeners();
                _usernameInput.onEndEdit.AddListener(OnUsernameEndEdit);
            }

            if (_emailInput != null)
            {
                _emailInput.onEndEdit.RemoveAllListeners();
                _emailInput.onEndEdit.AddListener(OnEmailEndEdit);
            }
        }

        private void Start()
        {
            if (_statusText != null) _statusText.text = string.Empty;

            // Generem els 10 usuaris inicials
            GenerateInitialLeaderboard(10);

            if (_usernameInput != null) _usernameInput.Select();
        }

        private void Update()
        {
            // Navegació amb Tabulador
            if (Input.GetKeyDown(KeyCode.Tab) && _eventSystem != null)
            {
                if (_usernameInput.isFocused) _emailInput.Select();
                else if (_emailInput.isFocused) _usernameInput.Select();
            }
        }

        /// <summary>
        /// En prémer Enter al primer camp, salta al segon camp.
        /// </summary>
        private void OnUsernameEndEdit(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_emailInput != null) _emailInput.Select();
            }
        }

        /// <summary>
        /// En prémer Enter al segon camp, envia el formulari.
        /// </summary>
        private void OnEmailEndEdit(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSubmitButtonClicked();
            }
        }

        /// <summary>
        /// Processa l'enviament del formulari de registre.
        /// </summary>
        private void OnSubmitButtonClicked()
        {
            // Protecció: Si ja s'està processant un enviament en aquest fotograma, ignora la crida
            if (_isProcessingSubmit) return;

            string username = _usernameInput.text.Trim();
            string email = _emailInput.text.Trim();

            // 1. Validació de camps buits
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                ShowFeedback("Error: Tots els camps són obligatoris.", _errorColor);
                return;
            }

            // 2. Validació de la '@'
            if (!email.Contains("@"))
            {
                ShowFeedback("Error: El correu electrònic ha d'incloure una '@'.", _errorColor);
                return;
            }

            // Activem la protecció de seguretat
            _isProcessingSubmit = true;

            // 3. Procés d'èxit: Afegim l'usuari
            int randomScore = Random.Range(1000, 9999);
            AddUserToLeaderboard(username, randomScore);

            // 4. Mostrem el feedback verd
            ShowFeedback($"Usuari {username} registrat amb èxit!", _successColor);

            // 5. Netejem els camps
            ClearForm();

            // Desactivem la protecció al final del fotograma
            StartCoroutine(ResetSubmitFlagCoroutine());
        }

        private IEnumerator ResetSubmitFlagCoroutine()
        {
            yield return new WaitForEndOfFrame();
            _isProcessingSubmit = false;
        }

        private void GenerateInitialLeaderboard(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string dummyUser = $"NetRunner_{i:00}";
                int dummyScore = 10000 - (i * 750);
                AddUserToLeaderboard(dummyUser, dummyScore);
            }

            StartCoroutine(RebuildLayoutCoroutine());
        }

        private void AddUserToLeaderboard(string username, int score)
        {
            if (_rowPrefab == null || _contentTransform == null) return;

            _registeredUsers.Add(username);
            int currentRank = _registeredUsers.Count;

            GameObject newRowObj = Instantiate(_rowPrefab, _contentTransform);
            
            if (newRowObj.TryGetComponent<LeaderboardRow>(out var rowUI))
            {
                rowUI.Setup(currentRank, username, score);
            }

            StartCoroutine(RebuildLayoutCoroutine());
        }

        private IEnumerator RebuildLayoutCoroutine()
        {
            yield return new WaitForEndOfFrame();
            if (_contentTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_contentTransform.GetComponent<RectTransform>());
            }
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
            _usernameInput.Select();
        }

        private void OnDestroy()
        {
            if (_submitButton != null) _submitButton.onClick.RemoveAllListeners();
            if (_usernameInput != null) _usernameInput.onEndEdit.RemoveAllListeners();
            if (_emailInput != null) _emailInput.onEndEdit.RemoveAllListeners();
        }
    }
}