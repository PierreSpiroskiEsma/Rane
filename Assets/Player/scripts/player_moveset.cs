using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class player_moveset : MonoBehaviour
{

    /* --- Stat --- */
    [Header("Stat")]
    [SerializeField] private float Movement_Speed;
    [SerializeField] private float Jump_Height;
    [SerializeField] private float Fall_MaxSpeed;


    /* --- GroundCheck --- */
    [Header("GroundCheck")]
    [SerializeField] private Transform Ground_Check_Position;
    [SerializeField] private float Ground_Check_Radius;
    [SerializeField] private LayerMask Ground_Check_Layer;

    /* --- Control --- */
    private Player_manager _player_manager;
    private InputAction _Move, _Jump;
    private Vector2 Input_Direction;

    /* --- Shortcut --- */
    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    /* --- State --- */
    private bool _IsGrounded;

    /* --- Enemy --- */
    [Header("Enemy")]
    [SerializeField] private Transform Enemy_Detector_position;
    [SerializeField] private float Enemy_Detector_Radius;
    [SerializeField] private LayerMask Enemy_Detector_LayerMask;


    /* --- Collectible --- */
    [Header("Collectible")]
    [SerializeField] private int Coin_Amount;


    /* --- Waypoint --- */
    [Header("Waypoint")]
    [SerializeField] private Transform Waypoint_Detector_position;
    [SerializeField] private float Waypoint_Detector_Radius;
    [SerializeField] private LayerMask Waypoint_Detector_LayerMask;
    [SerializeField] private Transform Waypoint_Bag;

    /* --- Debug --- */

    [Header("Debug")]
    [SerializeField] private bool Debug_jump = false;
    [SerializeField] private bool Debug_Waypoint_Detector = false;
    [SerializeField] private bool Debug_Enemy_Detector = false;






    /************************************  Setup  ************************************/


    void Awake() {

        _player_manager = new Player_manager();
        _Move = _player_manager.Player.Move;
        _Jump = _player_manager.Player.Jump;

        Coin_Amount = 0;
    }

    private void OnEnable() {

        _rigidbody = this.GetComponent<Rigidbody2D>();

        _Move.Enable();
        _Jump.Enable();

        _Jump.performed += On_Jump;
        _Jump.canceled += Cancel_Jump;
        _Move.performed += On_Move;

        Ground_Check_Position = transform.Find("Overlap_Jump").GetComponent<Transform>();
    }
  
    private void OnDisable() {

        _Move.Disable();
        _Jump.Disable();

        _Jump.performed -= On_Jump;
        _Move.performed -= On_Move;
    }


    /************************************  Update  ************************************/


    private void FixedUpdate() {

        Movement_Update();
        Fall_Manager();

        Ground_detection();
        Enemy_Detection();
        Waypoint_Detection();
    }


    /************************************  Debug  ************************************/


    private void OnDrawGizmos() {
        
        if (Debug_jump) {

            if (_IsGrounded) {

                Gizmos.color = Color.green;

            } else {

                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(Ground_Check_Position.position, Ground_Check_Radius);
        }


        if (Debug_Waypoint_Detector) {

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Waypoint_Detector_position.position, Waypoint_Detector_Radius);
        }


        if (Debug_Enemy_Detector) {


            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Enemy_Detector_position.position, Enemy_Detector_Radius);
        }

    }


    /************************************  Mouvement  ************************************/


    private void Movement_Update() {

        Input_Direction = _Move.ReadValue<Vector2>();

        Vector2 Player_Velocity = _rigidbody.velocity;
        Player_Velocity.x = ((Movement_Speed * Time.deltaTime) * Input_Direction.x);

        _rigidbody.velocity = Player_Velocity;

    }

    private void On_Move(InputAction.CallbackContext context) {

        Debug.Log("ça marche <3");
    }


    /************************************  Jump  ************************************/


    private void On_Jump(InputAction.CallbackContext context) {

        Debug.Log("il saute :)");

        if (_IsGrounded && _rigidbody.velocity.y < 0.05) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, (Jump_Height * Time.deltaTime));
            // lancer annimation saut
        } 
            
    }

    private void Cancel_Jump(InputAction.CallbackContext context) {

        if (_rigidbody.velocity.y > 0.01) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, (_rigidbody.velocity.y*0.5f));
        }
    }


    /************************************  Collectible  ************************************/


    private void Add_Collectible() {

        Coin_Amount = Coin_Amount + 1;

        //jouer le son des piece
    }

    public int Get_Collectible_Amount() {

        return Coin_Amount;
    }


    /************************************  Respawn  ************************************/


    private void Respawn() {

        // trigger animation death

        _rigidbody.position = Waypoint_Bag.position;

        // trigger animation Respawn
    }



    private void Change_Waypoint(Transform Target) {

        Waypoint_Bag = Target;

    }


    /************************************  detection  ************************************/


    /* --- Ground --- */
    private void Ground_detection() {

        Collider2D[] Ground_detection = Physics2D.OverlapCircleAll(Ground_Check_Position.position, Ground_Check_Radius, Ground_Check_Layer);

        _IsGrounded = false;
        
        foreach (var Object in Ground_detection) { 
        
            Debug.Log( Object.name);    

            if ( Object.tag == "Ground_World") {

                _IsGrounded = true;
            }
        }
    }


    /* --- Enemy --- */
    private void Enemy_Detection() {

        Collider2D[] Enemy_Detector = Physics2D.OverlapCircleAll(Enemy_Detector_position.position, Enemy_Detector_Radius, Enemy_Detector_LayerMask);

        foreach (var Object in Enemy_Detector) {

            if (Object.tag == "Enemy") {

                Respawn();
            }
        }
    }

    /* --- Coin --- */
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.CompareTag("Coin")) {

            Add_Collectible();
        }

    }

    /* --- Waypoint --- */
    private void Waypoint_Detection() {

        Collider2D[] Waypoint_Catcher = Physics2D.OverlapCircleAll(Waypoint_Detector_position.position, Waypoint_Detector_Radius, Waypoint_Detector_LayerMask);

        foreach (var Object in Waypoint_Catcher) {

            if (Object.tag == "Waypoint") {

                Change_Waypoint(Object.transform);
            }
        }

    }


    /************************************  Fall  ************************************/


    private void Fall_Manager() {

        if (_rigidbody.velocity.y < Fall_MaxSpeed) {

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Fall_MaxSpeed);
        }
    }


}
