using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Cursor : MonoBehaviour
{
    public Sprite validCursorSprite; // ��ȯ ���� ���� ���콺 Ŀ�� ��������Ʈ
    public Sprite invalidCursorSprite; // ��ȯ �Ұ� ���� ���콺 Ŀ�� ��������Ʈ
    public CreatureSpawner creatureSpawner; // CreatureSpawner ��ũ��Ʈ ����
    public UI_Setting uiSetting; // UI_Setting ��ũ��Ʈ ����

    public Vector2 cursorOffset = new Vector2(-2f, -10f); // Ŀ�� ��ġ ����

    private Image cursorImage;
    private RectTransform rectTransform;

    void Start()
    {
        cursorImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // �⺻ Ŀ�� ��Ȱ��ȭ

        // Ŀ�� �̹����� ��Ŀ�� �߾����� ����
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void Update()
    {
        if (uiSetting != null && uiSetting.IsSettingsPanelActive()) //����â Ȱ��ȭ ��
        {
            cursorImage.sprite = validCursorSprite; 
            UpdateCursorPosition(); 
        }
        else //�Ϲ� ���� ȭ��
        {
            UpdateCursor(); 
        }
    }

    void UpdateCursor() // Ŀ�� ��ġ�� ���� ��������Ʈ ������Ʈ
    {
        UpdateCursorPosition();

       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ���콺 ��ġ�� ���� �������� ��ȯ
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //���콺 Ŀ���� ���� �浹������ ã��
        {
            Vector3 worldCursorPosition = hit.point; //�浹 ������ ��ǥ ����

            
            bool isValidSpawnPosition = creatureSpawner.IsWithinSpawnRange(worldCursorPosition); // Ŀ�� ��ġ�� ���� ���� �ȿ� �ִ��� Ȯ��

          
            cursorImage.sprite = isValidSpawnPosition ? invalidCursorSprite : validCursorSprite;   // ��ġ�� ���� ��������Ʈ ���� 
        }
    }

    void UpdateCursorPosition() // Ŀ�� ��ġ ������Ʈ
    {
    
        Vector2 cursorPosition;

        //���콺 ��ǥ�� rectTransform �ȿ����� ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out cursorPosition
        ); 
        cursorPosition += cursorOffset; //��ġ ����
        rectTransform.anchoredPosition = cursorPosition; //Ŀ�� ���
    }
}
