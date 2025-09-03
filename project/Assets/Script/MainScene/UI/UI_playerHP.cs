using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_playerHP : MonoBehaviour
{
    public Slider hpSlider; //hp 슬라이더 참조
    public PlayerHP playerHP; //용사 hp 참조

    void Start()
    {
        hpSlider.maxValue = playerHP.max_hp; // 최대 체력을 슬라이더의 최대 값으로 설정
       
        hpSlider.value = playerHP.hp; // 현재 체력을 슬라이더의 초기 값으로 설정
    }

    void Update()
    {
        hpSlider.value = playerHP.hp; //슬라이더의 값을 플레이어의 현재 체력으로 업데이트
    }
}
