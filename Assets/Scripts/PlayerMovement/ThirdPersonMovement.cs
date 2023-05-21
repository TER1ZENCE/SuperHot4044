using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.XR;

public class ThirdPersonMovement : MonoBehaviour
{

    //References
    public CharacterController controller;
    public Camera cam;
    private Animator anim;

    //Movement
    public float walkSpeed;
    public float runSpeed;
    public float speed;

    //Rotation
    public float turnSmoothTime;
    float turnSmoothVelocity;

    //Animation

    float walkBlendTreePrameter;

    //Gravity
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;




    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }


    void Update()
    {
        Move();
        GravityChecker();
    }

    private void GravityChecker()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngel, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            Walk();
        }
        else if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        else if (direction == Vector3.zero)
        {
            Idle();
        }
    }

    private void Idle()
    {
        DOTween.To(() => walkBlendTreePrameter, x => {
            walkBlendTreePrameter = x;
            speed = walkSpeed;
            anim.SetFloat("Speed", walkBlendTreePrameter);
        }, 0f, 0.5f);

       
    }

    private void Walk()
    {
        DOTween.To(() => walkBlendTreePrameter, x => {
            walkBlendTreePrameter = x;
            speed = walkSpeed;
            anim.SetFloat("Speed", walkBlendTreePrameter);
            }, 0.5f, 0.5f);
        
    }

    private void Run()
    {
        DOTween.To(() => walkBlendTreePrameter, x =>
        {
            walkBlendTreePrameter = x;
            speed = runSpeed;
            anim.SetFloat("Speed", walkBlendTreePrameter);
        }, 1f, 0.5f);
    }
}