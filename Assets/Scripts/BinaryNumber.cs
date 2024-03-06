using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BinaryNumber : MonoBehaviour
{
    // Start is called before the first frame update
    private static readonly string binCode;
    [SerializeField] private TMP_Text BinCode;
    private int randomNumber;
    private string binaryCode;
    void start()
    {
        int randomNumber = Random.Range(0, 16);
        binaryCode = System.Convert.ToString(randomNumber, 2).PadLeft(4, '0');
        BinCode.text = randomNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
