using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string sceneName;
    
    private void Awake() => GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(sceneName));
}
