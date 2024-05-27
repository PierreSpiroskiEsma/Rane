using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Script_Coin_UI : MonoBehaviour
{

    [SerializeField] Transform Player;

    private TextMeshProUGUI _TextMeshPro;
    

    // Start is called before the first frame update
    void Start(){

        _TextMeshPro = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _TextMeshPro.text = Player.GetComponent<player_moveset>().Get_Collectible_Amount().ToString();
    }
}
