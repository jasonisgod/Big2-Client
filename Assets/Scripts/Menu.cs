using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
//using System.String;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject[] cards;
    public GameObject[] heads;
    public GameObject[] arrows;
    public GameObject inputIP;

    float timer = 0f;
    bool isMenu = true;
    
    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.1)
        {
            timer = 0;
            foreach (var card in cards)
            {
                if (Random.Range(0f, 1f) < 0.01)
                {
                    card.GetComponent<Damping>().Hit();
                }
            } 
        }
        Game.url = "http://" + inputIP.GetComponent<InputField>().text + "/";
    }

    public void OnClickSwitch()
    {
        isMenu = !isMenu;
        menu.SetActive(isMenu);
    }

    void OnClickPlayer(int id)
    {
        Game.id = id;
        //Game.SetId(id);
        for (var p = 1; p <= 4; p++)
        {
            arrows[p].GetComponent<Image>().enabled = (p == id);
        }
    }

    public void OnClickP1() { OnClickPlayer(1); }
    public void OnClickP2() { OnClickPlayer(2); }
    public void OnClickP3() { OnClickPlayer(3); }
    public void OnClickP4() { OnClickPlayer(4); }
    
    public void OnEnter()
    {
        StartCoroutine(RequestEnter());
    }
    
    public void OnStart()
    {
        StartCoroutine(RequestStart());
        OnClickSwitch();
    }
    
    public void OnStop()
    {
        StartCoroutine(RequestStop());
        //OnClickSwitch();
    }
    
    IEnumerator RequestEnter()
    {
        using (var webRequest = UnityWebRequest.Get(Game.url + "enter/" + Game.id))
        {
            yield return webRequest.SendWebRequest();
        }
    }

    IEnumerator RequestStart()
    {
        using (var webRequest = UnityWebRequest.Get(Game.url + "start"))
        {
            yield return webRequest.SendWebRequest();
        }
    }
    
    IEnumerator RequestStop()
    {
        using (var webRequest = UnityWebRequest.Get(Game.url + "reset"))
        {
            yield return webRequest.SendWebRequest();
        }
    }

}
