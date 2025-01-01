using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [SerializeField] Transform hand;
    [SerializeField] Vector3 offset;
    public GameObject player;
    public GameObject bullet;
    public GameObject[] weapons;
    private bool canFire = true;
    private int currentWeaponIndex = 0;
     
     public enum GunType
    {
        Pistol,
        Revolver,
        Rifle
    }
    public GunType currentGunType;

    void Awake()
    {
        // Set the gun's parent to the hand
        transform.SetParent(hand);
        
       transform.localRotation = Quaternion.Euler(278.477814f,348.370789f,102.123825f);
       EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        transform.position = hand.position + offset;
        HandleShooting();
        HandleWeaponSwitching();
    }

    void HandleShooting() {
        if (Input.GetMouseButtonDown(0) && canFire) { //canFire boolean controls the firing rate so 1 bullet can be fired every 0.25 seconds
            StartCoroutine(FireBullet());  //coroutine handles the bullet firing 
            }
        
        // Check for controller right trigger
        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightTrigger.wasPressedThisFrame && canFire)
            {
                StartCoroutine(FireBullet());
            }
        }
    }

        IEnumerator FireBullet() {
        canFire=false; //disables new bullets from firing until set to true
        GameObject newbullet = GameObject.Instantiate(bullet, this.gameObject.transform.position + this.gameObject.transform.forward, this.gameObject.transform.rotation); //instansiate bullet's position to the front of the spaceship, and give it the spaceship's rotation so it fires straight
        Bullet bulletComponent = newbullet.GetComponent<Bullet>();
        
        switch (currentGunType)
        {
        case GunType.Pistol:
        if (bulletComponent != null)
        {
            bulletComponent.setDamage(1f);
        }
        newbullet.transform.Rotate(90f, 0f, 0f, Space.Self);
        newbullet.GetComponent<Rigidbody>().AddForce((transform.forward * 20), ForceMode.Impulse); //give the bullet force
        yield return new WaitForSeconds(0.25f); 
        break;

        case GunType.Revolver:
        if (bulletComponent != null)
        {
            bulletComponent.setDamage(2.5f);
        }
        newbullet.transform.Rotate(90f, 0f, 0f, Space.Self);
        newbullet.GetComponent<Rigidbody>().AddForce((transform.forward * 20), ForceMode.Impulse); //give the bullet force
        yield return new WaitForSeconds(0.50f); 
        break;
        }
        canFire = true;
    }

        void EquipWeapon(int weaponIndex)
    {
        // Deactivate all weapons
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // Activate the selected weapon
        if (weaponIndex >= 0)
        {
            weapons[weaponIndex].SetActive(true);
            currentWeaponIndex = weaponIndex;
        }
    }
    public GunType getGunType() {
        return currentGunType;
    }

    void HandleWeaponSwitching()
    {
        // Keyboard switching weapons
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            currentWeaponIndex = 0;
            currentGunType = GunType.Pistol;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            currentWeaponIndex = 1;
            currentGunType = GunType.Revolver;
        }
        EquipWeapon(currentWeaponIndex);
        
        
        // Controller switching weapons (using dpad)
        if (Gamepad.current != null)
        {

            if (Gamepad.current.dpad.left.wasPressedThisFrame)
            {
                currentWeaponIndex = 0;
                currentGunType = GunType.Pistol;
            }
            else if (Gamepad.current.dpad.right.wasPressedThisFrame)
            {
                currentWeaponIndex = 1;
                currentGunType = GunType.Revolver;
            }

            EquipWeapon(currentWeaponIndex);
        }
    }
}
