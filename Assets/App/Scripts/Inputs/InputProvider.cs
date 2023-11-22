using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App.Scripts.Inputs
{
    public class InputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        public IObservable<InputAction.CallbackContext> OnMoveLeft =>
            _input.actions["MoveLeft"].AsObservable().TakeUntilDestroy(gameObject);

        public IObservable<InputAction.CallbackContext> OnMoveRight =>
            _input.actions["MoveRight"].AsObservable().TakeUntilDestroy(gameObject);

        public int GetAxisX
        {
            get
            {
                var moveLeft = _input.actions["MoveLeft"].IsPressed() ? 1 : 0;
                var moveRight = _input.actions["MoveRight"].IsPressed() ? 1 : 0;

                return moveRight - moveLeft;
            }
        }

        public IObservable<InputAction.CallbackContext> OnJump =>
            _input.actions["Jump"].AsObservable().TakeUntilDestroy(gameObject);
        
        public bool IsJumpPressed => _input.actions["Jump"].IsPressed();
    }

    public static class InputProviderExtensions
    {
        public static IObservable<InputAction.CallbackContext> AsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.performed += h,
                h => action.performed -= h);
        }
    }
}