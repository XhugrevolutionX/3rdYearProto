using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI timerTmp;
    [SerializeField] private TextMeshProUGUI bossHealthTmp;
    [SerializeField] private Slider bossHealthBarSlider;
    
    public static UIManager Instance { get; private set; }
    
    public TextMeshProUGUI BossHealthTmp => bossHealthTmp;
    public Slider BossHealthBarSlider => bossHealthBarSlider;
    
    private GameManager _gameManager;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnTimerEnd += () => bossHealthBarSlider.transform.parent.gameObject.SetActive(true);
    }

    private void Update()
    {
        int minutes = (int)_gameManager.Timer / 60;
        int seconds = (int)_gameManager.Timer % 60;
        timerTmp.SetText($"{minutes:00}:{seconds:00}");
    }
}
