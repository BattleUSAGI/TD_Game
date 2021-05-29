using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject impactEffect;

    private Transform target;
    public float speed = 70f;




    public void Seek (Transform _target)
    {
        target = _target; //将其他脚本引用的Seek方法，当中输入的参数，赋值给target
    }



    void Update()
    {
        if(target == null) //如果投射物找不到目标时（有时候敌人进入我方据点后就会自动Destroy了，此时投射物找不到目标）
        {
            Destroy(gameObject); //销毁投射物
            return; //当Destroy一个该脚本附加的gameObject时，记得跟一个return，避免执行后续的逻辑
        }

        //设置投射物的射击目标
        Vector3 dir = target.position - transform.position; //目标距离自身的距离
        float distanceThisFrame = speed * Time.deltaTime; // 投射物每帧的运动速度

        if(dir.magnitude <= distanceThisFrame) //当目标与自身的距离，小于等于当投射物每帧的运动速度时，才会执行下述代码。此处为了避免投射物飞得超出敌人的距离
        {
            HItTarget();
            return;
        }

        //设置投射物的运动
        transform.Translate(dir.normalized * distanceThisFrame, Space.World); 


    }

    void HItTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);

        Destroy(target.gameObject);

        Destroy(gameObject);



    }









}
