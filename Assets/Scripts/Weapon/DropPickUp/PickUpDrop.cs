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
                    thirdPersonShooterController.hasAGun = true;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            if (gunGrabbable != null)
            {
                thirdPersonShooterController.SetNormalCamera();
                gunGrabbable.Drop(Camera.main.transform.forward, playerThrowForce);
                thirdPersonShooterController.hasAGun = false;
                gunGrabbable = null;
            }
        }
    }
}