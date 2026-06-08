using UnityEngine;

public class Area : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private TypeColor type;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Texture2D whiteTexture;
    
    public TypeColor Type
    {
        get => type;
        set => type = value;
    }

    public void RemoveColor()
    {
        type = TypeColor.WHITE;
        renderer.material.SetTexture("_Texture2D", whiteTexture);
    }
}
