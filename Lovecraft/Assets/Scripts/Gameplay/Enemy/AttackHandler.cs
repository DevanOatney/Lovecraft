using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

public class AttackHandler : MonoBehaviour
{
    public float telegraphDuration = 1f;
    public float attackDuration = 0.5f;
    public float recoveryDuration = 1f;
    public float attackRange = 1f; // Ensure this is set correctly

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
            if (_attackData.isChargeAttack)
            {
                StartCoroutine(ChargeAttackRoutine(target, _attackData.damageToDeal));
            }
            else if (_attackData.isProjectileAttack)
            {
                StartCoroutine(ProjectileAttackRoutine(target, _attackData.damageToDeal));
            }
            else
            {
                StartCoroutine(AttackRoutine(target, _attackData.damageToDeal));
            }
        }
    }

    public void AttackTree(Transform target, int damageToDeal)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(AttackTreeRoutine(target, damageToDeal));
        }
    }

    private IEnumerator AttackRoutine(Transform target, int damage)
    {
        // First, face the direction of the target
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        transform.rotation = lookDir;

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

        // Telegraph
        if (telegraphArea == null)
        {
            Vector3 heightAdjust = target.position + new Vector3(0f, transform.lossyScale.y * 0.5f, 0f);
            telegraphArea = Instantiate(enemyAttackData.Telegraph.transform, heightAdjust, this.transform.rotation);
            telegraphArea.GetComponent<Collider>().enabled = false;
            telegraphArea.name = "Telegraph";
        }
        yield return new WaitForSeconds(telegraphDuration);

        if (target == null)
        {
            CancelAttack();
            yield break;
        }

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

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
            else if (target.GetComponent<PlayerSixWayDirectionalController>() != null)
            {
                target.GetComponent<PlayerSixWayDirectionalController>().TakeDamage(damage);
            }
            else if (target.GetComponent<EnemyAI>() != null)
            {
                target.GetComponent<EnemyAI>().TakeDamage(damage);
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

    private IEnumerator ChargeAttackRoutine(Transform target, int damage)
    {
        // First, face the direction of the target
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        transform.rotation = lookDir;

        if (target == null)
        {
            CancelAttack();
            yield break;
        }

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

        // Telegraph
        if (telegraphArea == null)
        {
            Vector3 heightAdjust = target.position + new Vector3(0f, transform.lossyScale.y * 0.5f, 0f);
            telegraphArea = Instantiate(enemyAttackData.Telegraph.transform, heightAdjust, this.transform.rotation);
            telegraphArea.GetComponent<Collider>().enabled = false;
            telegraphArea.name = "Telegraph";
        }
        yield return new WaitForSeconds(telegraphDuration);

        if (target == null)
        {
            CancelAttack();
            yield break;
        }

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

        // Execute charge attack
        float chargeSpeed = 10f; // Adjust this as needed
        float chargeDuration = 1f; // Adjust this as needed
        float startTime = Time.time;
        while (Time.time < startTime + chargeDuration)
        {
            transform.position += transform.forward * chargeSpeed * Time.deltaTime;
            yield return null;
        }

        if (telegraphArea != null)
        {
            Destroy(telegraphArea.gameObject);
        }

        // Check for collision with the player
        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            if (target.GetComponent<PlayerSixWayDirectionalController>() != null)
            {
                target.GetComponent<PlayerSixWayDirectionalController>().TakeDamage(damage);
            }
            else if (target.GetComponent<EnemyAI>() != null)
            {
                target.GetComponent<EnemyAI>().TakeDamage(damage);
            }
        }

        // Recover
        yield return new WaitForSeconds(recoveryDuration);

        isAttacking = false;
        enemyAI.OnAttackComplete();
    }

    private IEnumerator ProjectileAttackRoutine(Transform target, int damage)
    {
        // First, face the direction of the target
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        transform.rotation = lookDir;

        if (target == null)
        {
            CancelAttack();
            yield break;
        }

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

        // Telegraph
        if (telegraphArea == null)
        {
            Vector3 heightAdjust = target.position + new Vector3(0f, transform.lossyScale.y * 0.5f, 0f);
            telegraphArea = Instantiate(enemyAttackData.Telegraph.transform, heightAdjust, this.transform.rotation);
            telegraphArea.GetComponent<Collider>().enabled = false;
            telegraphArea.name = "Telegraph";
        }
        yield return new WaitForSeconds(telegraphDuration);

        if (target == null)
        {
            CancelAttack();
            yield break;
        }

        if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            CancelAttack();
            yield break;
        }

        // Execute projectile attack
        if (enemyAttackData.ProjectilePrefab != null)
        {
            GameObject projectileInstance = Instantiate(enemyAttackData.ProjectilePrefab, transform.position + transform.forward, transform.rotation);
            EnemyProjectileController projectile = projectileInstance.GetComponent<EnemyProjectileController>();
            projectile.DamageToDeal = damage;
            projectile.Speed = 10f; // Set the speed for the projectile
            projectile.FilteredTag = enemyAI.TargetTag;
            projectile.Initialize(GetComponent<Collider>(), target.position);
        }

        if (telegraphArea != null)
        {
            Destroy(telegraphArea.gameObject);
        }

        // Recover
        yield return new WaitForSeconds(recoveryDuration);

        isAttacking = false;
        enemyAI.OnAttackComplete();
    }

    private IEnumerator AttackTreeRoutine(Transform target, int damage)
    {
        // First, face the direction of the thing we're attacking (maybe?)
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        float rotSpeed = 5f; // Adjust as needed
        while (Quaternion.Angle(transform.rotation, lookDir) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(telegraphDuration);

        // Execute attack
        if (target != null)
        {
            if (target.GetComponent<TreeController>() != null)
            {
                target.GetComponent<TreeController>().TakeDamage(damage);
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

    public void CancelAttack()
    {
        if (telegraphArea != null)
        {
            Destroy(telegraphArea.gameObject);
        }
        if (attack != null)
        {
            Destroy(attack.gameObject);
        }
        isAttacking = false;
        enemyAI.OnAttackComplete();
    }
}
