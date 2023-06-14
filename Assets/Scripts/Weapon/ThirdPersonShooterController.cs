using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Aim")]
    [Space(5)]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float aimRotateSpeed = 20f;

    [Header("Sensitivity")]
    [Space(5)]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [Header("LayerMask")]
    [Space(5)]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    [Header("Bullet")]
    [Space(5)]
    [SerializeField] private Transform pfBulletProjectile;

    [Header("AnimatorLayerCheck")]
    [Space(5)]
    [SerializeField] private int baseMovementLayerIndex;
    [SerializeField] private int aimingLayerIndex;

    //References
    private ThirdPersonMovement thirdPersonMovement;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private GunGrabbable gunGrabbable;

    [Header("CanShoot?")]
    [Space(5)]
    [SerializeField] public bool hasAGun;
    [SerializeField] public bool isInAimState;
    [SerializeField] public bool hasAmmo;
    [SerializeField] private bool canShoot;

    [Header("ShootDelay")]
    [Space(5)]
    [SerializeField] private float shootDelay = 0.5f;
    private float lastShootTime;


    private void Awake()
    {
        thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        canShoot = false;
        baseMovementLayerIndex = animator.GetLayerIndex("Base Movement");
        aimingLayerIndex = animator.GetLayerIndex("Aiming");
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }


        //Set Aim State
        if (starterAssetsInputs.aim)
        {
            PickUpDrop pickUpDrop = GetComponent<PickUpDrop>();

            if (hasAGun)
            {
                SetAimCamera();

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimRotateSpeed);
            }
        }
        else
        {
            SetNormalCamera();
            isInAimState = false;
        }

        //Shoot
        if (starterAssetsInputs.shoot && starterAssetsInputs.aim)
        {
                CheckShootState();

                if (canShoot)
                {
                  gunGrabbable.Shoot(mouseWorldPosition);
                 StartCoroutine(thirdPersonMovement.ActionE());

                  lastShootTime = Time.time;
                  starterAssetsInputs.shoot = false;
            }   
        }
        else
        {
            StopCoroutine(thirdPersonMovement.ActionE());
            starterAssetsInputs.shoot = false;
        }
    }

    //Camera States
    public void SetNormalCamera()
    {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonMovement.SetSensitivity(normalSensitivity);
        thirdPersonMovement.SetRotateOnMove(true);
        animator.SetLayerWeight(aimingLayerIndex, Mathf.Lerp(animator.GetLayerWeight(aimingLayerIndex), 0f, Time.deltaTime * 10f));
        animator.SetLayerWeight(baseMovementLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseMovementLayerIndex), 1f, Time.deltaTime * 200f));
    }

    public void SetAimCamera()
    {
        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonMovement.SetSensitivity(aimSensitivity);
        thirdPersonMovement.SetRotateOnMove(false);
        animator.SetLayerWeight(aimingLayerIndex, Mathf.Lerp(animator.GetLayerWeight(aimingLayerIndex), 1f, Time.deltaTime * 10f));
        animator.SetLayerWeight(baseMovementLayerIndex, Mathf.Lerp(animator.GetLayerWeight(baseMovementLayerIndex), 0f, Time.deltaTime * 1f));
    }

    private void CheckShootState()
    {
        gunGrabbable = GetComponentInChildren<GunGrabbable>();

        if(gunGrabbable  != null )
        gunGrabbable.CheckAmmoCount();

        hasAmmo = gunGrabbable.hasAmmo;

        if (hasAGun && hasAmmo && isInAimState && Time.time - lastShootTime >= shootDelay)
            EnableShooting();
        else
            DisableShooting();
    }

    private void SetAimState()
    {
        if (!isInAimState && starterAssetsInputs.aim)
        {
            isInAimState = true;
        }
    }

    //private void CheckAmmoCount()
    //{
    //    gunGrabbable = GetComponentInChildren<GunGrabbable>();
    //    if (gunGrabbable != null)
    //    {
    //        if (gunGrabbable.ammoCount > 0)
    //            hasAmmo = true;
    //        else hasAmmo = false;
    //    }
    //}


    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }

    public void EnableShooting()
    {
            CanShoot = true;
    }

    public void DisableShooting()
    {
        CanShoot = false;
        starterAssetsInputs.shoot = false;
    }
}

