using UnityEngine;

public class Area : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private TypeColor type;
    
    public TypeColor Type => type;
}
