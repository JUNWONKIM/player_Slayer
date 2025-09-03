using UnityEngine;
using System.Collections;
public class UI_CameraButton : MonoBehaviour
{
    private Camera mainCamera; // 메인 카메라
    private Vector3 initialPosition; // 카메라 초기 위치
    private Quaternion initialRotation; // 카메라 초기 회전

    void Start()
    {
       
        mainCamera = Camera.main;

        // 초기 카메라의 위치와 회전을 저장
        initialPosition = mainCamera.transform.position;
        initialRotation = mainCamera.transform.rotation;
    }

    void Update()
    {

       
        if (Input.GetKeyDown(KeyCode.T))  // T 키를 누르면 플레이어로 카메라 이동
        {
            MoveCameraToObjectWithTag("Player");
        }

      
        if (Input.GetKeyDown(KeyCode.Y))  // Y 키를 누르면 보스로 카메라 이동
        {
            MoveCameraToObjectWithTag("Boss");
        }
    }

    
    public void ResetCameraToInitialPosition()// 카메라 초기화
    {
        // 카메라의 위치와 회전을 초기값으로 설정
        mainCamera.transform.position = initialPosition;
        mainCamera.transform.rotation = initialRotation;
    }

    
    public void MoveCameraToObjectWithTag(string tag)// 특정 태그를 가진 오브젝트로 카메라 이동 (회전과 높이는 변경하지 않음)
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(tag);
        if (targetObject != null)
        {
            // 현재 카메라의 회전과 높이 정보를 가져옴
            Vector3 cameraPosition = mainCamera.transform.position;
            Quaternion cameraRotation = mainCamera.transform.rotation;

            // 카메라의 높이(y)를 유지
            Vector3 targetPosition = targetObject.transform.position;
            targetPosition.y = cameraPosition.y;

           
            Vector3 directionToTarget = targetPosition - cameraPosition; //카메라 이동 방향 설정
            directionToTarget = Quaternion.Inverse(cameraRotation) * directionToTarget;

            // 비율 계산 (Y = 100일 때 Z = -40)
            float baseY = 100f;
            float baseZ = -40f;

            // 현재 Y 좌표에 따른 Z 좌표 계산
            float currentY = cameraPosition.y;
            float adjustedZ = baseZ * (currentY / baseY);

            // 카메라의 위치를 조정
            Vector3 adjustedTargetPosition = cameraPosition + (cameraRotation * directionToTarget);
            adjustedTargetPosition.z = adjustedTargetPosition.z + adjustedZ; // Z 좌표를 조정

           
            mainCamera.transform.position = adjustedTargetPosition;  // 카메라의 위치를 업데이트

       
            mainCamera.transform.rotation = cameraRotation;     // 카메라의 회전은 변경하지 않음
        }
       
    }

}

