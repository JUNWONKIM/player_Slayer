using UnityEngine;
using UnityEngine.UI;

public class UI_playerHP : MonoBehaviour
{
    public Slider healthSlider;
    public PlayerHP playerHP;

    void Start()
    {
        // 최대 체력을 슬라이더의 최대 값으로 설정
        healthSlider.maxValue = playerHP.max_hp;
        // 현재 체력을 슬라이더의 초기 값으로 설정
        healthSlider.value = playerHP.hp;
    }

    void Update()
    {
        // 매 프레임마다 슬라이더의 값을 플레이어의 현재 체력으로 업데이트
        healthSlider.value = playerHP.hp;
    }
}
