using UnityEngine;

public class VFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem particle;
    
    public void PlayVFX() => particle.Play();
}
