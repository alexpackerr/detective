using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FadeController : MonoBehaviour
{

    private bool fadeIn, fadeOut, talking;
    [SerializeField]
    private float fadeSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    fadeIn = true;
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    fadeOut = true;
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    talking = !talking;
        //}

        if (fadeOut)
        {
            Color objectColor = this.GetComponent<SpriteRenderer>().color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.b, objectColor.g, fadeAmount);
            this.GetComponent<SpriteRenderer>().color = objectColor;

            if (objectColor.a <= 0)
            {
                fadeOut = false;
            }
        }
        else if (fadeIn)
        {
            Color objectColor = this.GetComponent<SpriteRenderer>().color;
            float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.b, objectColor.g, fadeAmount);
            this.GetComponent<SpriteRenderer>().color = objectColor;
            Debug.Log(this.GetComponent<SpriteRenderer>().color);
            if (objectColor.a >= 1)
            {
                fadeIn = false;
            }
        }
        else if(talking)
        {
            Color objectColor = this.GetComponent<SpriteRenderer>().color;
            this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, objectColor.a);
        }
        else if(!talking)
        {
            Color objectColor = this.GetComponent<SpriteRenderer>().color;
            this.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, objectColor.a);
        }

    }
    
    [YarnCommand("FadeOut")]
    public void FadeOut()
    {
        fadeOut = true;
    }

    [YarnCommand("FadeIn")]
    public void FadeIn()
    {
        fadeIn = true;
    }

    [YarnCommand("StartTalk")]
    public void StartTalk()
    {
        talking = true;
    }
    [YarnCommand("StopTalk")]
    public void StopTalk()
    {
        talking = false;
    }
}
