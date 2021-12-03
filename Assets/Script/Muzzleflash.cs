using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzleflash : MonoBehaviour {

    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;

    void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);

        for(int i =0; i<spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime); //딜레이를 시간만큼 주고 함수를 실행시킨다 코루틴같은성향
    }

    void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
