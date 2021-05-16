using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Test : MonoBehaviour
{
    public float speed = 10f;

    public BasicGround basicGround;

    private Transform target;
    private int wavePointIndex = 0;


    void Start()
    {
        //if (GameObject.Find("WayPoints_2"))
        //{
        //    Debug.Log("find");
            
        //}

        //basicGround = GameObject.Find("WayPoints_2").GetComponent<BasicGround>();
        target = basicGround.points[0];


        //target = GameObject.Find("WayPoints_2").

        //target = BasicGround.points[0];//前进目标 = 第X个transform点
    }


    void Update()
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.02f)
        {
            GetNextWayPonint();
        }


    }


    void GetNextWayPonint()
    {
        if(wavePointIndex >= basicGround.points.Length - 1)
        {
            Destroy(gameObject);
            return;
        }



        wavePointIndex++ ;
        target = basicGround.points[wavePointIndex];


    }







}
