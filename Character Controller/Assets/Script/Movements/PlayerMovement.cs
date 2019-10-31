using System;
using UnityEngine;

namespace Script.Movements
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        #region PUBLIC FIELDS

        [Header("Walk / Run Setting")] public float walkSpeed;
        public float runSpeed;

        [Header("Jump Settings")] public float playerJumpForce;
        public ForceMode appliedForceMode;

        [Header("Jumping State")] public bool playerIsJumping;

        [Header("Current Player Speed")] public float currentSpeed;

        #endregion


        #region PRIVATE FIELDS

        private float _xAxis;
        private float _zAxis;
        private Rigidbody _rb;
        private RaycastHit _hit;
        private Vector3 _groundLocation;
        private bool _isCapslockPressedDown;

        #endregion

        #region MONODEVELOP ROUTINES

        private void Start()
        {
            #region Initializing Components

            _rb = GetComponent<Rigidbody>();

            #endregion
        }

        private void Update()
        {
            #region Controller Input

            _xAxis = Input.GetAxis("Horizontal");
            _zAxis = Input.GetAxis("Vertical");

            #endregion

            #region Adjust Player Speed

            currentSpeed = _isCapslockPressedDown ? runSpeed : walkSpeed;

            #endregion

            #region Player Jump Status

            playerIsJumping = Input.GetButton("Jump");

            #endregion

            #region Disable Multiple Jumps

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 10f, Color.blue);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out _hit,
                Mathf.Infinity))
            {
                if (String.Compare(_hit.collider.tag, "ground", StringComparison.Ordinal) == 0)
                {
                    _groundLocation = _hit.point;
                }

                var distanceFromPlayerToGround = Vector3.Distance(transform.position, _groundLocation);
                if (distanceFromPlayerToGround > 1f)
                    playerIsJumping = false;
            }

            #endregion
        }

        private void FixedUpdate()
        {
            #region Move Player

            _rb.MovePosition(transform.position + Time.deltaTime * currentSpeed * 
                             transform.TransformDirection(_xAxis, 0f, _zAxis));

            #endregion

            #region Player Jump Applied

            if(playerIsJumping)
                PlayerJump(playerJumpForce, appliedForceMode);

            #endregion
        }

        private void OnGUI()
        {
            _isCapslockPressedDown = Event.current.capsLock;
        }

        #endregion

        #region HELPER ROUTINES

        private void PlayerJump(float jumpForce, ForceMode forceMode)
        {
            _rb.AddForce(jumpForce * _rb.mass * Time.deltaTime * Vector3.up, forceMode);
        }

        #endregion
    }
}
