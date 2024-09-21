using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public float speed = 5.0f;  // 기본 이동 속도
    public float runSpeed = 10.0f;  // 달리기 속도
    public float jumpForce = 2.0f;  // 점프 힘
    public float gravity = 9.81f;  // 중력 값
    public float groundCheckDistance = 0.2f;  // 지면 체크 거리
    public float mouseSensitivity = 300.0f;  // 마우스 감도
    public Transform playerCamera;  // 플레이어 카메라

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float xRotation = 0f;

    // 지면 감지 관련 변수
    private bool isGrounded;
    public LayerMask groundMask;  // 감지할 지면 레이어

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 마우스 커서를 잠그어 게임 화면에서 커서가 안 보이게 설정
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 플레이어 이동 및 회전 처리
        MovePlayer();
        RotatePlayer();

        // 땅에 닿아있는지 Raycast로 감지
        GroundCheck();
    }

    void MovePlayer()
    {
        // 달리기 기능을 위해 좌우 이동 속도 결정
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;

        // 수평 이동 입력 처리 (WASD 키 입력에 따라)
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        moveDirection.x = move.x * moveSpeed;
        moveDirection.z = move.z * moveSpeed;

        // 점프 처리: 땅에 닿아있을 때만 점프 가능
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpForce;
        }
        else if (!isGrounded)
        {
            // 공중에 있을 때 중력 적용
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 캐릭터 컨트롤러로 이동 적용
        controller.Move(moveDirection * Time.deltaTime);
    }

    void RotatePlayer()
    {
        // 마우스 입력에 따른 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 상하 회전 제한 (-90도에서 +90도 사이)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 카메라 회전 적용
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void GroundCheck()
    {
        // 플레이어의 발 아래로 Raycast를 쏴서 지면과의 충돌을 감지
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.down * (controller.height / 2);

        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundMask);

        // 디버그용: 레이캐스트 시각적으로 표시 (선택 사항)
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);
    }
}
