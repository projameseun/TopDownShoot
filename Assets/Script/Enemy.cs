using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMesh))]

public class Enemy : LivingEntity {

    public enum State { Idle, Chasing, Attacking }; //0,1,2

    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial;
    LivingEntity targetEntity; 

    Color originalColor;

    float attckDistanceThereshold = .5f; //적의 공격거리
    float timeBetweenAttacks = 1; //공격간격시간 시간을 안넣어주면 매프레임마다 공격해서 성능이 저하됨
    float nextAttackTime;//다음공격시간
    float damage = 1f;

    

    //적이 플레이어 안에 가지 들어오지않기위해서 양쪽의 반지름을 구한다
    float myCollisionRadius;  //
    float targetCollisionRadius;

    bool hasTarget; //타켓이 죽었는지 살았는지 판단 

    protected override void Start () {

        base.Start();


        
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;
     if(GameObject.FindGameObjectWithTag("Player") != null)
        { 
            currentState = State.Chasing; //추적 
            hasTarget = true; //타켓을 찾았을대 
            //player가 만약에 먼저 죽으면 태그에서 찾으면 오류가 난다 그러므로 태그를 없애보겟다 
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath; //이벤트 

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }


    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject,deathEffect.startLifetime);

        }

        base.TakeHit(damage, hitPoint, hitDirection);
        
    }


    void OnTargetDeath() //타켓 죽는곳 
    {
        hasTarget = false;
        //적의 상태 할일없음
        currentState = State.Idle;
    }
	
   

	void Update () {
        //다음공격시간
        if(hasTarget)
        { 
            if(Time.time > nextAttackTime)
            { 

             float sqrdtsToTarget = (target.position - transform.position).sqrMagnitude;

            //공격거리한계거리를 표면으로부터재는법 잘알아두기 + 반지름 추가된곳임
                if (sqrdtsToTarget < Mathf.Pow(attckDistanceThereshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {

        currentState = State.Attacking;

        //공격중일대 계속 쫓아가질원하지않는다

        pathfinder.enabled = false; 
        //시작점 /끝점
     
        Vector3 originalPosition = transform.position; //시작점
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);



        float percent = 0;// 0 일때 대기 1일때 공격
        float attackSpeed  = 3;
        skinMaterial.color = Color.red;

        bool hasAppliedDamage = false;//데미지를 적용중인가

        while(percent <=1)  //0 > 1 1> 0 
        {
            if(percent >= .5f && !hasAppliedDamage) //데미지입히지않앗다면
            {
                hasAppliedDamage = true; //아직 데미지입혀야된다
                targetEntity.TakeDamage(damage);
            }

            //targetEntity 플레이어가 맞을때 TakeHit 호출을할려고한다
            //문제는 데미지 그리고 레이저 hit 가필요하다 
            //그러므로 hit는 필요없으므로 데미지만 만든다 
            percent += Time.deltaTime * attackSpeed;

            //0 과 1 로만 왓다갓다하기위해서 lerp 보간을 이용한다 
            float interpolation = (-Mathf.Pow(percent, 2) + percent )*4;
            //transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        //공격 끝나는지점
        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
       
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f; //
        while(hasTarget)  //target != null 똑같은말이지만 bool을이용한게 더 갈끔하다 
        {
           if(currentState == State.Chasing)
           {
                Vector3 dirTotarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirTotarget * (myCollisionRadius + targetCollisionRadius + attckDistanceThereshold /2);
            if( !dead) //죽지않앗을대만 찾아야 에러가안나서 예외설정
            { 
            pathfinder.SetDestination(targetPosition);
            }
           }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
