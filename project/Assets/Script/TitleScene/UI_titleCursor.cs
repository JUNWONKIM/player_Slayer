using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_titleCursor : MonoBehaviour
{
    public Sprite customCursorSprite;  // 사용할 커서 스프라이트
    public Vector2 cursorOffset = new Vector2(-2f, -10f);  // 커서 이미지 오프셋

    private Image cursorImage;  // UI의 Image 컴포넌트
    private RectTransform rectTransform;  // RectTransform 참조

    void Start()
    {
        cursorImage = GetComponent<Image>();  // Image 컴포넌트 가져오기
        rectTransform = GetComponent<RectTransform>();  // RectTransform 가져오기
        Cursor.visible = false;  // 기본 시스템 커서 숨기기

        // 커서 이미지의 앵커를 중앙으로 설정
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        cursorImage.sprite = customCursorSprite;  // 커서 이미지 설정
    }

    void LateUpdate()
    {
        UpdateCursorPosition();  // 마우스 움직임에 따라 커서 위치 업데이트
    }

    void UpdateCursorPosition()
    {
        // 마우스 위치를 따라 커서 위치 업데이트
        Vector2 cursorPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPosition
        );
        cursorPosition += cursorOffset;  // 오프셋 적용
        rectTransform.anchoredPosition = cursorPosition;  // 커서 위치 설정
    }
}