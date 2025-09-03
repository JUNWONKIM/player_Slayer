using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject groundTilePrefab; // 땅 프리팹
    public float tileSize = 50f; //크기
    public float tileSpacing = 125f; // 타일 간의 간격
    public int viewDistance = 5; // 용사 주변에 생성될 타일의 범위
    public Transform tilesParent; // 타일을 자식으로 넣을 부모 오브젝트

    private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>(); //좌표와 오브젝트 관리 딕셔너리
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        GenerateInitialTerrain(); //시작 시 땅 생성
    }

    void Update()
    {
        if (playerTransform == null) return; //용사가 존재하지 않을 시 리턴

        GenerateTerrainAroundPlayer(); //용사 주변 땅 생성
        RemoveDistantTiles();// 멀어진 타일 제거
    }

    void GenerateInitialTerrain() //초기 땅 생성
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord(); //용사 좌표 확인

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z);
                CreateTileAt(tileCoord);
            }
        }
    }

    void GenerateTerrainAroundPlayer() //용사 주변 타일 생성
    {
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2 tileCoord = new Vector2(playerPosition.x + x, playerPosition.y + z); //새로운 타일 좌표 생성

                if (!terrainTiles.ContainsKey(tileCoord)) //해당 좌표에 타일이 없으면
                {
                    CreateTileAt(tileCoord); //타일 생성
                }
            }
        }
    }

    void RemoveDistantTiles()// 멀어진 타일 제거
    {
        List<Vector2> tilesToRemove = new List<Vector2>();
        Vector2 playerPosition = GetCurrentPlayerTileCoord();

        foreach (var tile in terrainTiles)
        {
            if (Vector2.Distance(tile.Key, playerPosition) > viewDistance) //용사와 멀어진 타일 확인
            {
                tilesToRemove.Add(tile.Key);//제거할 타일 배열에 추가
            }
        }

        foreach (var tileCoord in tilesToRemove)
        {
            Destroy(terrainTiles[tileCoord]); //제거할 타일 배열 삭제
            terrainTiles.Remove(tileCoord);//딕셔너리에서 제거
        }
    }

    Vector2 GetCurrentPlayerTileCoord() //현재 용사가 위치한 타일 좌표 계산
    {
        //좌표를 타일 크기에 맞게 정렬
        float xCoord = Mathf.Floor(playerTransform.position.x / tileSpacing);
        float zCoord = Mathf.Floor(playerTransform.position.z / tileSpacing);

        return new Vector2(xCoord, zCoord);
    }

    void CreateTileAt(Vector2 tileCoord) //타일 생성
    {
        // 타일의 위치를 고정된 위치로 계산
        Vector3 tilePosition = new Vector3(tileCoord.x * tileSpacing, 0, tileCoord.y * tileSpacing);

        // 타일 위치와 회전을 고정
        GameObject newTile = Instantiate(groundTilePrefab, tilePosition, Quaternion.identity);

        // 타일의 부모를 설정
        newTile.transform.parent = tilesParent;

        newTile.transform.localScale = new Vector3(tileSize, 1, tileSize);

        // 타일 딕셔너리에 추가
        terrainTiles.Add(tileCoord, newTile);
    }
}
