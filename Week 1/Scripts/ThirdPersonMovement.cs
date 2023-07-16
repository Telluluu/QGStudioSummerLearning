using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public Animator animator;
    float idleSecond = 0;

    //public float jumpSpeed = 2.0f;
    //public float gravity = 9.8f;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    //旋转时间，越小完成旋转越快

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        //旋转速率
        float turnSmoothVelocity = 0;


        idleSecond += Time.deltaTime;
        //if (Input.GetKeyDown("space"))
        //{
        //    animator.SetBool("isJumping", true);
        //    animator.SetBool("isWalking", false);
        //    animator.SetBool("isIdleA", false);
        //    direction.y = jumpSpeed;
        //}
        //else
        if ((Input.GetKey("up")) || (Input.GetKey("right")) || (Input.GetKey("down")) || (Input.GetKey("left")) || Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s") || Input.GetKey("a"))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdleA", false);
        }
        else if(idleSecond >= 15)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdleA", false);
            animator.SetTrigger("idleB");
            idleSecond = 0;
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdleA", true);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }
}
