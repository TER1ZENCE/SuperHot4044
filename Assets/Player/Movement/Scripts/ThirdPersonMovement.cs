using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
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
            float targetAngel = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngel, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngel, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (direction != Vector3.zero && !Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            Walk();
        }
        else if (direction != Vector3.zero && Input.GetKeyDown(KeyCode.LeftShift))
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
        anim.SetFloat("Speed", 0);
    }

    private void Walk()
    {
        speed = 2f;
        anim.SetFloat("Speed", 1f);
    }

    private void Run()
    {
        //speed = 5f;
        //anim.SetFloat("Speed",1f);
    }
}
