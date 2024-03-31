using UnityEngine;

public class Player_Test : MonoBehaviour
{
    public float moveSpeed = 5f; // 플레이어 이동 속도

    void Update()
    {
        // 방향키 입력을 받아 이동 방향을 설정
        float horizontalInput = Input.GetAxis("Player_Horizontal");
        float verticalInput = Input.GetAxis("Player_Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 이동 방향에 따라 이동
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}
