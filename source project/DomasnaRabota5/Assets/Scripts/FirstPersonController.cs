using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // referenci
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // podesuvanja za igracot
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;

    // detekcija na dopir
    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    // kontrola na kamerata
    private Vector2 lookInput;
    private float cameraPitch;

    // dvizenje 
    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    void Start()
    {
        // id = -1 znaci deka prstot ne se registrira
        leftFingerId = -1;
        rightFingerId = -1;

        // sirina na polovina ekran
        halfScreenWidth = Screen.width / 2;

        // presmetka na dead zonata za dvizenje
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    void Update()
    {
        GetTouchInput();


        if (rightFingerId != -1)
        {
            // se vrsi rotacija samo ako se registrira desniot prst
            Debug.Log("Rotiranje");
            LookAround();
        }

        if (leftFingerId != -1)
        {
            // se vrsi pridvizuvanje samo ako se registrira leviot prst
            Debug.Log("Dvizenje");
            Move();
        }
    }

    void GetTouchInput()
    {
        // iteriraj niz site detektirani dopiri
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // proveri ja sekoja faza na dopir
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        // registriranje na leviot prst dokolku prethodno ne bil registriran
                        leftFingerId = t.fingerId;

                        // postavi pocetna pozicija za prstot za dvizenje
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // registriranje na desniot prst dokolku prethodno ne bil registriran
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // prekini so registriranje na dvizenjeto
                        leftFingerId = -1;
                        Debug.Log("Prekin na dvizenjeto");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // prekini so registriranje na kamerata
                        rightFingerId = -1;
                        Debug.Log("Prekin na dvizenje na kamerata");
                    }

                    break;
                case TouchPhase.Moved:

                    // vnes za gledanje naokolu
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {

                        // presmetuvanje na delta pozicijata od pocetnata pozicija
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // postavi go vnesot za orientacija na 0 ako prstot ne se registrira
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {

        // vertikalna rotacija
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontalna rotacija
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {

        // nema dvizenje dokolku delta na dopir e pokratka otkolku posakuvanata dead zona
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

        // mnozenje na normaliziranata nasoka so brzinata
        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        // relativno dvizenje vo nasoka na lokalnata transformacija
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

}