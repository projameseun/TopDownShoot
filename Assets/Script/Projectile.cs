using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 10f;

    public LayerMask collisionMask;

    float damage = 1;

    float lifetime = 3;
    float skinWidth = .1f;

    void Start () {
        Destroy(gameObject,lifetime);

        //적이 가까이붙엇을대는 레이저가 정확히잡지못한다 
      //우리의 발사체와 겹쳐있는 모든 충돌체들의 배열을 얻음 
        Collider[] initalCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initalCollisions.Length > 0) //무언가 충돌이됫다는의미
        {
            //첫번째 녀석을준다 
            OnHitObject(initalCollisions[0],transform.position);
        }
	}

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update () {
        float moveDistance = speed * Time.deltaTime;

        transform.Translate(Vector3.forward * moveDistance);
        CheckCollisions(moveDistance);
    }

    void CheckCollisions (float moveDistance)
    {
        Ray ray = new Ray(transform.position , transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
