using UnityEngine;
using UnityEngine.UI;

public class UI_cursor : MonoBehaviour
{
    public Sprite validCursorSprite; // 소환 가능 범위 마우스 커서 스프라이트
    public Sprite invalidCursorSprite; // 소환 불가 범위 마우스 커서 스프라이트
    public CreatureSpawner creatureSpawner; // CreatureSpawner 스크립트 참조

    private Image cursorImage;
    private RectTransform rectTransform;

    void Start()
    {
        cursorImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // 기본 시스템 커서 숨기기
    }

    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor()
    {
        // 마우스 위치를 따라 커서 위치 업데이트
        Vector2 cursorPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPosition
        );
        rectTransform.anchoredPosition = cursorPosition;

        // 뷰포트 공간의 마우스 위치를 월드 공간으로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldCursorPosition = hit.point;

            // 커서 위치가 spawnRange 안에 있는지 확인
            bool isValidSpawnPosition = creatureSpawner.IsWithinSpawnRange(worldCursorPosition);

            // 유효한 커서 상태에 따라 스프라이트 설정
            cursorImage.sprite = isValidSpawnPosition ? invalidCursorSprite : validCursorSprite;
        }
    }
}
