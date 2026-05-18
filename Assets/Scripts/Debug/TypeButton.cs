using UnityEngine;
using UnityEngine.UI;

public class TypeButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool clearTypes;
    [SerializeField] private TypeColor type = TypeColor.WHITE;

    private void Start() => GetComponent<Button>().onClick.AddListener(() =>
    {
        if (clearTypes) PlayerController.playerController.ClearTypes();
        else PlayerController.playerController.AddType(type);
    });
}
