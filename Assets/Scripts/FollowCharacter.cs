using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;

    [SerializeField] private Vector3 _offsetPosition = new Vector3(0, 25, -25);

    private void Update()
    {
        transform.position = _objectToFollow.position + _offsetPosition;
    }
}
