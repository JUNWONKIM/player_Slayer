using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_selectCreature : MonoBehaviour
{
    public Button myButton;
    public int buttonIndex; // ��ư �ε���
    public float cooldownTime = 0f; // ��ư�� ��Ÿ�� �ð�
    public Color activeColor = Color.white; // ��ư Ȱ��ȭ ����
    public Color cooldownColor = Color.red; // ��ư ��Ÿ�� ����
    public Color selectedColor = Color.green; // ��ư ���� ����

    private ColorBlock originalColors; // ��ư�� ���� ���� ����
    private bool isOnCooldown = false; // ��ٿ� Ȯ��
    private float cooldownEndTime; // ��Ÿ���� ������ �ð��� ����
    private CreatureSpawner spawner; // CreatureSpawner ��ũ��Ʈ ����

    void Start()
    {
        if (myButton != null)
        {
            originalColors = myButton.colors; // ��ư�� ���� ���� ����
            myButton.onClick.AddListener(OnButtonClick); // ��ư Ŭ�� �̺�Ʈ ����
        }

        spawner = FindObjectOfType<CreatureSpawner>();
        UpdateButtonState(); // ��ư ���� ����
    }

    void Update()
    {
        if (spawner != null && myButton != null)
        {
            UpdateButtonState();
        }

        if (isOnCooldown)
        {
            UpdateCooldown();
        }
    }

    void UpdateButtonState()
    {
        if (spawner.selectedCreature == buttonIndex)
        {
            PressButton(); // ���õ� ��ư�� �ʷϻ����� ����
        }
        else
        {
            if (!isOnCooldown)
            {
                ReleaseButton(); // ���õ��� �ʾҰ� ��Ÿ���� ���� �� ���� ���·�
            }
        }
    }

    void PressButton() // ��ư�� ���õǾ��� ��
    {
        if (myButton.image != null)
        {
            myButton.image.color = selectedColor; // ���õ� ���¿��� �ʷϻ����� ����
        }
    }

    void ReleaseButton() // ��ư�� �����Ǿ��� ��
    {
        if (myButton.image != null && !isOnCooldown)
        {
            myButton.image.color = activeColor; // �⺻ Ȱ��ȭ ���·� ����
        }
    }

    void OnButtonClick() // ��ư Ŭ�� �� ��Ÿ�� ����
    {
        if (!isOnCooldown)
        {
            StartCooldown();
        }
    }

    public void StartCooldown() // ��Ÿ�� ����
    {
        isOnCooldown = true;
        cooldownEndTime = Time.time + cooldownTime;

        if (myButton.image != null)
        {
            myButton.image.color = cooldownColor; // ��Ÿ�� ���� ���������� ����
        }
    }

    void UpdateCooldown() // ��Ÿ�� ���� ��
    {
        float remainingTime = cooldownEndTime - Time.time; // ���� �ð� ���

        if (remainingTime <= 0) // ��Ÿ���� ������ ��
        {
            EndCooldown();
        }
        else
        {
            // ��Ÿ�� ���� ������ ���� �ٲ�� ����
            if (myButton.image != null)
            {
                myButton.image.color = Color.Lerp(cooldownColor, activeColor, 1 - (remainingTime / cooldownTime));
            }
        }
    }

    void EndCooldown() // ��Ÿ�� ���� ��
    {
        isOnCooldown = false;

        // ��Ÿ���� ������ ���� ���·� ����
        if (myButton.image != null)
        {
            myButton.image.color = activeColor;
        }

        // ���� ��ư�� ���õ� ���¶��, ���� �������� ���ư��� ó��
        if (spawner.selectedCreature == buttonIndex)
        {
            PressButton();
        }
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
}
