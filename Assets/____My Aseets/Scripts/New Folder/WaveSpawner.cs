using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab; //生成的敌人prefab
    public Transform SpawnPoint_1; //关卡敌人出生点

    public Text levelEnemyNum; //关卡敌人数量
    public Text levelBaseHP; //关卡据点血量



    public float countDown = 2f;


    private int waveIndex = 0;
    private float timeBetweenWaves = 5f;



    //void Start()
    //{
    //    Instantiate(wayPointPrefab);
    //}



    void Update()
    {
        if (countDown <= 0f) //倒计时小于0时
        {
            StartCoroutine(SpawnWave());
            countDown = timeBetweenWaves;

        }


        countDown -= Time.deltaTime;

        levelEnemyNum.text = Mathf.Round(countDown).ToString(); //纯测试用，后续可注可删

    }



    IEnumerator SpawnWave() //控制波数
    {
        waveIndex++;

        for (int i = 0; i < waveIndex; i++) //当i小于波数时，i++，并且生成Enemy
        {
            SpawnEnemy();
            Debug.Log("spawn enemy");
            yield return new WaitForSeconds(0.5f);
        }

    }



    void SpawnEnemy() //控制敌人生成
    {
        Instantiate(enemyPrefab, SpawnPoint_1.position, SpawnPoint_1.rotation);
    }





}
