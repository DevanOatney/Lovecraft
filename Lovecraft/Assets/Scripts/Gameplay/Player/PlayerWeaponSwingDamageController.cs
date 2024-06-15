using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSwingDamageController : MonoBehaviour
{
    [SerializeField] GameObject weaponSwingDamageZone;
    [SerializeField] AudioSource audioSource;
    public List<AudioClip> swingSFXList;

    public void CreateDamageZone(AnimationEvent ae)
    {
        GameObject obj = Instantiate(weaponSwingDamageZone, weaponSwingDamageZone.transform.position, weaponSwingDamageZone.transform.rotation);
        obj.SetActive(true);
        obj.GetComponent<DamageZoneController>().damageToOpponant = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.ATTACK_DAMAGE].GetModifiedValue(1);
        audioSource.PlayOneShot(swingSFXList[Random.Range(0, swingSFXList.Count)]);
    }
}
