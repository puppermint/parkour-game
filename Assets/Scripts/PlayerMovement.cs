using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    public Transform startArea;

    public CharacterController controller;

    float speed;
    public float crouchSpeed = 6f;
    public float walkSpeed = 12f;
    public float runSpeed = 20f;

    public float gravity = -9.81f;
    float jumpHeight;
    public float standJumpHeight = 3f;
    public float crouchJumpHeight = 1.5f;

    public Transform playerModel;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    float height;
    float mHeight;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    bool isCrouching;

    Vector3 climbPos1;
    Vector3 climbPos2;
    bool canClimb;
    bool isClimbing;

    float startTime;
    float journeyLength;

    Vector3 slideDir;
    bool isSliding;

    public GameObject Timer;
    private TextMeshProUGUI timerText;
    private static int time = 0;
    private float second = 1f;

    public GameObject pauseMenuCanvas;
    bool pauseMenu = false;

    bool shouldEnd = false;

    public CanvasGroup fadeOut;
    float fadeOutTime;
    float fadeDuration = 1f;

    public GameObject UI;
    public GameObject lbEnter;

    private void Start()
    {
        height = controller.height;
        mHeight = playerModel.localScale.y;
        speed = walkSpeed;
        jumpHeight = standJumpHeight;
        isClimbing = false;
        canClimb = false;
        startTime = 0f;
        journeyLength = 0f;
        isCrouching = false;
        isSliding = false;

        timerText = Timer.GetComponent<TextMeshProUGUI>();
        time = 0;
        

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape") && !shouldEnd)
        {
            TogglePauseMenu();
        }

        if (pauseMenu == false && !shouldEnd)
        {

            float newHeight = height;
            float newMHeight = mHeight;

            if (second > 0)
            {
                second -= Time.deltaTime;
            }
            else
            {
                time++;
                second = 1;
            }
            timerText.SetText("Time: " + time);

            if (isGrounded)
            {
                speed = walkSpeed;
                jumpHeight = standJumpHeight;
            }

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetKey("left shift"))
            {
                speed = runSpeed;
            }


            if (isCrouching)
            {
                newHeight = 0.5f * height;
                newMHeight = 0.5f * mHeight;
                speed = crouchSpeed;
                jumpHeight = crouchJumpHeight;
                int mask = 1 << 8;
                mask = ~mask;
                if (!Input.GetKey("c") && !Physics.Raycast(groundCheck.position, Vector3.up, height, mask))
                {
                    isCrouching = false;
                }
            }

            if (Input.GetKey("left shift") && !isSliding && !isCrouching)
            {
                if (Input.GetKeyDown("c"))
                {
                    isSliding = true;
                    slideDir = transform.forward * runSpeed;
                }
            }
            else if (isSliding)
            {
                if (Input.GetKeyUp("c") || Input.GetKeyUp("left shift"))
                {
                    isSliding = false;
                }
            }

            if (Input.GetKeyDown("c"))
            {
                isCrouching = true;
            }

            if (Input.GetKey("w") && Input.GetKey("space") && canClimb && !isClimbing)
            {
                isClimbing = true;
                startTime = Time.time;
                journeyLength = Vector3.Distance(climbPos1, climbPos2);
            }

            if (isClimbing)
            {
                float distCovered = (Time.time - startTime) * 10.0f;
                float fractionOfJourney = distCovered / journeyLength;
                speed = 0;
                transform.position = Vector3.Lerp(climbPos1, climbPos2, fractionOfJourney);
                if (transform.position.Equals(climbPos2))
                {
                    isClimbing = false;
                }
            }

            float lastHeight = controller.height;
            controller.height = Mathf.Lerp(controller.height, newHeight, 5.0f * Time.deltaTime);

            float lastMHeight = playerModel.localScale.y;
            Vector3 newScale = new Vector3(playerModel.localScale.x, newMHeight, playerModel.localScale.z);
            playerModel.localScale = Vector3.Lerp(playerModel.localScale, newScale, 5.0f * Time.deltaTime);

            Vector3 p = transform.position;
            p.y += (controller.height - lastHeight) * 0.5f;
            transform.position = p;

            Vector3 s = playerModel.localScale;
            s.y += (playerModel.localScale.y - lastMHeight) * 0.5f;
            playerModel.localScale = s;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            //slideDir -= 4f * Time.deltaTime;
            slideDir = Vector3.Lerp(slideDir, new Vector3(0, 0, 0), 0.8f * Time.deltaTime);

            if (!isClimbing)
            {
                if (isSliding)
                {
                    controller.Move(slideDir * Time.deltaTime);
                }
                else
                {
                    controller.Move(move * speed * Time.deltaTime);
                }
                controller.Move(velocity * Time.deltaTime);
            }

        }

        if (shouldEnd)
        {
            fadeOutTime += Time.deltaTime;
            fadeOut.alpha = fadeOutTime / fadeDuration;
            if (fadeOutTime > fadeDuration * 2)
            {
                UI.SetActive(false);
                lbEnter.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }

    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == 10 && Physics.Raycast(transform.position, transform.forward, 1f) && !isClimbing)
        {
            int groundLayerMask = 1 << 9;
            Vector3 start = transform.position + transform.forward * controller.radius * 4 + transform.up * controller.height;
            RaycastHit pos;
            if(Physics.Raycast(start, Vector3.down, out pos, controller.height * 1f, groundLayerMask))
            {
                if (!Physics.Raycast(pos.point, Vector3.up, controller.height))
                {
                    climbPos1 = transform.position;
                    climbPos2 = pos.point + Vector3.up * controller.height * 0.5f;
                    canClimb = true;
                }
                
            }
            
        } else if (!Physics.Raycast(transform.position, transform.forward, 1f))
        {
            canClimb = false;
        }

        if (hit.gameObject.tag == "jumpPad")
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 6 * -2f * gravity);
        } 

        if(hit.gameObject.tag == "startArea")
        {
            time = 0;
        }

        if (hit.gameObject.tag == "endArea")
        {
            shouldEnd = true;
        }

        if (hit.gameObject.tag == "deathZone")
        {
            transform.position = startArea.position + Vector3.up * height * 0.5f;
        }

    }

    void TogglePauseMenu()
    {
        MouseLook.ToggleInputLock();

        if (pauseMenu == false)
            pauseMenu = true;
        else
            pauseMenu = false;

        if (pauseMenu)
        {
            pauseMenuCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pauseMenuCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public static int GetTime()
    {
        return time;
    }

}
