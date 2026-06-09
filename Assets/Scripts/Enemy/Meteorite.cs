using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer meteoriteRenderer;

    public Renderer MeteoriteRenderer => meteoriteRenderer;
}
