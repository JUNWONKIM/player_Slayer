using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_playerHP : MonoBehaviour
{
    public Slider hpSlider; //hp �����̴� ����
    public PlayerHP playerHP; //��� hp ����

    void Start()
    {
        hpSlider.maxValue = playerHP.max_hp; // �ִ� ü���� �����̴��� �ִ� ������ ����
       
        hpSlider.value = playerHP.hp; // ���� ü���� �����̴��� �ʱ� ������ ����
    }

    void Update()
    {
        hpSlider.value = playerHP.hp; //�����̴��� ���� �÷��̾��� ���� ü������ ������Ʈ
    }
}
