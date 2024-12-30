using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckOutOfBounds()); //coroutine to check if bullet is offscreen
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void setDamage(float d) {
        this.damage = d;
    }

    public float getDamage() {
        return this.damage;
    }

    IEnumerator CheckOutOfBounds() {
        while(true) {
            yield return new WaitForSeconds(0.2f); //runs 5 times a second
        //get the bullet coordinates in screen coordinates
        Vector3 bulletScreenPosition = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        //check if the bullet is off-screen and destroy it
        if (bulletScreenPosition.x > Screen.width || bulletScreenPosition.x < 0 || bulletScreenPosition.y > Screen.height || bulletScreenPosition.y < 0) {
            GameObject.Destroy(this.gameObject); //destroy the bullet if it goes offscreen
        } 
        }
    }
    void OnTriggerEnter(Collider other) {
     //Debug.Log("Bullet collided with something! destroying myself.");
     GameObject.Destroy(this.gameObject);
    }
}