using UnityEngine;

public class MainCamera_Move : MonoBehaviour
{
    public float moveSpeed = 100f; // 카메라 이동 속도
    public float zoomSpeed = 5000f; // 줌 인/아웃 속도
    public float mouseBorderWidth = 10f; // 화면 끝으로 마우스를 밀 때의 폭

    void Update()
    {
        // ws 키를 통한 앞뒤 이동
        float verticalInput = Input.GetAxis("Vertical");
        // ad 키를 통한 좌우 이동
        float horizontalInput = Input.GetAxis("Horizontal");

        // 이동 벡터 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 카메라의 방향을 고려하여 이동 방향 계산
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0; // 카메라가 45도 기울여져 있으므로 y 방향 이동은 무시
        right.y = 0;

        Vector3 move = forward * verticalInput + right * horizontalInput;
        move.Normalize(); // 정규화

        // 이동 벡터 계산
        Vector3 movement = move * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // 마우스 스크롤을 통한 줌 인/아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(0, scroll * -zoomSpeed * Time.deltaTime, 0, Space.World);


        // 마우스를 화면 끝으로 밀면 그 방향으로 이동
       /* Vector3 mousePosition = Input.mousePosition;
        Vector3 moveVector = Vector3.zero;

        if (mousePosition.x < mouseBorderWidth) // 왼쪽으로 이동
        {
            moveVector -= right;
        }
        else if (mousePosition.x > Screen.width - mouseBorderWidth) // 오른쪽으로 이동
        {
            moveVector += right;
        }

        if (mousePosition.y < mouseBorderWidth) // 아래로 이동
        {
            moveVector -= forward;
        }
        else if (mousePosition.y > Screen.height - mouseBorderWidth) // 위로 이동
        {
            moveVector += forward;
        }

        transform.Translate(moveVector.normalized * moveSpeed * Time.deltaTime, Space.World);*/
    }
}
