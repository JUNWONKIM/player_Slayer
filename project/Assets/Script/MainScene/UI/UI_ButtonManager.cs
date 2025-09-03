using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonManager : MonoBehaviour
{
    public Button[] creatureButtons; // ��ư �迭 
    private PlayerHP playerHP;

    public Color disabledColor = Color.gray; // ��Ȱ��ȭ ���� ����
    public Color enabledColor = Color.white; // Ȱ��ȭ ���� ����
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHP = player.GetComponent<PlayerHP>();
        }

        // ó�� ��ư ���� ������Ʈ
        UpdateButtonStates();
    }

    void Update()
    {
        if (playerHP != null)
        {      
            UpdateButtonStates();  // �� �����Ӹ��� ��ư ���� ������Ʈ 
        }
    }


    void UpdateButtonStates()
    {
        // 1���� 2�� ��ư�� �׻� Ȱ��ȭ
        UpdateButtonState(creatureButtons[0], true);
        UpdateButtonState(creatureButtons[1], true);

        // 3�� ��ư�� ü���� 70% ������ ���� Ȱ��ȭ
        bool isCreature3Enabled = playerHP.hp <= playerHP.max_hp * 0.7f;
        UpdateButtonState(creatureButtons[2], isCreature3Enabled);

        // 4�� ��ư�� ü���� 50% ������ ���� Ȱ��ȭ
        bool isCreature4Enabled = playerHP.hp <= playerHP.max_hp * 0.5f;
        UpdateButtonState(creatureButtons[3], isCreature4Enabled);
    }

    void UpdateButtonState(Button button, bool isEnabled)
    {
        button.interactable = isEnabled;

        // ��ư ���� ����
        ColorBlock colors = button.colors;
        colors.disabledColor = disabledColor; // ��Ȱ��ȭ ���¿��� ȸ������ ����
        colors.normalColor = isEnabled ? enabledColor : disabledColor;
        button.colors = colors;
    }
}