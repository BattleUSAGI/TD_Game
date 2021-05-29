﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_GameManager : MonoBehaviour
{

    private static CS_GameManager instance = null;
    public static CS_GameManager Instance { get { return instance; } }

    [SerializeField] int myMaxLife = 10;
    private int myCurrentLife;

    [SerializeField] GameObject[] myPlayerPrefabs = null;
    private List<CS_Player> myPlayerList = new List<CS_Player>();

    [SerializeField] GameObject myDirectionObject = null;

    private CS_Player myCurrentPlayer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // init total lives
        myCurrentLife = myMaxLife;
        CS_UIManager.Instance.SetLife(myCurrentLife);

        // init all players
        foreach (GameObject f_prefab in myPlayerPrefabs)
        {
            GameObject f_object = Instantiate(f_prefab, this.transform);
            f_object.SetActive(false);
            // get player script
            CS_Player f_player = f_object.GetComponent<CS_Player>();
            // add script to list
            myPlayerList.Add(f_player);
        }

        // init direction object
        myDirectionObject.SetActive(false);
    }

    public void SetMyCurrentPlayer(int g_index)
    {
        // dont do anything if its setting direction
        if (myDirectionObject.activeSelf == true)
        {
            return;
        }

        myCurrentPlayer = myPlayerList[g_index];
    }

    public void BeginDragPlayer()
    {
        // dont do anything if its setting direction
        if (myDirectionObject.activeSelf == true)
        {
            return;
        }

        myCurrentPlayer.gameObject.SetActive(true);
        myCurrentPlayer.Arrange();
        myCurrentPlayer.ShowHighlight();

        // set slow mode
        Time.timeScale = 0.1f;
    }

    public void DragPlayer()
    {
        // dont do anything if its setting direction
        if (myDirectionObject.activeSelf == true)
        {
            return;
        }

        // do raycast
        RaycastHit t_hit;
        Ray t_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(t_ray, out t_hit))
        {
            CS_Tile t_tile = t_hit.collider.gameObject.GetComponentInParent<CS_Tile>();
            if (t_tile != null)
            {
                if (t_tile.GetType() == myCurrentPlayer.GetTileType())
                {
                    Vector3 t_position = t_tile.transform.position;
                    t_position.y = t_hit.point.y;
                    myCurrentPlayer.transform.position = t_position;
                    return;
                }
            }

            myCurrentPlayer.transform.position = t_hit.point;
        }
    }

    public void EndDragPlayer()
    {
        // dont do anything if its setting direction
        if (myDirectionObject.activeSelf == true)
        {
            return;
        }

        // hide highlight
        myCurrentPlayer.HideHighlight();

        // do raycast
        RaycastHit t_hit;
        Ray t_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(t_ray, out t_hit))
        {
            CS_Tile t_tile = t_hit.collider.gameObject.GetComponentInParent<CS_Tile>();
            if (t_tile != null)
            {
                if (t_tile.GetType() == myCurrentPlayer.GetTileType())
                {
                    // show direction object
                    myDirectionObject.transform.position = myCurrentPlayer.transform.position;
                    myDirectionObject.SetActive(true);
                    return;
                }
            }
        }

        // reset current player
        myCurrentPlayer.gameObject.SetActive(false);
        myCurrentPlayer = null;
    }

    public void BeginDragDirection()
    {
    }

    public void DragDirection()
    {

        // do raycast
        RaycastHit t_hit;
        Ray t_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(t_ray, out t_hit))
        {
            Vector3 t_v2HitPos = new Vector3(t_hit.point.x, 0, t_hit.point.z);
            Vector3 t_v2PlayerPos = new Vector3(myCurrentPlayer.transform.position.x, 0, myCurrentPlayer.transform.position.z);
            if (Vector3.Distance(t_v2HitPos, t_v2PlayerPos) > 1)
            {
                // calculate forward
                Vector3 t_forward = t_v2HitPos - t_v2PlayerPos;
                if (Mathf.Abs(t_forward.x) > Mathf.Abs(t_forward.z))
                {
                    t_forward.z = 0;
                }
                else
                {
                    t_forward.x = 0;
                }

                // rotate
                myCurrentPlayer.transform.forward = t_forward;
                // show highlight
                myCurrentPlayer.ShowHighlight();
                return;
            }
        }
        // hide highlight
        myCurrentPlayer.HideHighlight();
    }

    public void EndDragDirection()
    {
        // do raycast

        // 很重要，看看作者是怎么实现鼠标停留的位置，选中对应的object的。参考视频 https://www.youtube.com/watch?v=_yf5vzZ2sYE
        RaycastHit t_hit;
        Ray t_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(t_ray, out t_hit))
        {
            Vector3 t_v2HitPos = new Vector3(t_hit.point.x, 0, t_hit.point.z);
            Vector3 t_v2PlayerPos = new Vector3(myCurrentPlayer.transform.position.x, 0, myCurrentPlayer.transform.position.z);
            if (Vector3.Distance(t_v2HitPos, t_v2PlayerPos) > 1) //若鼠标raycast的hit点，与玩家单位的当前位置，距离大于1
            {
                // hide highlight
                myCurrentPlayer.HideHighlight();
                // hide direction 
                myDirectionObject.SetActive(false);
                // init player
                myCurrentPlayer.Init();
                myCurrentPlayer = null;
                // set slow mode back
                Time.timeScale = 1f;
                return;
            }
        }
    }

    // 点击地块时
    public void OnClickTile(CS_Tile g_tile)
    {
        if (myCurrentPlayer != null)
        {
            myCurrentPlayer.transform.position = g_tile.transform.position; //地块的position，赋值给玩家单位position
            myCurrentPlayer.gameObject.SetActive(true); //设置单位为激活状态
            g_tile.Occupy(myCurrentPlayer); //设置地块为占用状态，占用的对象为myCurrentPlayer
        }
    }

    // 掉血function
    public void LoseLife()
    {
        myCurrentLife--; //生命值减-1
        CS_UIManager.Instance.SetLife(myCurrentLife); //修改生命值显示

        if (myCurrentLife <= 0)
        { //生命值小于1时
            CS_UIManager.Instance.ShowPageFail(); //游戏失败
            Time.timeScale = 0; //游戏暂停
        }
    }

    public List<CS_Player> GetPlayerList()
    {
        return myPlayerList;
    }
}
