using System.Collections;
using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject player;
    public Vector3 offset;
    public bool follow = true;

    void Start()
    {
        // Set the offset to place the camera directly above the player
        offset = new Vector3(0f, 10f, 0f); // Adjust the Y-value to control height above the player
        StartCoroutine(FollowPlayer()); // Start following the player
    }

    IEnumerator FollowPlayer()
    {
        while (follow) // Run this coroutine indefinitely
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

    public IEnumerator pan(float duration)
    {
        Debug.Log("Panning camera");
        // float duration = 600f;
        float time = 0f;

        // pan camera for duration
        while (time < duration)
        {
            time += Time.deltaTime;
            
            // move camera up
            mainCamera.transform.position += new Vector3(0f, 1f * Time.deltaTime, 0f);
            
            // wait until next frame
            yield return null;
        }
    }
}
