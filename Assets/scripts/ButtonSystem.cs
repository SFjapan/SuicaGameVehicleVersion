using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSystem : MonoBehaviour
{
    //タッチ操作での処理
    public Crane crane;
    public bool onRight;
    public bool onLeft;
    public bool showButton = true;
    [SerializeField] Text showButtonText;
    [SerializeField] GameObject right, left, drop;
    void Start()
    {
        showButton = true;
    }
    void Update(){
        if (onRight) 
        { 
            crane.transform.Translate(crane.transform.right * crane.speed * Time.deltaTime); 
            right.GetComponent<Image>().color = Color.red;
        }
        if (onLeft){
            crane.transform.Translate(crane.transform.right * -crane.speed * Time.deltaTime);
            left.GetComponent<Image>().color = Color.red;
        } 
    }

    public void OnRightButtonDown()
    {
        onRight = true;
    }
    public void OnRightButtonUp()
    {
        onRight = false;
        right.GetComponent<Image>().color = Color.green;
    }
    

    public void OnLeftButtonDown()
    {
        onLeft = true;
    }

    public void OnLeftButtonUp()
    {
        onLeft = false;
        left.GetComponent<Image>().color = Color.green;
    }
    
    public void Drop()
    {
        crane.MouseDrop();
    }

    public void SetButton()
    {
        showButton = !showButton;
        if (showButton)
        {
            showButtonText.text = "button:hidden";
            right.gameObject.SetActive(true);
            left.gameObject.SetActive(true);
            drop.gameObject.SetActive(true);
        }
        else
        {
            showButtonText.text = "button:active";
            right.gameObject.SetActive(false);
            left.gameObject.SetActive(false);
            drop.gameObject.SetActive(false);
        }
    }
}
