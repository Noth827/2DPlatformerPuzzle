using System.Linq;
using App.Scripts.Inputs;
using UnityEngine;

namespace App.Scripts.PhysicsActors
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField] [Tooltip("Max speed, in units per second, that the character moves.")]
        private float _speed;

        [SerializeField] [Tooltip("Acceleration while grounded.")]
        private float _walkAcceleration;

        [SerializeField] [Tooltip("Acceleration while in the air.")]
        private float _airAcceleration;

        [SerializeField] [Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
        private float _groundDeceleration;

        [SerializeField] private float _gravity;


        [SerializeField] [Tooltip("Max height the character will jump regardless of gravity")]
        private float _jumpHeight;

        [SerializeField] private InputProvider _input;

        private BoxCollider2D _boxCollider;
        private Rigidbody2D _rigidbody2D;

        private const float RayMargin = 0.01f;

        /// <summary>
        ///     Set to true when the character intersects a collider beneath
        ///     them in the previous frame.
        /// </summary>
        private bool _isGrounded;

        private bool _isHitCeilFlag;

        private Vector2 _velocity;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var moveInput = _input.GetAxisX;

            if (_isGrounded && _input.IsJumpPressed)
            {
                _isGrounded = false;
                _velocity.y = Mathf.Sqrt(2f * _jumpHeight * Mathf.Abs(_gravity));
            }

            var acceleration = _isGrounded ? _walkAcceleration : _airAcceleration;
            var deceleration = _isGrounded ? _groundDeceleration : 0;

            if (moveInput != 0)
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, _speed * moveInput, acceleration * Time.deltaTime);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
            }


            if (_isGrounded)
            {
                _velocity.y = 0;
                _isHitCeilFlag = false;
            }
            else
            {
                // 重力
                _velocity.y += _gravity * Time.deltaTime;
            }

            if (IsHittingCeiling())
            {
                if (_isHitCeilFlag == false)
                {
                    _velocity.y = 0;
                    _isHitCeilFlag = true;
                }
            }

            _rigidbody2D.velocity = _velocity;

            _isGrounded = IsGrounded();
        }

        private bool IsGrounded()
        {
            var pos = transform.position;
            var rayLength = _boxCollider.size.y * 0.5f + 0.1f;
            var hits = Physics2D.RaycastAll(pos, Vector2.down, rayLength);
            var hitsLeft = Physics2D.RaycastAll(new Vector2((pos.x - _boxCollider.size.x * 0.5f + RayMargin), pos.y),
                Vector2.down, rayLength);
            var hitsRight = Physics2D.RaycastAll(new Vector2((pos.x + _boxCollider.size.x * 0.5f - RayMargin), pos.y),
                Vector2.down, rayLength);


            if (hits.Any(hit => hit.collider != _boxCollider)) return true;
            if (hitsLeft.Any(hit => hit.collider != _boxCollider)) return true;
            if (hitsRight.Any(hit => hit.collider != _boxCollider)) return true;

            return false;
        }

        private bool IsHittingCeiling()
        {
            var pos = transform.position;
            var rayLength = _boxCollider.size.y * 0.5f + 0.1f;
            var hits = Physics2D.RaycastAll(pos, Vector2.up, rayLength);
            var hitsLeft = Physics2D.RaycastAll(new Vector2((pos.x - _boxCollider.size.x * 0.5f + RayMargin), pos.y),
                Vector2.up, rayLength);
            var hitsRight = Physics2D.RaycastAll(new Vector2((pos.x + _boxCollider.size.x * 0.5f - RayMargin), pos.y),
                Vector2.up, rayLength);

            if (hits.Any(hit => hit.collider != _boxCollider)) return true;
            if (hitsLeft.Any(hit => hit.collider != _boxCollider)) return true;
            if (hitsRight.Any(hit => hit.collider != _boxCollider)) return true;

            return false;
        }
    }
}