using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KdTree;
using KdTree.Math;
using UnityEngine.AI;
using UnityEngine.UI;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler instance = null;

    public Dictionary<int, List<AttackableObject>> units = new Dictionary<int, List<AttackableObject>>();
    public List<int> unitCodes = new List<int>();

    public Text unitCounter = null;

    public Text deltaSearchTime = null;
    public Text deltaAttackTime = null;
    public Text deltaApproachTime = null;

    private KdTree<float, ResourceNode> resources = new KdTree<float, ResourceNode>(2, new FloatMath());

    private void Awake()
    {
        instance = this;

        StartCoroutine(HandleSearch());
        StartCoroutine(ApproachTargets());
        StartCoroutine(HandleAttacking());
    }

    public void AddUnit(AttackableObject a)
    {
        if (!unitCodes.Contains(a.TeamCode))
        {
            unitCodes.Add(a.TeamCode);
            units[a.TeamCode] = new List<AttackableObject>();
            units[a.TeamCode].Add(a);
        }
        else
        {
            units[a.TeamCode].Add(a);
        }
    }

    public void AddResourceNode(ResourceNode r)
    {
        resources.Add(new float[] { r.transform.position.x, r.transform.position.z}, r);
    }

    public void RemoveUnit(AttackableObject a)
    {
        units[a.TeamCode].Remove(a);
    }

    public void RemoveResourceNode(ResourceNode r)
    {
        resources.RemoveAt(new float[] { transform.transform.position.x, r.transform.position.z});
    }

    IEnumerator HandleSearch()
    {
        float timeBeg = 0;
        float timeInter = 0;

        int hitUnits = 0;

        timeInter = Time.realtimeSinceStartup;
        while (true)
        {
            hitUnits = 0;
            timeBeg = Time.realtimeSinceStartup;
            Dictionary<int, KdTree<float, AttackableObject>> unitTrees = new Dictionary<int, KdTree<float, AttackableObject>>();
            for (int i = 0; i < unitCodes.Count; i++)
            {
                unitTrees[i] = new KdTree<float, AttackableObject>(2, new FloatMath());
                for (int j = 0; j < units[unitCodes[i]].Count; j++)
                {
                    AttackableObject u = units[unitCodes[i]][j];
                    unitTrees[i].Add(u.GetFloatArray(), u);
                    if (u.startSearch)
                    {
                        if (!u.onTargetSearch) u.onTargetSearch = true;
                        u.startSearch = false;
                    }

                    if (Time.realtimeSinceStartup - timeInter > 0.005f)
                    {
                        //yield return new WaitForSeconds(0.1f * (Time.realtimeSinceStartup - timeInter) + 0.05f);
                        yield return null;
                        timeInter = Time.realtimeSinceStartup;
                    }
                }
            }

            for (int i = 0; i < unitCodes.Count; i++)
            {
                for (int j = 0; j < units[unitCodes[i]].Count; j++)
                {
                    AttackableObject searcher = units[unitCodes[i]][j];
                    if (!searcher.onTargetSearch) continue;
                    AttackableObject closest = null;
                    for (int k = 0; k < unitCodes.Count; k++)
                    {
                        if (k == i) continue;

                        if (unitTrees[unitCodes[k]].Count == 0)
                        {
                            continue;
                        }

                        AttackableObject temp = unitTrees[unitCodes[k]].GetNearestNeighbours(searcher.GetFloatArray(), 1)[0].Value;
                        if (temp != null && (closest == null || Vector3.Distance(searcher.transform.position, closest.transform.position) > Vector3.Distance(searcher.transform.position, temp.transform.position)))
                        {
                            if (Vector3.Distance(searcher.transform.position, temp.transform.position) <= searcher.searchRange) closest = temp;
                            /*searcher.target = closest;
                            Debug.Log("Set target");
                            searcher.startApproach = true;
                            Debug.Log("Unit: isMovable: " + searcher.isMovable + " isApproaching: " + searcher.isApproaching);*/
                            //closest = temp;
                        }
                    }
                    if (closest != null)
                    {
                        if (searcher.isMovable)
                        {
                            searcher.target = closest;
                            searcher.onTargetSearch = false;
                            searcher.startApproach = true;
                        }
                        else if(Vector3.Distance(searcher.transform.position, closest.transform.position) <= searcher.idealRange)
                        {
                            searcher.target = closest;
                            searcher.onTargetSearch = false;
                            searcher.startAttack = true;
                        }
                    }

                    if (Time.realtimeSinceStartup - timeInter > 0.005f)
                    {
                        //yield return new WaitForSeconds(0.1f * (Time.realtimeSinceStartup - timeInter) + 0.05f);
                        yield return null;
                        timeInter = Time.realtimeSinceStartup;
                    }
                }
            }
            deltaSearchTime.text = "DST: " + ((Time.realtimeSinceStartup - timeBeg)) + " TU: " + hitUnits;
            yield return new WaitForSeconds(0.5f + 1.0f * (Time.realtimeSinceStartup - timeBeg));
        }
    }

    IEnumerator ApproachTargets()
    {
        float timeBeg = 0;
        float timeEnd = 0;
        float time3 = 0;

        float dist = 0;
        float stoppingDist = 0;

        float ax, ay, az;
        float tx, ty, tz;

        while (true)
        {
            if (unitCodes.Count == 0)
            {
                yield return null;
                continue;
            }

            timeBeg = Time.realtimeSinceStartup;
            for (int i = 0; i < unitCodes.Count; i++)
            {
                for (int j = 0; j < units[unitCodes[i]].Count; j++)
                {
                    AttackableObject appr = units[unitCodes[i]][j];

                    if (appr.target == null)
                    {
                        appr.startApproach = false;
                        appr.isApproaching = false;
                        appr.startSearch = false;
                    }

                    if (appr.startApproach && appr.isMovable)
                    {
//                        Debug.Log("Starting approach: " + Vector3.Distance(appr.transform.position, appr.target.transform.position));
                        appr.startApproach = false;
                        appr.isApproaching = true;
                        appr.navAgent.SetDestination(appr.target.transform.position);
                        appr.navAgent.speed = appr.moveSpeed;
                    }
                    else if (appr.isApproaching)
                    {
                        AttackableObject targ = appr.target;

                        if (targ == null)
                        {
                            appr.isApproaching = false;
                            appr.startSearch = true;
                            continue;
                        }

                        ax = appr.transform.position.x;
                        ay = appr.transform.position.y;
                        az = appr.transform.position.z;

                        tx = targ.transform.position.x;
                        ty = targ.transform.position.y;
                        tz = targ.transform.position.z;

                        NavMeshAgent apprNav = appr.navAgent;
                        NavMeshAgent targNav = targ.navAgent;

                        //Calculate stopping condition for NavMeshAgent
                        //apprNav.stoppingDistance = apprNav.radius / appr.transform.localScale.x + targNav.radius / targ.transform.localScale.x;
                        apprNav.stoppingDistance = appr.idealRange;

                        //Distance between approacher and target
                        dist = Mathf.Sqrt((tx-ax)*(tx-ax)+(ty-ay)*(ty-ay)+(tz-az)*(tz-az));

                        //stoppingDist = appr.idealRange + appr.transform.localScale.x * targ.transform.localScale.x*apprNav.stoppingDistance;
                        stoppingDist = appr.idealRange;

                        // counting increased distances (failure to approach) between attacker and target;
                        // if counter failedR becomes bigger than critFailedR, preparing for new target search.
                        if (appr.prevDist < dist)
                        {
                            appr.apprFailed += 1;
                            if (appr.apprFailed > appr.critApprFailed)
                            {
                                appr.isApproaching = false;
                                appr.startSearch = true;
                                appr.apprFailed = 0;
                            }
                        }
                        else
                        {
                            if (dist < stoppingDist)
                            {
//                                Debug.Log("Reached target");
                                apprNav.SetDestination(appr.transform.position);
                                //Debug.Log("Starting to attack");
                                //Set unit up for attacking
                                appr.isApproaching = false;
                                appr.isAttacking = true;
                            }
                            else
                            {
                                //Starting to move
                                if (appr.isMovable)
                                {
                                    apprNav.SetDestination(targ.transform.position);
                                    apprNav.speed = appr.moveSpeed;
                                }
                            }
                        }
                        appr.prevDist = dist;
                    }
                    if (Time.realtimeSinceStartup - time3 > 0.005f)
                    {
                        //twaiter = twaiter + 0.1f * (Time.realtimeSinceStartup - t3) + 0.05f;
                        yield return new WaitForSeconds(0.1f * (Time.realtimeSinceStartup - time3) + 0.05f);
                        time3 = Time.realtimeSinceStartup;
                    }
                }
            }
            yield return new WaitForSeconds(1.0f * (timeEnd - timeBeg) + 1.0f);
        }
    }

    private IEnumerator HandleAttacking()
    {
        float timeBeg = 0f;
        //Intermediate time used when in loop
        float inter = Time.realtimeSinceStartup;

        float ax, ay, az;
        float tx, ty, tz;

        while (true)
        {
            timeBeg = Time.realtimeSinceStartup;
            for (int i = 0; i < unitCodes.Count; i++)
            {
                for (int j = 0; j < units[unitCodes[i]].Count; j++)
                {
                    AttackableObject attacker = units[unitCodes[i]][j];
                    if (attacker.isAttacking && attacker.target != null)
                    {
                        AttackableObject target = attacker.target;

                        ax = attacker.transform.position.x;
                        ay = attacker.transform.position.y;
                        az = attacker.transform.position.z;

                        tx = target.transform.position.x;
                        ty = target.transform.position.y;
                        tz = target.transform.position.z;

                        if (Vector3.Distance(attacker.transform.position, target.transform.position) > attacker.idealRange)
                        {
                            attacker.isApproaching = true;
                            attacker.isAttacking = false;
                        }
                        else
                        {
//                            Debug.Log("Attacking");
                            target.TakeDamage(attacker.attackDamage);
                        }
                    }
                    else
                    {
                        attacker.isAttacking = false;
                        attacker.startSearch = true;
                    }

                    if (Time.realtimeSinceStartup - inter > 0.005f)
                    {
                        //twaiter = twaiter + 0.1f * (Time.realtimeSinceStartup - t3) + 0.05f;
                        yield return new WaitForSeconds(0.1f * (Time.realtimeSinceStartup - inter) + 0.05f);
                        inter = Time.realtimeSinceStartup;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f + 1.0f * (Time.realtimeSinceStartup - timeBeg));
        }
    }

    private void OnGUI()
    {
        int count = 0;
        for (int i = 0; i < unitCodes.Count; i++)
        {
            count += units[unitCodes[i]].Count;
        }
        unitCounter.text = count.ToString();
    }
}

public abstract class AttackableObject : MonoBehaviour
{
    public int TeamCode = 0;
    [SerializeField] public HealthBar healthBar = null;

    public bool isMovable = false;
    public bool canAttack = false;
    public bool isSelected = false;

    public bool startSearch = false;
    public bool startApproach = false;
    public bool startAttack = false;

    //public bool isReady = true;
    public bool isApproaching = false;
    public bool isAttacking = false;
    public bool onTargetSearch = false;
    
    public NavMeshAgent navAgent = null;
    public float moveSpeed = 5f;

    public AttackableObject target = null;

    public float searchRange = 10f;
    public float idealRange = 2f;
    public float prevDist = 0;
    public int apprFailed = 0;
    public int critApprFailed = 10;

    public float attackDamage = 10f;
    public float lastAttack = 0;
    public float timeBetweenAttacks = 0.1f;

    public float maxHealth = 100;
    public float currentHealth = 100;

    public float[] GetFloatArray()
    {
        return new float[] { transform.position.x, transform.position.z};
    }

    public virtual void Populate(int teamCode)
    {
        TeamCode = teamCode;
        CombatHandler.instance.AddUnit(this);
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(transform.position);
        startSearch = true;

        gameObject.GetComponent<Renderer>().material.color = GameManager.instance.players[TeamCode].playerColor;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
//            Debug.Log("Killed unit");
            CombatHandler.instance.RemoveUnit(this);
            Destroy(gameObject);
        }
    }

    public virtual void AttackTarget()
    {
        if(Time.realtimeSinceStartup >= lastAttack + timeBetweenAttacks)
        {
            target.TakeDamage(attackDamage);
            lastAttack = Time.realtimeSinceStartup;
        }
    }

    public virtual void SetSelected(bool selected)
    {
        healthBar.gameObject.SetActive(selected);
    }
}

public class ChargedAttackBase : AttackableObject
{
    /*[SerializeField] private float timeBetweenAttacks = 1f;
    public float lastAttack = 0f;*/

    public override void AttackTarget()
    {
        if (Time.realtimeSinceStartup >= lastAttack + timeBetweenAttacks)
        {
            target.TakeDamage(attackDamage);
            lastAttack = Time.realtimeSinceStartup;
        }
    }
}