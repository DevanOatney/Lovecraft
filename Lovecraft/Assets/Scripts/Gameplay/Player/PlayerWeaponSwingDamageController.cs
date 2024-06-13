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
        Instantiate(weaponSwingDamageZone, weaponSwingDamageZone.transform.position, weaponSwingDamageZone.transform.rotation).SetActive(true);
        audioSource.PlayOneShot(swingSFXList[Random.Range(0, swingSFXList.Count)]);
    }
}
