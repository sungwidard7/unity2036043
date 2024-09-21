using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public float speed = 5.0f;  // �⺻ �̵� �ӵ�
    public float runSpeed = 10.0f;  // �޸��� �ӵ�
    public float jumpForce = 2.0f;  // ���� ��
    public float gravity = 9.81f;  // �߷� ��
    public float groundCheckDistance = 0.2f;  // ���� üũ �Ÿ�
    public float mouseSensitivity = 300.0f;  // ���콺 ����
    public Transform playerCamera;  // �÷��̾� ī�޶�

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float xRotation = 0f;

    // ���� ���� ���� ����
    private bool isGrounded;
    public LayerMask groundMask;  // ������ ���� ���̾�

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // ���콺 Ŀ���� ��׾� ���� ȭ�鿡�� Ŀ���� �� ���̰� ����
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // �÷��̾� �̵� �� ȸ�� ó��
        MovePlayer();
        RotatePlayer();

        // ���� ����ִ��� Raycast�� ����
        GroundCheck();
    }

    void MovePlayer()
    {
        // �޸��� ����� ���� �¿� �̵� �ӵ� ����
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;

        // ���� �̵� �Է� ó�� (WASD Ű �Է¿� ����)
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        moveDirection.x = move.x * moveSpeed;
        moveDirection.z = move.z * moveSpeed;

        // ���� ó��: ���� ������� ���� ���� ����
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpForce;
        }
        else if (!isGrounded)
        {
            // ���߿� ���� �� �߷� ����
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // ĳ���� ��Ʈ�ѷ��� �̵� ����
        controller.Move(moveDirection * Time.deltaTime);
    }

    void RotatePlayer()
    {
        // ���콺 �Է¿� ���� ȸ�� ó��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // �÷��̾� �¿� ȸ��
        transform.Rotate(Vector3.up * mouseX);

        // ī�޶� ���� ȸ�� ���� (-90������ +90�� ����)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // ī�޶� ȸ�� ����
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void GroundCheck()
    {
        // �÷��̾��� �� �Ʒ��� Raycast�� ���� ������� �浹�� ����
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.down * (controller.height / 2);

        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundMask);

        // ����׿�: ����ĳ��Ʈ �ð������� ǥ�� (���� ����)
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);
    }
}
