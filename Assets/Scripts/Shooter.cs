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
    
    // prices and owner status of each weapon for points purchasing
    public int[] prices;
    public bool[] owned;
     
    public enum GunType
    {
        Pistol,
        Revolver,
        Rifle
    }
    public GunType currentGunType;
    
    private CharacterScript cs; 

    void Awake()
    {
        // Set the gun's parent to the hand
        transform.SetParent(hand);
        
        transform.localRotation = Quaternion.Euler(278.477814f,348.370789f,102.123825f);
        EquipWeapon(currentWeaponIndex);
        
        cs = GetComponentInParent<CharacterScript>(); 
    }

    void Start()
    {
        prices = new int[]{0, 1000, 2000};
        owned = new bool[]{true, false, false};
        // owned = new bool[] { true, true, true };
    }

    void Update()
    {
        // Keep this shooter transform at the correct offset from the hand
        transform.position = hand.position + offset;
        
        HandleShooting();
        HandleWeaponSwitching();
    }

    void HandleShooting() 
    {
        // Mouse shooting
        if (Input.GetMouseButtonDown(0) && canFire)
        {
            cs.isShooting = true;
            StartCoroutine(FireBullet());
        }

        // Controller shooting
        if (Gamepad.current != null)
        {
            // Right trigger
            if (Gamepad.current.rightTrigger.wasPressedThisFrame && canFire)
            {
                cs.isShooting = true;
                StartCoroutine(FireBullet());
            }
        }
    }

    IEnumerator FireBullet()
    {
        canFire = false; // Disable new bullets until done
        
        yield return new WaitUntil(() => cs.isReadyToFireBullet);

        // Instantiate bullet at this Shooter’s position + forward
        GameObject newbullet = GameObject.Instantiate(
            bullet,
            this.gameObject.transform.position + this.gameObject.transform.forward,
            this.gameObject.transform.rotation
        );
        Bullet bulletComponent = newbullet.GetComponent<Bullet>();

        // Existing gun-type logic
        switch (currentGunType)
        {
            case GunType.Pistol:
                if (bulletComponent != null)
                {
                    bulletComponent.setDamage(1f);
                }

                newbullet.transform.Rotate(90f, 0f, 0f, Space.Self);
                newbullet.GetComponent<Rigidbody>()
                    .AddForce((transform.forward * 20), ForceMode.Impulse); // bullet force
                yield return new WaitForSeconds(0.25f);
                break;

            case GunType.Revolver:
                if (bulletComponent != null)
                {
                    bulletComponent.setDamage(2.5f);
                }

                newbullet.transform.Rotate(90f, 0f, 0f, Space.Self);
                newbullet.GetComponent<Rigidbody>()
                    .AddForce(transform.forward * 0.1f, ForceMode.Impulse); // bullet force
                yield return new WaitForSeconds(0.50f);
                break;

            case GunType.Rifle:
                if (bulletComponent != null)
                {
                    bulletComponent.setDamage(3f);
                }

                newbullet.transform.Rotate(90f, 0f, 0f, Space.Self);
                newbullet.GetComponent<Rigidbody>()
                    .AddForce(transform.forward * 25, ForceMode.Impulse); // bullet force
                yield return new WaitForSeconds(0.15f);
                break;
        }

        canFire = true;
        
        cs.isShooting = false;
    }

    public void EquipWeapon(int weaponIndex)
    {
        // Deactivate all weapons
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        
        if (weaponIndex < 0 || weaponIndex >= weapons.Length) return;
        
        weapons[weaponIndex].SetActive(true);
        currentWeaponIndex = weaponIndex;

        // Set gun to parent's hand
        weapons[weaponIndex].transform.SetParent(hand);
        
        weapons[weaponIndex].transform.localPosition = Vector3.zero;
        weapons[weaponIndex].transform.localRotation = Quaternion.identity;
        
        // Pistol
        if (weaponIndex == 0)
        {
            weapons[weaponIndex].transform.localPosition = new Vector3(-0.002f, 0.202f, 0.036f);
            weapons[weaponIndex].transform.localRotation = Quaternion.Euler(278.48f, 348.37f, 102.12f);
        }
        
        // Revolver
        else if (weaponIndex == 1)
        {
            weapons[weaponIndex].transform.localPosition = new Vector3(0.006f, 0.228f, 0.011f);
            weapons[weaponIndex].transform.localRotation = Quaternion.Euler(278.48f, 348.37f, 102.12f);
        }
        
        // Rifle
        else if (weaponIndex == 2) 
        {
            weapons[weaponIndex].transform.localRotation = Quaternion.Euler(-123.98f, -69.077f, 198.7f);
            weapons[weaponIndex].transform.localPosition = new Vector3(-0.02f, 0.031f, 0.041f);
        }
    }

    public GunType getGunType()
    {
        return currentGunType;
    }

    void HandleWeaponSwitching()
    {
        // get char script
        CharacterScript cs = player.GetComponent<CharacterScript>();

        // track if weapon is switched
        bool weaponSwitched = false;
        
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X))
        {
            // this is a secret
            owned = new bool[] { true, true, true };
            Debug.Log("All weapons unlocked!");
        }

        // Keyboard switching
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeaponIndex != 0 && owned[0])
        {
            currentWeaponIndex = 0;
            currentGunType = GunType.Pistol;
            weaponSwitched = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeaponIndex != 1 && owned[1])
        {
            currentWeaponIndex = 1;
            currentGunType = GunType.Revolver;
            weaponSwitched = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeaponIndex != 2 && owned[2])
        {
            currentWeaponIndex = 2;
            currentGunType = GunType.Rifle;
            weaponSwitched = true;
        }

        // Controller switching (dpad)
        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.left.wasPressedThisFrame && currentWeaponIndex != 0 && owned[0])
            {
                currentWeaponIndex = 0;
                currentGunType = GunType.Pistol;
                weaponSwitched = true;
            }
            else if (Gamepad.current.dpad.right.wasPressedThisFrame && currentWeaponIndex != 1 && owned[1])
            {
                currentWeaponIndex = 1;
                currentGunType = GunType.Revolver;
                weaponSwitched = true;
            }
            else if (Gamepad.current.dpad.up.wasPressedThisFrame && currentWeaponIndex != 2 && owned[2])
            {
                currentWeaponIndex = 2;
                currentGunType = GunType.Rifle;
                weaponSwitched = true;
            }
        }
        
        if (weaponSwitched)
        {
            // Freeze player so the weapon switch doesn't break animations, etc.
            cs.freezePlayer();
            EquipWeapon(currentWeaponIndex);
        }
    }
}
