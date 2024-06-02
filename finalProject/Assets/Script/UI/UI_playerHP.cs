using UnityEngine;
using UnityEngine.UI;

public class UI_playerHP : MonoBehaviour
{
    public Text playerHPText; // UI Text 요소

    private PlayerHP playerHP; // 플레이어의 HP를 관리하는 스크립트

    void Start()
    {
        // 플레이어 오브젝트에서 PlayerHP 스크립트를 가져옴
        playerHP = FindObjectOfType<PlayerHP>();

        // 만약 PlayerHP 스크립트를 찾을 수 없으면 에러를 출력하고 스크립트를 비활성화
        if (playerHP == null)
        {
            Debug.LogError("PlayerHP 스크립트를 찾을 수 없습니다!");
            enabled = false;
        }
    }

    void Update()
    {
        // UI Text 요소에 플레이어의 HP를 표시
        if (playerHPText != null && playerHP != null)
        {
            playerHPText.text = "HP: " + playerHP.hp.ToString(); // HP 값을 문자열로 변환하여 표시
        }
    }
}
