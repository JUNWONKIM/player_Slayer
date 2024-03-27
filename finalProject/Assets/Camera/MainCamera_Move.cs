using UnityEngine;

public class MainCamera_Move : MonoBehaviour
{
    public float moveSpeed = 100f; // 카메라 이동 속도
    public float zoomSpeed = 5000f; // 줌 인/아웃 속도
    public float mouseBorderWidth = 10f; // 화면 끝으로 마우스를 밀 때의 폭

    void Update()
    {
        // 상하좌우 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        // 마우스 스크롤을 통한 줌 인/아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(0, 0, scroll * zoomSpeed * Time.deltaTime, Space.Self);

        // 마우스를 화면 끝으로 밀면 그 방향으로 이동
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveVector = Vector3.zero;

        if (mousePosition.x < mouseBorderWidth) // 왼쪽으로 이동
        {
            moveVector -= transform.right;
        }
        else if (mousePosition.x > Screen.width - mouseBorderWidth) // 오른쪽으로 이동
        {
            moveVector += transform.right;
        }

        if (mousePosition.y < mouseBorderWidth) // 아래로 이동
        {
            moveVector += transform.forward;
        }
        else if (mousePosition.y > Screen.height - mouseBorderWidth) // 위로 이동
        {
            moveVector -= transform.forward;
        }

        transform.Translate(moveVector.normalized * moveSpeed * Time.deltaTime);
    }
}