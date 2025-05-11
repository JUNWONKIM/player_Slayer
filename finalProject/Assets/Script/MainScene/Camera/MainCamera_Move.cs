using UnityEngine;
using System.Collections;

public class MainCamera_Move : MonoBehaviour
{
    public float moveSpeed = 100f; // ī�޶� �̵� �ӵ�
    public float zoomSpeed = 5000f; // �� ��/�ƿ� �ӵ�
    public float mouseBorderWidth = 10f; // ȭ�� �𼭸� ��
    public float maxZoomHeight = 100f; // �ִ� �� �� ���� ����
    public float minZoomHeight = 10f; // �ּ� �� �ƿ� ���� ����

    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical"); // ws Ű�� ���� �յ� �̵�
        float horizontalInput = Input.GetAxis("Horizontal"); // ad Ű�� ���� �¿� �̵�

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // ī�޶��� ������ ����Ͽ� �̵� ���� ���
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // y�� �̵� ����
        forward.y = 0;
        right.y = 0;

        Vector3 move = forward * verticalInput + right * horizontalInput;
        move.Normalize();

        // �̵� ���� ���
        Vector3 movement = move * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // ���콺 ��ũ���� ���� �� ��/�ƿ�
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomMovement = new Vector3(0, scroll * -zoomSpeed * Time.deltaTime, 0);
        Vector3 newPosition = transform.position + zoomMovement;

        // �� ��/�ƿ� ���� ���� ����
        newPosition.y = Mathf.Clamp(newPosition.y, minZoomHeight, maxZoomHeight);
        transform.position = newPosition;

       // ���콺�� ȭ�� ������ �и� �� �������� �̵�
        //Vector3 mousePosition = Input.mousePosition;
        //Vector3 moveVector = Vector3.zero;

        //if (mousePosition.x < mouseBorderWidth) // �������� �̵�
        //{
        //    moveVector -= right;
        //}
        //else if (mousePosition.x > Screen.width - mouseBorderWidth) // ���������� �̵�
        //{
        //    moveVector += right;
        //}

        //if (mousePosition.y < mouseBorderWidth) // �Ʒ��� �̵�
        //{
        //    moveVector -= forward;
        //}
        //else if (mousePosition.y > Screen.height - mouseBorderWidth) // ���� �̵�
        //{
        //    moveVector += forward;
        //}

        //transform.Translate(moveVector.normalized * moveSpeed * Time.deltaTime, Space.World);
    }
}
