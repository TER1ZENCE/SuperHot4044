using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform playerCameraTransform;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask pickUpLayerMask;

    [Header("Pickup Distance")]
    [SerializeField] private float pickupDistance = 5f;
    public float playerThrowForce = 10f;

    [SerializeField] private GunGrabbable gunGrabbable;
    [SerializeField] private ThirdPersonShooterController thirdPersonShooterController;
    public Transform pistolPositionTransform;

    [Space(10)]
    [Header("Audio")]
    public AudioSource pickupAudioSource;
    public AudioClip pistolPickupClip;
    public AudioClip pistolDropClip;


    private void Start()
    {
       playerCameraTransform = Camera.main.transform;
       thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickUpLayerMask))
            {
                if (gunGrabbable == null && raycastHit.transform.TryGetComponent(out gunGrabbable))
                {
                    gunGrabbable.Pickup(this.transform);
                    pickupAudioSource.PlayOneShot(pistolPickupClip);
                    thirdPersonShooterController.hasAGun = true;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            if (gunGrabbable != null)
            {
                thirdPersonShooterController.SetNormalCamera();
                pickupAudioSource.PlayOneShot(pistolDropClip);
                gunGrabbable.Drop(Camera.main.transform.forward, playerThrowForce);
                thirdPersonShooterController.hasAGun = false;
                gunGrabbable = null;
            }
        }
    }
}