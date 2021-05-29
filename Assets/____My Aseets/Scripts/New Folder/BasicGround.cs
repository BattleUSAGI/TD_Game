using UnityEngine;

public class BasicGround : MonoBehaviour
{
    public Transform[] points;

    void Awake()
    {
        points = new Transform[transform.childCount]; // 有多少个子物体，points数组就有多少个transform



        for (int i = 0; i < points.Length; i++) //当i小于数组的长度时，i++
        {
            points[i] = transform.GetChild(i); //transform数组point的第i个transform，等于该脚本附加的物件的第i个子物件的transform
        }



    }













}
