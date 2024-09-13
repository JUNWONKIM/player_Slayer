using UnityEngine;
using UnityEngine.UI;

public class UI_Cursor : MonoBehaviour
{
    public Sprite validCursorSprite; // 소환 가능 범위 마우스 커서 스프라이트
    public Sprite invalidCursorSprite; // 소환 불가 범위 마우스 커서 스프라이트
    public CreatureSpawner creatureSpawner; // CreatureSpawner 스크립트 참조
    public UI_Setting uiSetting; // UI_Setting 스크립트 참조

    public Vector2 cursorOffset = new Vector2(-2f, -10f); // 커서 이미지 오프셋 (왼쪽으로 이동)

    private Image cursorImage;
    private RectTransform rectTransform;

    void Start()
    {
        cursorImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // 기본 시스템 커서 숨기기

        // 커서 이미지의 앵커를 중앙으로 설정
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void LateUpdate()
    {
        if (uiSetting != null && uiSetting.IsSettingsPanelActive())
        {
            cursorImage.sprite = validCursorSprite;
            UpdateCursorPosition(); // 커서 위치만 업데이트
        }
        else
        {
            UpdateCursor(); // 일반 모드에서 커서 업데이트
        }
    }

    void UpdateCursor()
    {
        UpdateCursorPosition();

        // 뷰포트 공간의 마우스 위치를 월드 공간으로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldCursorPosition = hit.point;

            // 커서 위치가 spawnRange 안에 있는지 확인
            bool isValidSpawnPosition = creatureSpawner.IsWithinSpawnRange(worldCursorPosition);

            // 유효한 커서 상태에 따라 스프라이트 설정 (역할 바꾸기)
            cursorImage.sprite = isValidSpawnPosition ? invalidCursorSprite : validCursorSprite;
        }
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
        cursorPosition += cursorOffset; // 오프셋 적용
        rectTransform.anchoredPosition = cursorPosition;
    }
}
