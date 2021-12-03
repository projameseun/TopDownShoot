﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    public Rigidbody myRigibody;

    public float forceMin; //질량
    public float forceMax;

    float lifetime = 4f;
    float fadetime = 2f;
	void Start () {

        float force = Random.Range(forceMin, forceMax);
        myRigibody.AddForce(transform.right * force);
        myRigibody.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
	}
	
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
