using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerWeaponSwingController : MonoBehaviour
{
    [SerializeField] private InputAction playerWeaponAttack;
    [SerializeField] private Animator weaponAnimationController;
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

        foreach(CanvasHitDetector canvas in GameObject.FindObjectsByType<CanvasHitDetector>(FindObjectsSortMode.None))
        {
            if( canvas.IsPointerOverUI() ) { return; }
        }

        weaponAnimationController.SetTrigger("WeaponSlash");


    }

}
