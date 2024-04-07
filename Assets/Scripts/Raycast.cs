using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Raycast : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Camera mainCamera;
    public GameObject playerObject;
    public float maxDistance;
    private GameObject lastHitObject;
    // public Transform leftDoor;
    // public Transform rightDoor;
    private Transform leftDoor;
    private Transform rightDoor;
    private bool isOpenedDoor = false;
    // public Transform leftSlideDoor;
    // public Transform rightSlideDoor;
    private Transform leftSlideDoor;
    private Transform rightSlideDoor;
    private bool isSlidedDoor = false;
    private bool isHidden = false;
    private Vector3 lastPosition;
    public GameObject menu;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        // 設定射線寬度
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
    }

    void Update()
    {
        // 計算射線起點，將其設定在相機下方位置
        Vector3 rayOrigin = mainCamera.transform.position - mainCamera.transform.up * 0.025f;
        Ray ray = new Ray(rayOrigin, mainCamera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            DrawRay(ray.origin, hit.point);
            if (lastHitObject != hit.collider.gameObject)
            {
                RestoreOriginalColor();
                lastHitObject = hit.collider.gameObject;
            }

            if (hit.collider.CompareTag("Button"))
            {
                string objectName = hit.collider.gameObject.name;
                if (objectName == "Resume")
                {
                    if (Input.GetAxis("js11") != 0)
                    {
                        menu.SetActive(false);
                        CharacterMovement targetScript = playerObject.GetComponent<CharacterMovement>();
                        targetScript.enabled = true;
                    }
                }
                else if(objectName == "Exit")
                {
                    Application.Quit();
                }
            }

            if (!hit.collider.gameObject.GetComponent<Outline>() && !hit.collider.CompareTag("Ground"))
            {
                var outline = hit.collider.gameObject.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 5f;
            }

            // Use X on keyboard or js11 on joystick to interact with objects
            if (Input.GetAxis("js11") != 0 || Input.GetAxis("js24") != 0 || Input.GetKeyDown(KeyCode.X))
            {
                string gazedObjectName = hit.collider.gameObject.name;
                GameObject temp = GameObject.Find(gazedObjectName);

                // If object is a door (tagged as "Door1", "Door2", etc.)
                if (hit.collider.CompareTag("Door"))
                {
                    // Play object's audio source
                    AudioSource audioSourceToUse = temp.GetComponent<AudioSource>();
                    audioSourceToUse.Play();

                    // Modify transform to gazed object
                    leftDoor = temp.transform.Find("Left Door").transform;
                    rightDoor = temp.transform.Find("Right Door").transform;

                    RotateDoors();
                    
                    if (isOpenedDoor)
                    {
                        hit.collider.isTrigger = true;
                    }
                    else
                    {
                        hit.collider.isTrigger = false;
                    }
                }
                else if (hit.collider.CompareTag("Slide Door"))
                {
                    // Play object's audio source
                    AudioSource audioSourceToUse = temp.GetComponent<AudioSource>();
                    audioSourceToUse.Play();

                    // Modify transform to gazed object
                    leftSlideDoor = temp.transform.Find("Left Slide Door").transform;
                    rightSlideDoor = temp.transform.Find("Right Slide Door").transform;

                    SlideDoors();

                    if (isSlidedDoor)
                    {
                        hit.collider.isTrigger = true;
                    }
                    else
                    {
                        hit.collider.isTrigger = false;
                    }
                }
                else if (hit.collider.CompareTag("Hide Place"))
                {
                    Vector3 bedPosition = hit.collider.gameObject.transform.position;
                    Hide(bedPosition);
                }
            }
        }
        else
        {
            DrawRay(ray.origin, ray.origin + ray.direction * maxDistance);
            RestoreOriginalColor();
            lastHitObject = null;
        }
        // B Button on joystick to open menu
        if ((Input.GetAxisRaw("js7") != 0 || Input.GetAxisRaw("15") != 0) && !menu.activeSelf)
        {
            menu.SetActive(true);
            CharacterMovement targetScript = playerObject.GetComponent<CharacterMovement>();
            targetScript.enabled = false;
        }
    }

    void RotateDoors()
    {
        Vector3 leftDoorMove = new Vector3(-1, 0, 1);
        Vector3 rightDoorMove = new Vector3(1, 0, 1);
        Vector3 leftDoorRotate = new Vector3(0, 90, 0);
        Vector3 rightDoorRotate = new Vector3(0, -90, 0);
        if (!isOpenedDoor)
        {
            leftDoor.localPosition += leftDoorMove;
            leftDoor.localRotation *= Quaternion.Euler(leftDoorRotate);
            rightDoor.localPosition += rightDoorMove;
            rightDoor.localRotation *= Quaternion.Euler(rightDoorRotate);
            isOpenedDoor = true;
        }
        else
        {
            leftDoor.localPosition -= leftDoorMove;
            leftDoor.localRotation *= Quaternion.Euler(-leftDoorRotate);
            rightDoor.localPosition -= rightDoorMove;
            rightDoor.localRotation *= Quaternion.Euler(-rightDoorRotate);
            isOpenedDoor = false;
        }

        // start an audio source
        // AudioSource audioSource = GetComponent<AudioSource>();
        // audioSource.Play();
    }
    void SlideDoors()
    {
        Vector3 leftDoorMove = new Vector3(-2, 0, 0);
        Vector3 rightDoorMove = new Vector3(2, 0, 0);
        if (!isSlidedDoor)
        {
            leftSlideDoor.localPosition += leftDoorMove;
            rightSlideDoor.localPosition += rightDoorMove;
            isSlidedDoor = true;
        }
        else
        {
            leftSlideDoor.localPosition -= leftDoorMove;
            rightSlideDoor.localPosition -= rightDoorMove;
            isSlidedDoor = false;
        }
    }

    void Hide(Vector3 bedPosition)
    {
        if (!isHidden)
        {
            lastPosition = playerObject.transform.position;
            CharacterMovement targetScript = playerObject.GetComponent<CharacterMovement>();
            targetScript.enabled = false;
            Vector3 offset = new Vector3(0f, -3f, 0f);
            playerObject.transform.position = bedPosition + offset;
            isHidden = true;
        }
        else
        {
            CharacterMovement targetScript = playerObject.GetComponent<CharacterMovement>();
            targetScript.enabled = true;
            playerObject.transform.position = lastPosition;
            isHidden = false;
        }
    }

    void DrawRay(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private void RestoreOriginalColor()
    {
        if (lastHitObject != null)
            Destroy(lastHitObject.GetComponent<Outline>());
    }
}
