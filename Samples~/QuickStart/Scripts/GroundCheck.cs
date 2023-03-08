using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Transform groundCheckObject;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    public bool IsGrounded()
    {
        if (groundCheckObject == null)
        {
            groundCheckObject = transform;
        }
        return Physics.CheckSphere(groundCheckObject.position, groundDistance, groundMask);
    }
}
