using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 负责角色的攻击范围检测

public class CharAttackRange : MonoBehaviour
{
    #region 生成攻击范围collider数组

    //public Collider[] charAttackRangeBox;


    //void Awake()
    //{
    //    charAttackRangeBox = new Collider[transform.childCount]; // 有多少个子物体，charAttackRangeBox数组就有多少个Collider

    //    for (int i = 0; i < charAttackRangeBox.Length; i++) //当i小于数组的长度时，i++
    //    {
    //        charAttackRangeBox[i] = transform.GetChild(i).GetComponent<Collider>(); //transform数组point的第i个transform，等于该脚本附加的物件的第i个子物件的transform
    //    }

    //}

    #endregion

    public List<GameObject> enemiesInAttackRange = new List<GameObject>();






    #region 当游戏开始时，关闭子物体的rangeShow条纹显示

    public SpriteRenderer[] rangeShow;

    void Start()
    {
        rangeShow = new SpriteRenderer[transform.childCount]; // 有多少个子物体，charAttackRangeBox数组就有多少个Collider

        for (int i = 0; i < rangeShow.Length; i++) //当i小于数组的长度时，i++
        {
            rangeShow[i] = transform.GetChild(i).GetComponent<SpriteRenderer>(); //transform数组point的第i个transform，等于该脚本附加的物件的第i个子物件的transform
            rangeShow[i].enabled = false;
        }
    }

    #endregion









    void OnTriggerEnter(Collider other)
    {
        // 判断敌人进入攻击范围
        if (other.CompareTag("Enemy"))  
        {
            enemiesInAttackRange.Add(other.gameObject);  //添加该单位进数组


            Debug.Log(enemiesInAttackRange.Count + other.gameObject.name);

        }
    }



    void OnTriggerExit(Collider other)
    {
        // 判断敌人离开攻击范围
        if (other.CompareTag("Enemy"))
        {
            enemiesInAttackRange.Remove(other.gameObject); //移除该单位出数组
        }
    }






















    //在Scene窗口中画出该物体的BoxCollider范围
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(transform.position, gameObject.GetComponent<BoxCollider>().size);
    //}

















}
