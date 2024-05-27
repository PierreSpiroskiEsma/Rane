using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Script_CoinDoor : MonoBehaviour {

    // --- Door Cost --- \\
    [Header("Door Cost")]
    [SerializeField] private int Price;

    [Header("Player")]
    [SerializeField] private Transform Player;

    private Light2D Red, Green;
    private Transform _Wall;


    void Start() {

        Red = this.transform.Find("Lgt_Red").GetComponent<Light2D>();
        Green = this.transform.Find("Lgt_Green").GetComponent<Light2D>();

        _Wall = this.transform.Find("Wall").GetComponent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.tag == "Player") {

            Coin_check();

        }
    }

    private void Coin_check() {

        if (Player.GetComponent<player_moveset>().Get_Collectible_Amount() < Price) {
        
            Red.intensity = 2;
            Green.intensity = 0;
        
        } else {

            Red.intensity = 0;
            Green.intensity = 2;

            _Wall.GetComponent<BoxCollider2D>().enabled = false;
        }

    }

}
