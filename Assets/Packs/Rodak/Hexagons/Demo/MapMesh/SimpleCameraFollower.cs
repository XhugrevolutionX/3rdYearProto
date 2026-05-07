using UnityEngine;

namespace Rodak.Hexagons.Demo.MapMesh
{
    /// <summary>
    /// A naive camera foolow script.
    /// </summary>
    public class SimpleCameraFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float lerpSpeed = 2f;

        private Vector3 initialOffset;

        private void Start()
        {
            initialOffset = transform.position - target.position;
        }

        private void Update()
        {
            Vector3 targetPosition = target.position + initialOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
        }
    }
}