using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [Space(5)]
    public GunGrabbable gunGrabbable;
    [SerializeField] ThirdPersonShooterController thirdPersonShooterController;
    [SerializeField] PickUpDrop pickUpDrop;
    [SerializeField] RagdollManager ragdollManager;

    [Header("IsDeath?")]
    [Space(5)]
    public bool isDeath;

    [Space(10)]
    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip playerDeathClip;

    private void Start()
    {
        ragdollManager = GetComponent<RagdollManager>();
        ragdollManager.SetRigidbodyState(true, isDeath);
        ragdollManager.SetCharacterControllerState(false);
        thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        pickUpDrop = GetComponent<PickUpDrop>();
    }

    public void PlayerDie()
    {
        isDeath = true;
        ragdollManager.SetRigidbodyState(false, isDeath);
        ragdollManager.SetCharacterControllerState(true);
        CheckForWeapons();
        if (thirdPersonShooterController.hasAGun)
        {
            gunGrabbable.Drop(Camera.main.transform.forward, pickUpDrop.playerThrowForce);
        }
        playerAudioSource.PlayOneShot(playerDeathClip);
        GetComponent<Animator>().enabled = false;
        GetComponent<ThirdPersonMovement>().enabled = false;
        GetComponent<ThirdPersonShooterController>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PickUpDrop>().enabled = false;
    }

    private void CheckForWeapons()
    {
        gunGrabbable = GetComponentInChildren<GunGrabbable>();

        if (gunGrabbable != null)
        {
            thirdPersonShooterController.hasAGun = true;
        }
        else
        {
            thirdPersonShooterController.hasAGun = false;
        }
    }

}
