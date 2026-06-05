using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform _cam;

    private void Start() => _cam = Camera.main.transform;

    private void LateUpdate() => transform.rotation = _cam.rotation;
}
