using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public Text coinText;
    public int coins;

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Парички : " + coins;
    }
}
