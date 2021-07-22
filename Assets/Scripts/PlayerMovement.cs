using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView PV;

    public CharacterController controller;
    public float speed = 12.0f;

    Vector3 velocity;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    public bool paused = false;

    private CinemachineVirtualCamera cinemachine = null;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            cinemachine = FindObjectOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera;
            cinemachine.Follow = transform;
            (FindObjectOfType(typeof(MouseLook)) as MouseLook).playerBody = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && !paused)
        {
            Movement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
