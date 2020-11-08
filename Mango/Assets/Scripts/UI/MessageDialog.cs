using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageDialog : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public TextMeshProUGUI buttonText;
    public Button button;

   public void SetMessage(string title, string content, string buttonText)
    {
        this.title.text = title;
        this.content.text = content;
        this.buttonText.text = buttonText;
        button.onClick.AddListener(() => Destroy(gameObject));
    }
}
