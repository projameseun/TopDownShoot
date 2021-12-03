using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilty   {
                            //티타입 , t타입의 배열 seed는 랜덤값으 초기값
    public static T[] ShuffleArray<T>(T[] array,int seed) //
    {
        //가짜 랜덤생성기
        System.Random prng = new System.Random(seed);

       
        //-1을 넣은이유는 셔플할때 마지막은 생략해도되서
        for(int i =0; i<array.Length -1; i++)
        {
           // Debug.Log("TEST : " + array.Length);
            int randomIndex = prng.Next(i, array.Length);
            //셔플
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;

        }

        return array;
    }
	
}
