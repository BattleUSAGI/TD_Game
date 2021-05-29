using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Attack = 1,
        Dead = 9,
        Arrange = 10,
    }

    private State myState = State.Arrange; //设定初始状态
    [SerializeField] Transform myRangeParent = null; //攻击范围父节点
    [SerializeField] Transform myRotateTransform = null; //旋转父节点
    private CS_Enemy myTargetEnemy; //敌人实例

    [SerializeField] protected Animator myAnimator = null; //角色动画Animator

    [SerializeField] Transform myTransform_HPBar = null; //血条transform

    [SerializeField] protected GameObject myEffectPrefab = null; //角色攻击投射物prefab
    protected CS_Effect myEffect = null;  //控制音效等

    [SerializeField] protected AudioSource myAudioSource_Attack; //音效实例

    [Header("Status")]
    [SerializeField] CS_Tile.Type myTileType = CS_Tile.Type.Ground; //设置tile实例，初始为ground
    [SerializeField] int myStatus_MaxHealth = 2400; //最大HP
    private int myCurrentHealth; //当前HP
    [SerializeField] protected int myStatus_Attack = 700; // 攻击力
    [SerializeField] protected float myStatus_AttackTime = 0.5f; //攻击间隔
    protected float myAttackTimer = 0; //攻击计时器，归0时发动攻击

    public void Arrange()
    { //方法内执行枚举状态切换
        myState = State.Arrange;
    }

    public void Init()
    { //部署预览（角色已在地块上的情况）
        myState = State.Idle;
        // hide highlight
        HideHighlight(); //隐藏攻击范围显示
        // face camera
        FaceCamera(); //当前角色的朝向设定
        // init health
        myCurrentHealth = myStatus_MaxHealth; //将角色生命值归位最大值
        myTransform_HPBar.localScale = Vector3.one; //设置血条大小为1
        // init effect
        if (myEffect == null)
        {
            myEffect = Instantiate(myEffectPrefab).GetComponent<CS_Effect>(); //将myEffectPrefab上的CS_Effect赋值给myEffect
            myEffect.Kill(); //不在场景中生成prefab
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(myState);
        if (myState == State.Arrange || myState == State.Dead)
        { //若当前state为待部署状态or死亡状态，则不执行后续逻辑
            return;
        }
        Update_Attack();
    }

    private void Update()
    {
        if (myState == State.Arrange)
        {
            FaceCamera();
        }
    }

    private void FaceCamera()
    {
        // face camera
        myRotateTransform.rotation = Quaternion.identity; //将单位的旋转归位
    }

    public void ShowHighlight()
    { //遍历数组，并将它们打开
        for (int i = 0; i < myRangeParent.childCount; i++)
        {
            myRangeParent.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void HideHighlight()
    { //遍历数组，并将它们关掉
        for (int i = 0; i < myRangeParent.childCount; i++)
        {
            myRangeParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected virtual void Update_Attack()
    { //更新攻击逻辑
        // update attack timer
        if (myAttackTimer > 0)
        {  //更新攻击计时器
            myAttackTimer -= Time.fixedDeltaTime;
            return;
        }

        // if enemy is gone, remove target
        if (myTargetEnemy != null && myTargetEnemy.gameObject.activeSelf == false)
        {  // 若当前存在指定的敌人，但敌人为非active状态，则默认设为没有找到敌人（确保初始化时没有敌人作为目标）
            myTargetEnemy = null;
        }

        // if i dont have a target, go through enemy list to find a target
        if (myTargetEnemy == null)
        {  // 若当前没有target，则去找target
            List<CS_Enemy> t_enemyList = CS_EnemyManager.Instance.GetEnemyList(); //拿到当前敌人的list
            foreach (CS_Enemy f_enemy in t_enemyList)
            { //遍历敌人list
                if (CheckInRange(f_enemy.transform) == true)
                { //若敌人在攻击范围内，则该敌人为攻击目标
                    myTargetEnemy = f_enemy;
                    break;
                }
            }
        }

        // if no enemy in range, dont attack
        if (myTargetEnemy == null)
        {
            return;
        }

        // if the enemy move out of the range, stop attacking this enemy
        if (CheckInRange(myTargetEnemy.transform) == false)
        {
            myTargetEnemy = null;
            Debug.Log("out of range");
            return;
        }

        // play sfx
        myAudioSource_Attack.Play();

        // play effect
        myEffect.Kill();//播放特效前，先确保当前没有特效正在播放（执行kill），然后确保特效播放的位置是在敌人的位置，再进行播放
        myEffect.transform.position = myTargetEnemy.transform.position;
        myEffect.gameObject.SetActive(true);

        // attack enemy
        myTargetEnemy.TakeDamage(myStatus_Attack); //敌人执行受击方法
        myAttackTimer += myStatus_AttackTime; //攻击后，计时器时间增加CD
        myAnimator.SetTrigger("Attack"); //播放攻击动画
    }

    protected bool CheckInRange(Transform g_transform)
    { //索敌方法
        Vector3 t_position = g_transform.position; //从其他方法输入的字段值，拿到敌人的position

        for (int i = 0; i < myRangeParent.childCount; i++)
        {
            Vector3 t_rangeCenter = myRangeParent.GetChild(i).position;
            if (t_position.x > t_rangeCenter.x - 0.5f && t_position.x < t_rangeCenter.x + 0.5f &&
                t_position.y > t_rangeCenter.y - 0.5f && t_position.y < t_rangeCenter.y + 0.5f)
            {
                return true;
            }
        }

        return false;
    }

    public void TakeDamage(int g_damage)
    {
        myCurrentHealth -= g_damage;

        if (myCurrentHealth <= 0)
        {
            myCurrentHealth = 0; //设置血量为0，确保显示和逻辑不会出问题
            // set dead
            myState = State.Dead;
            // hide enemy
            this.gameObject.SetActive(false); //关掉prefab
        }

        if (myCurrentHealth > myStatus_MaxHealth)
        { //确保血量不会超过最大值（被奶过头）
            myCurrentHealth = myStatus_MaxHealth;
        }

        // update HP bar ui
        myTransform_HPBar.localScale = new Vector3(GetHealthPercent(), 1, 1); //更新血条
    }

    public float GetHealthPercent()
    {
        return (float)myCurrentHealth / myStatus_MaxHealth;
    }

    public CS_Tile.Type GetTileType()
    {
        return myTileType;
    }

    public State GetState()
    {
        return myState;
    }
}
