using DG.Tweening;
using StarterAssets;
using System.Collections;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;


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

    [Space(10)]
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] pistolShotsClips;
    public AudioClip empyPistolTriggerClip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Pickup(Transform holder)
    {
        currentHolder = holder;
        FindChildTransformWithName(currentHolder, "PistolPosition");
        isPickedUp =true;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        GetComponent<Collider>().enabled = false;

        if(transform.parent != pistolTransform)
        transform.parent = pistolTransform;

        transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOLocalRotateQuaternion(Quaternion.identity, .25f).SetUpdate(true);
    }

    public void Drop(Vector3 throwDirection, float throwForce)
    {
        Debug.Log("Gun is droped");
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
        audioSource.PlayOneShot(pistolShotsClips[Random.Range(0, pistolShotsClips.Length - 1)]);
        ammoCount--;
    }

    public void CheckAmmoCount()
    {
        if (ammoCount > 0)
            hasAmmo = true;
        else 
        {
            hasAmmo = false;
            audioSource.PlayOneShot(empyPistolTriggerClip);
        } 
    }

    void FindChildTransformWithName(Transform parentTransform, string targetName)
    {
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform childTransform = parentTransform.GetChild(i);

            if (childTransform.name == targetName)
            {
                pistolTransform = childTransform;
            }

            if (childTransform.childCount > 0)
            {
                FindChildTransformWithName(childTransform, targetName);
            }
        }
    }
}
