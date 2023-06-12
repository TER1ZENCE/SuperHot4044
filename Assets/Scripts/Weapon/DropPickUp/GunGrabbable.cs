using DG.Tweening;
using StarterAssets;
using System.Collections;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.WSA;


public class GunGrabbable : MonoBehaviour
{
    [Header("References")]
    [Space(5f)]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform bullet;
    public Transform pistolTransform;
    

    [Header("Pick Up - Drop")]
    [Space(5f)]
    public float pickupSpeed;
    public float pickupDistanceThreshold;
    public bool isPickedUp = false;
    public Transform currentHolder;


    [Header("Ammo")]
    [Space(5f)]
    public float ammoCount;
    public bool hasAmmo;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform holder)
    {
        currentHolder = holder;
        FindChildTransformWithName(currentHolder, "PistolPosition");
        isPickedUp =true;
        rb.isKinematic = true; 
        GetComponent<Collider>().enabled = false;
        transform.parent = pistolTransform;
        transform.DOMove(pistolTransform.position, .25f).SetEase(Ease.OutBack).SetUpdate(true);    
        transform.DORotateQuaternion(pistolTransform.rotation, .25f).SetUpdate(true);
        rb.interpolation = RigidbodyInterpolation.None;
    }

    public void Drop(Vector3 throwDirection, float throwForce)
    {
        isPickedUp = false;
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * 2, ForceMode.Impulse);
        GetComponent<Collider>().enabled = true;
        currentHolder = null;
    }

    public void Shoot(Vector3 direction)
    {
        Transform spawnBulletPosition = gameObject.transform.Find("BulletSpawnPosition");
        Vector3 aimDir = (direction - spawnBulletPosition.position).normalized;

        Instantiate(bullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

        ammoCount--;
    }

    public void CheckAmmoCount()
    {
            if (ammoCount > 0)
                hasAmmo = true;
            else hasAmmo = false;
    }

    void FindChildTransformWithName(Transform parentTransform, string targetName)
    {
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform childTransform = parentTransform.GetChild(i);

            if (childTransform.name == targetName)
            {
                Debug.Log("Transform : " + childTransform.name);
                pistolTransform = childTransform;
            }

            if (childTransform.childCount > 0)
            {
                FindChildTransformWithName(childTransform, targetName);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPickedUp && currentHolder == pistolTransform)
        {
            if (Vector3.Distance(transform.position, pistolTransform.position) > pickupDistanceThreshold)
            {
                //transform.position = Vector3.Lerp(transform.position, playerPistolTransform.position, Time.deltaTime * pickupSpeed);
                //transform.rotation = Quaternion.Lerp(transform.rotation, playerPistolTransform.rotation, Time.deltaTime * pickupSpeed);
            }
            else
            {
                if (transform.position != pistolTransform.position && Vector3.Distance(transform.position, pistolTransform.position) < pickupDistanceThreshold)
                {
                    transform.position = pistolTransform.position;
                    transform.rotation = pistolTransform.rotation;
                }
            }
        }
    }
}
