using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Enemy enemy;
    public Wave[] waves;

    LivingEntity playerEntity;
    Transform playerT; //플레이어계속추적

    int enemiesRemaingAlive; //살아있는적의수 
    int enemiesRemainingToSpawn; // 남아있는 스폰할적
    float nextSpawnTime; //다음스폰할식나
    Wave currentWaves; //편의를 위해서 현재웨이브의 레퍼런스를 갖고옴
    int currentWaveNumber; //현재 웨이브횟수

    MapGenerator map;

    float timeCampingCheck = 2f;
    float nextCampingCheck;

    float campDistance = 1.5f; //
    Vector3 campPositionOld;

    bool isCamping;

    bool isDisabled; //플레이어 죽으면 비활성 시킬 기능 

    public event System.Action<int> OnNewWave; //map index증가 

    void Start() {

        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampingCheck = timeCampingCheck + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;


       map =  FindObjectOfType<MapGenerator>();
        NextWave();

       
    }


    void Update() {

        if (!isDisabled)
        {
            if (Time.time > nextCampingCheck)
            {
                nextCampingCheck = Time.time + timeCampingCheck;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campDistance);
                campPositionOld = playerT.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWaves.timeBetweenSpawns;

                StartCoroutine(SpanwnEnemy());
            }
        }  
    }

    IEnumerator SpanwnEnemy()
    {
        float spawnDelay = 1; //적이소환되기전까지 반짝이는 시간
        float tileFlashSpeed = 4f;

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color freshColor = Color.blue;
        float spawnTimer = 0;
        
        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, freshColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null; //한프레임
        }


        Enemy spawnedEnemey = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemey.OnDeath += OnEnemyDeath; //spawneEnemy.OnDeath + onEnemyDeath
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }
    void OnEnemyDeath()
    {
       enemiesRemaingAlive--;
        if (enemiesRemaingAlive == 0)
        {
            NextWave();
        }
        // print("Enemy Die"); 
    }

    //플레이어 리셋 
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up;
    }

    void NextWave()
    {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) //배열예외설정
        {
            currentWaves = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWaves.enemyCount;
            enemiesRemaingAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);

             
            }
            ResetPlayerPosition();
        }
    }
    [System.Serializable] //오브젝트1차원배열로변경

    public class Wave
    {
        public int enemyCount; //적의수
        public float timeBetweenSpawns;//스폰간격
                                       //웨이브의 단계에 따라 적의 이동속도라던지 게임진행이어렵게
    }




}
