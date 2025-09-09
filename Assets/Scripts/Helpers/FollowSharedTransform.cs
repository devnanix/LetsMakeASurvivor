using UnityEngine;

public class FollowSharedTransform : MonoBehaviour
{
    [SerializeField] private SharedTransform sharedTransform;

    private void LateUpdate()
    {
        transform.position = sharedTransform.shared.position;
    }
}
