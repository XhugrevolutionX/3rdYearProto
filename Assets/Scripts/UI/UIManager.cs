using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI timerTmp;
    
    private GameManager _gameManager;
    
    private void Start() => _gameManager = GameManager.Instance;

    private void Update()
    {
        int minutes = (int)_gameManager.Timer / 60;
        int seconds = (int)_gameManager.Timer % 60;
        timerTmp.SetText($"{minutes:00}:{seconds:00}");
    }
}
