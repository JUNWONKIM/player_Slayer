using UnityEngine;
using System.Collections;
public class UI_CameraButton : MonoBehaviour
{
    private Camera mainCamera; // ���� ī�޶�
    private Vector3 initialPosition; // ī�޶� �ʱ� ��ġ
    private Quaternion initialRotation; // ī�޶� �ʱ� ȸ��

    void Start()
    {
       
        mainCamera = Camera.main;

        // �ʱ� ī�޶��� ��ġ�� ȸ���� ����
        initialPosition = mainCamera.transform.position;
        initialRotation = mainCamera.transform.rotation;
    }

    void Update()
    {

       
        if (Input.GetKeyDown(KeyCode.T))  // T Ű�� ������ �÷��̾�� ī�޶� �̵�
        {
            MoveCameraToObjectWithTag("Player");
        }

      
        if (Input.GetKeyDown(KeyCode.Y))  // Y Ű�� ������ ������ ī�޶� �̵�
        {
            MoveCameraToObjectWithTag("Boss");
        }
    }

    
    public void ResetCameraToInitialPosition()// ī�޶� �ʱ�ȭ
    {
        // ī�޶��� ��ġ�� ȸ���� �ʱⰪ���� ����
        mainCamera.transform.position = initialPosition;
        mainCamera.transform.rotation = initialRotation;
    }

    
    public void MoveCameraToObjectWithTag(string tag)// Ư�� �±׸� ���� ������Ʈ�� ī�޶� �̵� (ȸ���� ���̴� �������� ����)
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(tag);
        if (targetObject != null)
        {
            // ���� ī�޶��� ȸ���� ���� ������ ������
            Vector3 cameraPosition = mainCamera.transform.position;
            Quaternion cameraRotation = mainCamera.transform.rotation;

            // ī�޶��� ����(y)�� ����
            Vector3 targetPosition = targetObject.transform.position;
            targetPosition.y = cameraPosition.y;

           
            Vector3 directionToTarget = targetPosition - cameraPosition; //ī�޶� �̵� ���� ����
            directionToTarget = Quaternion.Inverse(cameraRotation) * directionToTarget;

            // ���� ��� (Y = 100�� �� Z = -40)
            float baseY = 100f;
            float baseZ = -40f;

            // ���� Y ��ǥ�� ���� Z ��ǥ ���
            float currentY = cameraPosition.y;
            float adjustedZ = baseZ * (currentY / baseY);

            // ī�޶��� ��ġ�� ����
            Vector3 adjustedTargetPosition = cameraPosition + (cameraRotation * directionToTarget);
            adjustedTargetPosition.z = adjustedTargetPosition.z + adjustedZ; // Z ��ǥ�� ����

           
            mainCamera.transform.position = adjustedTargetPosition;  // ī�޶��� ��ġ�� ������Ʈ

       
            mainCamera.transform.rotation = cameraRotation;     // ī�޶��� ȸ���� �������� ����
        }
       
    }

}

