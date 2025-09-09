using UnityEngine;

public class RegisterSharedTransform : MonoBehaviour
{
    [SerializeField] private SharedTransform sharedTransform;

    private void Awake()
    {
        sharedTransform.shared = transform;
    }
}
