using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_time : MonoBehaviour
{
    public Text timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        // ��� �ð� ������Ʈ
        elapsedTime += Time.deltaTime;

        // �а� �� ���
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        // "00:00" �������� �ؽ�Ʈ ����
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
