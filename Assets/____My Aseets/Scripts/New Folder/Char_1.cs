using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 负责角色的基础逻辑

public class Char_1 : MonoBehaviour
{

    // 攻击
    [Header("Attributes")]
    public float fireRate = 1f; //攻击频率
    public float turnSpeed = 10f; //单位旋转速度（不重要）

    private float fireCountDown = 0f;



    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";



    public CharAttackRange charAttackRange;
    private Transform target;  //将要攻击的对象
    public Transform rotateDirection;  //单位的旋转部分（不重要）
    public GameObject bulletPrefab; //投射物prefab
    public Transform firePoint; // 投射物发射位置


    //public bool isEnemyInAttackRange = false;










    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f); //用Update每帧索敌的话太吃资源，因此用该方法来重复调用索敌方法
    }



    void Update()
    {
        if( target == null)  //确保敌人为空的情况下，不执行攻击逻辑
        {
            return;
        }

        // 单位旋转（其实我应该用不到，我不需要旋转单位，只需要根据target和自身的相对位置来判断需要播的动画即可）
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(rotateDirection.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;

        rotateDirection.rotation = Quaternion.Euler(0f, rotation.y, 0f);



        // 攻击逻辑
        if(fireCountDown <= 0f) //射击倒计时小于等于0时
        {
            Shoot(); //执行射击
            fireCountDown = 1f / fireRate; //重置射击倒计时
        }

        fireCountDown -= Time.deltaTime; //射击倒计时随着时间减少

    }





    //索敌用方法
    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); //寻找场景中的所有敌人

        float shortestDistance = Mathf.Infinity;  //初始状态下，所有敌人与本单位的距离都视为无限
        GameObject nearestEnemy = null;


        //计算所有敌人到本单位的距离
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);



            if(distanceToEnemy < shortestDistance) //如果该敌人与本单位的距离，小于最小距离
            {
                shortestDistance = distanceToEnemy;  //则最小距离 = 该敌人与本单位的距离
                nearestEnemy = enemy;  //并且将该敌人视为距离本单位最近的敌人


            }


        }

        if(nearestEnemy != null && charAttackRange.enemiesInAttackRange.Count != 0) //TODO:当前单位会攻击场景中所有敌人，哪怕该敌人并不在collider当中。需要修复   //如果存在最近的敌人，且敌人数组不为空
        {
            target = nearestEnemy.transform;  // 将距离最近的敌人transform赋值给target


            #region 判断敌人相对单位的方位
            //TODO: 根据敌人的相对方位，判断动画播放
            if (target.position.x < transform.position.x) // 敌人在单位左边时
            {
                if (target.position.z < transform.position.z) //敌人在单位下方时
                {
                    //左下
                }
                else if (target.position.z > transform.position.z) //敌人在单位上方时
                {
                    //左上
                }
            }

            else if (target.position.x > transform.position.x) // 敌人在单位右边时
            {
                if (target.position.z < transform.position.z) //敌人在单位下方时
                {
                    //右下
                }
                else if (target.position.z > transform.position.z) //敌人在单位上方时
                {
                    //右上
                }
            }
            #endregion

        }

        else
        {
            target = null;
        }


    }



    //远距离攻击
    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // 生成投射物
        Bullet bullet = bulletGO.GetComponent<Bullet>(); // 将生成的投射物上的Bullet Component赋值给bullet

        if( bullet != null)
        {
            bullet.Seek(target); // 执行Seek方法，寻找攻击对象，攻击对象为target
        }

    }





}
