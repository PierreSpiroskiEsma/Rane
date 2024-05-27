using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PatrolingAi_Prefab : MonoBehaviour
{

    // --- Statistic --- \\
    [Header("Enemi Statistics")]
    [SerializeField] private float speed;
    [SerializeField] private int Life_Point;

    // --- Patrol --- \\
    [Header("Patrol Detection Setting")]
    [SerializeField] private Transform Patrol_Detector_Position;
    [SerializeField] private Vector2 Patrol_Detector_Size;
    [SerializeField] private LayerMask Patrol_Detector_ColisionLayer;

    // --- Ai Setting --- \\
    [Header("AI Stetting")]
    [SerializeField] private bool Is_Alive;
    [SerializeField] private Transform Patrol_Zone, Left_Point, Right_Point;
    [SerializeField] private bool Default_Direction_Left;

    // --- State --- \\
    [SerializeField] private bool Is_Freeze, Is_On_Left;
    [SerializeField] private int Current_Patrol_Point;

    // --- Other --- \\
    private Rigidbody2D _RigideBody;
    private SpriteRenderer _SpriteRenderer;
    private Animator _Animator;
    private Transform _Transform;




    // ***************************************************************************************** \\
    // Setup
    // ***************************************************************************************** \\

    private void OnEnable() {

        _RigideBody = this.GetComponent<Rigidbody2D>();
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _Transform = this.GetComponent<Transform>();

        Patrol_Detector_Position = _Transform;

        Is_Freeze = false;

        if (Default_Direction_Left) {

            Current_Patrol_Point = 0;

        } else {

            Current_Patrol_Point = 1;
        }

    }

    // ***************************************************************************************** \\
    // Update
    // ***************************************************************************************** \\

    private void FixedUpdate() {

        if (Is_Alive && !Is_Freeze ) {

            Go_Patrol();
        }

        Is_Dead();
    }

    // ***************************************************************************************** \\
    // Assign Methode
    // ***************************************************************************************** \\

    public void Assign_Patrol(Transform _Patrol) {

        Patrol_Zone = _Patrol.transform;
    }

    // ***************************************************************************************** \\
    // Scan Methode
    // ***************************************************************************************** \\

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Patrol")
        {

            Assign_Patrol(collision.transform);
        }

    }


    // ***************************************************************************************** \\
    // Patrol Methode
    // ***************************************************************************************** \\

    private void Go_Patrol() {

        //Search_Patrol_Point();

        _SpriteRenderer.flipX = Default_Direction_Left;

        float CleenSpeed = speed * Time.deltaTime;

        Left_Point = Patrol_Zone.transform.Find("Patrol_Left_Position").GetComponent<Transform>();
        Right_Point = Patrol_Zone.transform.Find("Patrol_Right_Position").GetComponent<Transform>();

        if (Current_Patrol_Point == 0) {

            _SpriteRenderer.flipX = false;

           CleenSpeed = CleenSpeed * -1;

            if (Vector2.Distance(_Transform.position, Left_Point.position) < 0.25f) {

                Current_Patrol_Point = 1;
            }
        }
        
        if (Current_Patrol_Point == 1) {

            _SpriteRenderer.flipX = true;
            if (Vector2.Distance(_Transform.position, Right_Point.position) < 0.25f) {

                Current_Patrol_Point = 0;
            }
        }

        Vector2 _velocity = _RigideBody.velocity;
        _velocity.x = CleenSpeed;
        _RigideBody.velocity = _velocity;
    }


    // ***************************************************************************************** \\
    // Freeze Methode
    // ***************************************************************************************** \\
    public void FreezOn() {

        Is_Freeze = true;
    }

    public void FreezOff() {

        Is_Freeze = false;
    }

    // ***************************************************************************************** \\
    // Death Detection and application
    // ***************************************************************************************** \\
    private void Is_Dead()
    {

        if (Life_Point <= 0)
        {

            Destroy(gameObject, 0.5f);
        }

    }

}