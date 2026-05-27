using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Parameter")] 
    [SerializeField] private float timer = 300f;
    
    public static GameManager Instance { get; private set; }
    
    public float Timer => timer;
    
    private bool _gameFinished;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (_gameFinished) return;
        
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            OnTimerFinished();
        }
    }

    private void OnTimerFinished()
    {
        _gameFinished = true;
    }
}