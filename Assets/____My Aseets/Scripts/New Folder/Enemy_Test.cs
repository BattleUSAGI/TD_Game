using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Test : MonoBehaviour
{
    public string wayPointName;  // 需要记得给每个敌人写上关卡内的路径点名称
    public float speed = 10f;

    private int wavePointIndex = 0;



    public BasicGround basicGround;

    private Transform target;



    void Start()
    {

        // 找到场景中对应的路径点名称，并把BasicGround组件赋值给字段，再调用字段
        basicGround = GameObject.Find(wayPointName).GetComponent<BasicGround>();
        target = basicGround.points[0];


        //basicGround = GameObject.Find("WayPoints_2").GetComponent<BasicGround>();

        //target = basicGround.points[0]; //前进目标 = 第X个transform点


        //target = GameObject.Find("WayPoints_2").

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
