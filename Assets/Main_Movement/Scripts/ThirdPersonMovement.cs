using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Camera cam;
    public float walkSpeed;
    public float runSpeed;
    public float speed;
    public float turnSmoothTime;
    float turnSmoothVelocity;

    float walkBlendTreePrameter;


    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }


    void Update()
    {
        Move();
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
        speed = runSpeed;
        anim.SetFloat("Speed",1f);
    }
}