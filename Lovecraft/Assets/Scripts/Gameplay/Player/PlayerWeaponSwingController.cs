using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponSwingController : MonoBehaviour
{
    [SerializeField] private InputAction playerWeaponAttack;
    [SerializeField] private Animator weaponAnimationController;
    [SerializeField] private TrailRenderer slashTrailBase;
    [SerializeField] private Transform weaponVisual;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {

        playerWeaponAttack.performed += PerformWeaponAttack;
        playerWeaponAttack.Enable();
    }
    private void OnDisable()
    {
        playerWeaponAttack.Disable();
    }


    private void PerformWeaponAttack(InputAction.CallbackContext context)
    {
        weaponAnimationController.SetTrigger("WeaponSlash");
        Instantiate(slashTrailBase, weaponVisual).gameObject.SetActive(true);
    }

}
