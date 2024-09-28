using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Cursor : MonoBehaviour
{
    public Sprite validCursorSprite; // 소환 가능 범위 마우스 커서 스프라이트
    public Sprite invalidCursorSprite; // 소환 불가 범위 마우스 커서 스프라이트
    public CreatureSpawner creatureSpawner; // CreatureSpawner 스크립트 참조
    public UI_Setting uiSetting; // UI_Setting 스크립트 참조

    public Vector2 cursorOffset = new Vector2(-2f, -10f); // 커서 위치 조정

    private Image cursorImage;
    private RectTransform rectTransform;

    void Start()
    {
        cursorImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // 기본 커서 비활성화

        // 커서 이미지의 앵커를 중앙으로 설정
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (uiSetting != null && uiSetting.IsSettingsPanelActive()) //설정창 활성화 시
        {
            cursorImage.sprite = validCursorSprite; 
            UpdateCursorPosition(); 
        }
        else //일반 게임 화면
        {
            UpdateCursor(); 
        }
    }

    void UpdateCursor() // 커서 위치에 따른 스프라이트 업데이트
    {
        UpdateCursorPosition();

       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 위치를 월드 공간으로 변환
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //마우스 커서와 땅의 충돌지점을 찾음
        {
            Vector3 worldCursorPosition = hit.point; //충돌 지점의 좌표 저장

            
            bool isValidSpawnPosition = creatureSpawner.IsWithinSpawnRange(worldCursorPosition); // 커서 위치가 스폰 범위 안에 있는지 확인

          
            cursorImage.sprite = isValidSpawnPosition ? invalidCursorSprite : validCursorSprite;   // 위치에 따라 스프라이트 설정 
        }
    }

    void UpdateCursorPosition() // 커서 위치 업데이트
    {
    
        Vector2 cursorPosition;

        //마우스 좌표를 rectTransform 안에서의 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPosition
        ); 
        cursorPosition += cursorOffset; //위치 보정
        rectTransform.anchoredPosition = cursorPosition; //커서 출력
    }
}
