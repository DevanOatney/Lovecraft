using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerWeaponSwingDamageController : MonoBehaviour
{
    [SerializeField] GameObject weaponSwingDamageZone;
    [SerializeField] AudioSource audioSource;
    [SerializeField] VisualEffect slashVfx;
    public List<AudioClip> swingSFXList;

    public void CreateDamageZone(AnimationEvent ae)
    {
        GameObject obj = Instantiate(weaponSwingDamageZone, weaponSwingDamageZone.transform.position, weaponSwingDamageZone.transform.rotation);
        obj.SetActive(true);
        obj.GetComponent<DamageZoneController>().damageToOpponent = AbilityUpgradesManager.Instance.AbilityUpgrades[Abilities.ATTACK_DAMAGE].GetModifiedValue(1);
        audioSource.PlayOneShot(swingSFXList[Random.Range(0, swingSFXList.Count)]);
        slashVfx.Play();
    }
}
