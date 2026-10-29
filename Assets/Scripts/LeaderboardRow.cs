using UnityEngine;
using TMPro;

namespace CEVideojocs.Bloc2
{
    public class LeaderboardRow : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _usernameText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        /// <summary>
        /// Inicialitza les dades visuals de la fila.
        /// </summary>
        public void Setup(int rank, string username, int score)
        {
            if (_rankText != null) _rankText.text = $"#{rank}";
            if (_usernameText != null) _usernameText.text = username;
            if (_scoreText != null) _scoreText.text = $"{score} PTS";
        }
    }
}