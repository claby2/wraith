using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInformation : MonoBehaviour {
    public Text ItemName;
    public Text Description;

    public void SetDisplay(string name, string description) {
        ItemName.text = name;
        Description.text = description;
    }
}
