using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI timerTmp;
    [SerializeField] private GameManager gameOverPanel;
    
    [Header("Boss")]
    [SerializeField] private TextMeshProUGUI bossHealthTmp;
    [SerializeField] private Slider bossHealthBarSlider;
    
    [Header("Level System")]
    [SerializeField] private Slider playerLevelBarSlider;
    [SerializeField] private TextMeshProUGUI playerLevelTmp;
    
    public static UIManager Instance { get; private set; }
    
    public TextMeshProUGUI BossHealthTmp => bossHealthTmp;
    public Slider BossHealthBarSlider => bossHealthBarSlider;
    
    private GameManager _gameManager;
    private int _playerLevel;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnTimerEnd += () => bossHealthBarSlider.transform.parent.gameObject.SetActive(true);

        playerLevelBarSlider.minValue = 0f;
        playerLevelBarSlider.maxValue = 1f;
        playerLevelBarSlider.value    = 0f;
        playerLevelTmp.SetText("EXP 0 / LEVEL 0");
        
        PlayerLevelSystem playerLevelSystem = PlayerLevelSystem.Instance;

        playerLevelSystem.OnLevelUp += (level, _) => _playerLevel = level;
        playerLevelSystem.OnGainXP += (currentXP, xpToNextLevel) =>
        {
            playerLevelBarSlider.value = currentXP / (float)xpToNextLevel;
            playerLevelTmp.SetText($"EXP {currentXP} / LEVEL {_playerLevel}");
        };
    }

    private void Update()
    {
        int minutes = (int)_gameManager.Timer / 60;
        int seconds = (int)_gameManager.Timer % 60;
        timerTmp.SetText($"{minutes:00}:{seconds:00}");
    }
}
