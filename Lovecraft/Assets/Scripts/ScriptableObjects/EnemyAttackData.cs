using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Custom/Enemy/AttackData")]
public class EnemyAttackData : ScriptableObject
{
    public float telegraphDuration = 0f;
    public float attackDuration =  0f;
    public float recoveryDuration =  0f;
    public int damageToDeal = 1;
    public bool isChargeAttack = false;
    public bool isProjectileAttack = false;
    public GameObject Telegraph;
    public GameObject ProjectilePrefab;
}
