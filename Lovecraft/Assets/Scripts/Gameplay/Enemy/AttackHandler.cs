using System.Collections;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public float telegraphDuration = 1f;
    public float attackDuration = 0.5f;
    public float recoveryDuration = 1f;

    private EnemyAI enemyAI;
    private bool isAttacking = false;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    public void StartAttack(Transform target, int damage)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(AttackRoutine(target, damage));
        }
    }

    private IEnumerator AttackRoutine(Transform target, int damage)
    {
        // Telegraph
        yield return new WaitForSeconds(telegraphDuration);

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