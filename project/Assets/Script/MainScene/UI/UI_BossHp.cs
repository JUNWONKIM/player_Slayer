using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_BossHp : MonoBehaviour
{
    public Slider hpSlider; // ü���� ǥ���� �����̴�

   
    public void SetBossHp(BossHP bossHP) //���� hp ����
    {
        if (bossHP != null && hpSlider != null)
        {
            hpSlider.maxValue = bossHP.maxHP;
            hpSlider.value = bossHP.currentHP;

          
            StartCoroutine(UpdateHpBar(bossHP)); //���� hp ������Ʈ
        }
    }

    private IEnumerator UpdateHpBar(BossHP bossHP) //���� hp ������Ʈ
    {
        
        while (bossHP != null && bossHP.currentHP > 0) //boss�� ��� ���� ��
        {
            hpSlider.value = bossHP.currentHP;
            yield return null; 
        }

       
        hpSlider.gameObject.SetActive(false); //������ ������ ��Ȱ��ȭ
    }
}
