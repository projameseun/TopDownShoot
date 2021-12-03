using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform obstaclePrefab;
    public Transform tilePrefab;
   // public Vector2 mapSize; //xx yy 값을 가진다 
    public Vector2 maxmapSize;  //navmesh 에대한 맵
    //navmesh 맵 전체 
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;

    public Transform[,] tileMap; //전체저장할 공간


    [Range(0, 1)] //0~에서 1 범위정함 
    public float outlinePercent; //테두리

    //[Range(0, 1)]
    //public float obstaclePercent;

    //public int seed = 10;

    public float tileSize;


   // Coord mapCentre;

    // List<자료형 또는 클래스> _list = new List<자료형 또는 클래스>();

    //※ List 또한 객체이기 때문에 꼭 new를 해주자.

    //모든타일에 대한 좌표리스트 선언
    List<Coord> allTileCoords;

    //Enqueue : 줄 맨 뒤에 데이터를 넣어준다.
    //Dequeue : 줄 맨 앞의 데이읽고 터를 제거한다.
    //Peek : 줄 맨 앞의 데이터를 읽어온다.
    Queue<Coord> shffuledTileCoords; //좌표들에대한 큐
    Queue<Coord> shffuledOpenTileCoords; //에너미 스폰 타일 
    //현재 저장할 맵변수 

    Map currentMap;




    private void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
       
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed); //높이
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);
        
        
        //코드 (좌표)생성ㅇ 
        allTileCoords = new List<Coord>(); //할당 

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
               // Debug.Log(allTileCoords);
            }

        }
        //새좌표큐를 할당  //Utility.ShufleArray 할당 좌표리스트를 ToArray로 호출해서 배열로 만듬 
        shffuledTileCoords = new Queue<Coord>(Utilty.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        //바로위에까지 하면 셔플이된 자료 큐를 얻을수있다. 

       // mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2); //맵정중앙 플레이어 스폰위치


        //자식을 묶을 오브젝트생성 
        //맵홀더 오브젝트생성 
        string holderName = "Generated Map";
        if (transform.Find (holderName))
        {
            DestroyImmediate (transform.Find  (holderName).gameObject); //무조건지우겟다
        }

        //새로할당을 해준다 

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;  //자식 , 부모 


        //타일 생성 스폰 
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder; //자식 , ㅂ모
                tileMap[x, y] = newTile;
            }
        }

        //장애물을 생성하기전에 2차원배열bool 만들어서 체크를해줘야된다 
        //장애물 생성 스폰 
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        //Debug.Log("test : " + obstacleMap);
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords); //초기화 
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeigt = Mathf.Lerp(currentMap.minObstacleHeigth, currentMap.maxObstalceHeigth, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeigt/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize,obstacleHeigt, (1 - outlinePercent) *tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y; //퍼센트를 지정하기 위해서 랜덤코드가 얼마나 앞으로 나왓는지 
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord); //장애물이 설치된곳 지우기
            }
            //조건이 맞지 않아서 팅겨나올때 
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--; //다시원상복귀 
            }
        }

        shffuledOpenTileCoords = new Queue<Coord>(Utilty.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
        
        
        //네비매쉬마스크 생성 
        Transform maskLeft = Instantiate(navmeshMaskPrefab,Vector3.left * (currentMap.mapSize.x + maxmapSize.x)/4f * tileSize,Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxmapSize.x - currentMap.mapSize.x) / 2f, 1f, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxmapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxmapSize.x - currentMap.mapSize.x) / 2f, 1f, currentMap.mapSize.y) * tileSize;

        //위아래는 맵전체 사이즈로 

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxmapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxmapSize.x, 1f, (maxmapSize.y - currentMap.mapSize.y)/2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxmapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxmapSize.x, 1f, (maxmapSize.y - currentMap.mapSize.y) / 2f) * tileSize;


        //바닥부분
        navmeshFloor.localScale = new Vector3(maxmapSize.x, maxmapSize.y) * tileSize;
    }
    //맵전체 접근가능한 메소드
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        //정중앙타일을 넣는거부터시작
        queue.Enqueue(currentMap.mapCentre);
        //그리고 중앙이 비어잇다는걸아니까
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1; //접근가능했떤 타일의수 

        //Flood fill 알고리즘 꼭알기 
        //Debug.Log(queue.Count);
        while (queue.Count > 0)
        {
            //이말은 큐에첫번째아이템을 가지고오고 그것을큐에서제거]
            //중점을 중심으로 바깥에 8개의 타일검사
            Coord tile = queue.Dequeue();
            
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0) ///대각선 방향은체크하지않겟다 
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) //이타일을 이전에 체크하지않앗다면 그리고 장애물이 없다면 
                            {
                                mapFlags[neighbourX, neighbourY] = true; ///타일에대한 체크완료 
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }

                    }
                }
            }

        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    //벡트3로 변환 시키는구간 



    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize; //tilesize를 곱해주면 간격을 늘릴수있다
    }

    //플레이어 위치 가지고오기 

    public Transform GetTileFromPosition(Vector3 position)
    {               //반올림함수
        int x = Mathf.RoundToInt (position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        //인덱스 오류 안나게 범위 설정정해줌
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];
    }


    //큐로 부터 다음아이템을 얻어 랜덤좌표를 반환해준다 
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shffuledTileCoords.Dequeue();
        shffuledTileCoords.Enqueue(randomCoord);
        return randomCoord;

    }

    //Enemy Spawn tile
    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shffuledOpenTileCoords.Dequeue();
        shffuledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }



    //모든좌표를 생성한다
    //system coord ㅁ붙이는 이유는 인스펙터에 원할때마다 나오게하기위해서 
    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeigth; //장애물최소높이
        public float maxObstalceHeigth; //장애물최대
        public Color foregroundColour;
        public Color backgroundColour; 

        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
