using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{

    [SerializeField]
    private MenuUI menu;
    [SerializeField]
    private Button button;
    [SerializeField]
    private float moveSpeed = 50;

    private bool close = false;

    private void Update()
    {
        Vector3 position = menu.GetComponent<RectTransform>().position;
        if (close)
        {
            menu.GetComponent<RectTransform>().position = new Vector3(position.x, position.y - Time.deltaTime * moveSpeed, position.z);
            if(position.y <= -1 * Screen.height / 2)
            {
                close = false;
                button.gameObject.SetActive(true);
                menu.gameObject.SetActive(false);
            }
        }
        else if(position.y < Screen.height / 2)
        {

            menu.GetComponent<RectTransform>().position = new Vector3(position.x, position.y + Time.deltaTime * moveSpeed, position.z);
        }
        else if(position.y > Screen.height / 2)
        {
            menu.GetComponent<RectTransform>().position = new Vector3(position.x, Screen.height/2, position.z);
        }
    }

    public void Close()
    {
        close = true;
    }
}