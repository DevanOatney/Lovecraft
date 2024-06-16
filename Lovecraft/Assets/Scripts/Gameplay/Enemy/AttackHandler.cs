using System.Collections;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public float telegraphDuration = 1f;
    public float attackDuration = 0.5f;
    public float recoveryDuration = 1f;

    private EnemyAI enemyAI;
    private bool isAttacking = false;

    private Transform telegraphArea;
    private Transform attack;
    private EnemyAttackData enemyAttackData;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    public void StartAttack(Transform target, EnemyAttackData _attackData)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            enemyAttackData = _attackData;
            StartCoroutine(AttackRoutine(target, enemyAttackData.damageToDeal));
        }
    }

    public void AttackTree(Transform target, int damageToDeal)
    {

    }

    private IEnumerator AttackRoutine(Transform target, int damage)
    {
        //First- we should probably face the direction of the thing we're attacking (maybe?)
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        float rotSpeed = 5f;//idk how fast they should turn
        while (Quaternion.Angle(transform.rotation, lookDir) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotSpeed * Time.deltaTime);
            yield return null;
        }

        // Telegraph
        if (telegraphArea == null)
        {
            Vector3 heightAdjust = target.position + new Vector3(0f, transform.lossyScale.y * 0.5f , 0f);
            telegraphArea = Instantiate(enemyAttackData.Telegraph.transform, heightAdjust, this.transform.rotation);
            telegraphArea.name = "Telegraph";
        }
        yield return new WaitForSeconds(telegraphDuration);

        if (attack == null && telegraphArea != null)
        {
            attack = Instantiate(enemyAttackData.Telegraph.transform, telegraphArea.transform.position, telegraphArea.transform.rotation);
            attack.GetComponent<Collider>().enabled = true;
            attack.name = "Attack";
            attack.GetComponent<DamageZoneController>().Initialize(LayerMask.NameToLayer("PlayerLayer"), damage);
        }

        if (telegraphArea != null)
        {
            Destroy(telegraphArea.gameObject);
        }

        // Execute attack
        if (target != null)
        {
            if (target.GetComponent<TreeController>() != null)
            {
                target.GetComponent<TreeController>().TakeDamage(damage);
            }
            else if (target.GetComponent<PlayerController>() != null)
            {
                target.GetComponent<PlayerController>().TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(attackDuration);

        if (attack != null)
        {
            Destroy(attack.gameObject);
        }

        // Recover
        yield return new WaitForSeconds(recoveryDuration);

        isAttacking = false;
        enemyAI.OnAttackComplete();
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}