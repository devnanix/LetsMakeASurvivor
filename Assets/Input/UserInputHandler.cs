using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UserInputHandler", menuName = "Scriptable Objects/UserInputHandler")]
public class UserInputHandler : ScriptableObject, UserInput.IControlActions
{
    private UserInput cachedInput;
    private UserInput input => cachedInput ??= new UserInput();

    public UnityAction<Vector2> Locomotion;
    public UnityAction Jump;

    private void OnEnable()
    {
        input.Enable();
        input.Control.SetCallbacks(this);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void OnLocomotion(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Locomotion?.Invoke(input);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jump?.Invoke();
    }
}
