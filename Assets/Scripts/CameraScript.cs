using System.Collections;
using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject player;
    public Vector3 offset; // Offset between player and camera

    void Start()
    {
        // Set the offset to place the camera directly above the player
        offset = new Vector3(0f, 10f, 0f); // Adjust the Y-value to control height above the player
        StartCoroutine(FollowPlayer()); // Start following the player
    }

    IEnumerator FollowPlayer()
    {
        while (true) // Run this coroutine indefinitely
        {
            // Calculate the target position directly above the player
            Vector3 targetPos = player.transform.position + offset;

            // Smoothly interpolate the camera position towards the target position
            mainCamera.transform.position =
                Vector3.MoveTowards(mainCamera.transform.position, targetPos, 10f * Time.deltaTime);

            // Set the camera to look directly downward
            mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            // Wait until the next frame
            yield return null;
        }
    }
}
