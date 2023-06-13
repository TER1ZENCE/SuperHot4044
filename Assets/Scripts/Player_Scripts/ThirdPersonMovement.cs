using UnityEngine;
using DG.Tweening;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonMovement : MonoBehaviour
    {

        [Header("Movement")]
        public float walkSpeed = 2f;
        public float runSpeed = 5.335f;
        public float speed;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Header("Rotation")]
        public float RotationSmoothTime = 0.12f;
        public float turnSmoothVelocity;
        private bool _rotateOnMove = true;

        [Space(10)]
        [Header("Animation")]
        float walkBlendTreePrameter;

        [Space(10)]
        [Header("Gravity")]
        public float gravity = -9.81f;
        public Transform groundCheck;
        public float groundDistance = 0.5f;
        public LayerMask groundMask;
        private Vector3 velocity;
        [SerializeField] private bool isGrounded;


        [Space(10)]
        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;
        [Range(1,2)]
        public float LookSensitivity = 2;

        [Space(10)]
        [Header("Crouching Or Sprinting")]
        private bool isCrouching;
        private bool isSprinting;


        [Space(10)]
        [Header("Audio")]
        [SerializeField] private AudioSource footStepsAudioSource;
        [SerializeField] private AudioClip[] concreteStepsClips;
        [SerializeField] private float baseStepSpeed = 0.5f;
        [SerializeField] private float crouchStepMultiplier = 0.6f;
        [SerializeField] private float sprintStepMultiplier =0.6f;
        private float footStepTimer = 0f;
        public float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _terminalVelocity = -53f;


        //References
#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private Camera _mainCamera;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;
        private bool action;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }
        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
        }


        void Update()
        {

            GravityChecker();
            Move();
            CheckTimeScale();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += _input.look.x * LookSensitivity;
                _cinemachineTargetPitch += _input.look.y * LookSensitivity;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
                _cinemachineTargetYaw, 0.0f);   
        }

        private void CheckTimeScale()
        {
            float time = _input.move.magnitude > 0.1 ? 1f : .03f;
            float lerpTime = _input.move.magnitude > 0.1 ? .05f : .5f;
            time = action ? 1 : time;
            lerpTime = action ? .1f : lerpTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, time, lerpTime);
        }

        public IEnumerator ActionE()
        {
            action = true;
            yield return new WaitForSecondsRealtime(.03f);
            action = false;
        }


        private void GravityChecker()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (velocity.y > _terminalVelocity)
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }

        private void Move()
        {
                float targetSpeed = _input.sprint ? runSpeed : walkSpeed;

                if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                action = false;
            }
                    _speed = targetSpeed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;


                if (_input.move != Vector2.zero)
                {
                action = true;
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                        RotationSmoothTime);
                if(_rotateOnMove) 
                { 
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }


                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime)
                    + new Vector3(0f, velocity.y, 0f) * Time.deltaTime);



            if (_input.move != Vector2.zero && !_input.sprint)
            {
                if(!isCrouching)
                isCrouching = true;

                if (isSprinting)
                    isSprinting = false;

                Walk();
            }
            else if (_input.move != Vector2.zero && _input.sprint)
            {
                if (!isSprinting)
                    isSprinting = true;

                if (isCrouching)
                    isCrouching = false;

                Run();
            }
            else if (_input.move == Vector2.zero)
            {
                if (isSprinting)
                    isSprinting = false;

                if (!isCrouching)
                    isCrouching = true;

                Idle();
            }
            Handle_Footsteps();
        }

        public void SetRotateOnMove(bool newRotateOnMove)
        {
            _rotateOnMove = newRotateOnMove;
        }    

        public void SetSensitivity(float newSensitivity)
        {
            LookSensitivity = newSensitivity;
        }
        private void Idle()
        {
            DOTween.To(() => walkBlendTreePrameter, x =>
            {
                walkBlendTreePrameter = x;
                _animator.SetFloat("Speed", walkBlendTreePrameter);
            }, 0f, 0.5f);
        }



        private void Walk()
        {
            DOTween.To(() => walkBlendTreePrameter, x =>
            {
                walkBlendTreePrameter = x;
                speed = walkSpeed;
                _animator.SetFloat("Speed", walkBlendTreePrameter);
            }, 0.5f, 0.5f);
        }

        private void Run()
        {
            DOTween.To(() => walkBlendTreePrameter, x =>
            {
                walkBlendTreePrameter = x;
                speed = runSpeed;
                _animator.SetFloat("Speed", walkBlendTreePrameter);
            }, 1f, 0.5f);
        }

        private void Handle_Footsteps()
        {
            if (!isGrounded) return;
            if (_input.move == Vector2.zero) return;

            footStepTimer -= Time.deltaTime;

            if (footStepTimer <= 0)
            {
                if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, 3) )
                {
                    switch (hit.collider.tag)
                    {
                        case "Footsteps/CONCRETE":
                            footStepsAudioSource.PlayOneShot(concreteStepsClips[Random.Range(0, concreteStepsClips.Length - 1)]);
                            break;
                    }
                }
            }

            footStepTimer = GetCurrentOffset;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}