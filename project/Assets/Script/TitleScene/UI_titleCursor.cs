using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_titleCursor : MonoBehaviour
{
    public Sprite customCursorSprite;  // ����� Ŀ�� ��������Ʈ
    public Vector2 cursorOffset = new Vector2(-2f, -10f);  // Ŀ�� �̹��� ������

    private Image cursorImage;  // UI�� Image ������Ʈ
    private RectTransform rectTransform;  // RectTransform ����

    void Start()
    {
        cursorImage = GetComponent<Image>();  // Image ������Ʈ ��������
        rectTransform = GetComponent<RectTransform>();  // RectTransform ��������
        Cursor.visible = false;  // �⺻ �ý��� Ŀ�� �����

        // Ŀ�� �̹����� ��Ŀ�� �߾����� ����
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        cursorImage.sprite = customCursorSprite;  // Ŀ�� �̹��� ����
    }

    void LateUpdate()
    {
        UpdateCursorPosition();  // ���콺 �����ӿ� ���� Ŀ�� ��ġ ������Ʈ
    }

    void UpdateCursorPosition()
    {
        // ���콺 ��ġ�� ���� Ŀ�� ��ġ ������Ʈ
        Vector2 cursorPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPosition
        );
        cursorPosition += cursorOffset;  // ������ ����
        rectTransform.anchoredPosition = cursorPosition;  // Ŀ�� ��ġ ����
    }
}