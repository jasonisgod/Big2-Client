using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
//using System.String;

public class Game : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject cardReadonlyPrefab;
    public GameObject center;
    public GameObject[] players;
    public Sprite[] images;
    public GameObject[] heads;
    public GameObject[] arrows;
    public GameObject buttonGo;
    public GameObject buttonPass;
    
    string state;
    int player;
    List<int> defaultTop = new List<int>(); 
    List<int> top = new List<int>(); 
    List<int> lastTop = new List<int>();
    List<List<int>> options;
    List<List<int>> hands;
    List<List<int>> defaultHands;

    public static int id = 1;
    public static string url = "http://47.242.237.127/";

    int pollingN = -1;
    float timer = 0;

    int PlayerIdToSeatId(int p)
    {
        var shiftTable = new List<int>() {0, 1, 2, 3, 4, 1, 2, 3, 4}; 
        return shiftTable[p - id + 5];
    }

    void Start()
    {
        defaultTop = new List<int>();
        defaultHands = new List<List<int>>()
        {
            new List<int>(),
            new List<int>() {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            new List<int>() {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            new List<int>() {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            new List<int>() {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}
        };
        //Refresh();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5)
        {
            timer = 0;
            StartCoroutine(RequestPolling());
        }
        for (var p = 1; p <= 4; p++)
        {
            var pp = PlayerIdToSeatId(p);
            heads[pp].GetComponent<Image>().sprite = images[p];
        }
    }

    void Refresh()
    {
        if (state == "wait")
        {
            top = defaultTop;
            hands = defaultHands;
        }
        if (!top.SequenceEqual(lastTop))
        {
            center.GetComponent<Damping>().Hit();
        }
        buttonGo.SetActive(id == player && state == "game");
        buttonPass.SetActive(id == player && state == "game");
        for (var p = 1; p <= 4; p++) 
        {
            var pp = PlayerIdToSeatId(p);
            heads[pp].GetComponent<Floating>().speed = 0;
            arrows[pp].GetComponent<Image>().enabled = (p == player && state == "game");
            foreach (Transform child in players[pp].transform) 
            {
                Destroy(child.gameObject);
            }
            for (var i = 0; i < hands[p].Count; i++)
            {
                var num = hands[p][i];
                var card = GetCard(num, p != id);
                card.transform.position = new Vector3(i * 80 - 500, 0, 0);
                card.transform.SetParent(players[pp].transform, false);
            }
        }
        foreach (Transform child in center.transform) 
        {
            Destroy(child.gameObject);
        }
        for (var i = 0; i < top.Count; i++)
        {
            var num = top[i];
            var card = GetCard(num, true);
            card.transform.position = new Vector3((i - top.Count/2f) * 80 + 50, 0, 0);
            card.transform.SetParent(center.transform, false);
        }
    }

    public GameObject GetCard(int num, bool isReadonly)
    {
        var card = Instantiate(isReadonly? cardReadonlyPrefab: cardPrefab);
        var img = card.transform.Find("Mask/Image").gameObject;
        var rt = img.GetComponent<RectTransform>();
        var dc = -140; var dr = 211;
        if (num == -1) 
        {
            rt.anchoredPosition = new Vector3(13 * dc, 2 * dr, 0);
        } else {
            var row = (52-num) % 4;
            var col = (52-num) / 4;
            rt.anchoredPosition = new Vector3(col * dc, row * dr, 0);
        }
        return card;
    }

    public void OnGo()
    {
        var list = new List<int>();
        for (var i = 0; i < hands[id].Count; i++)
        {
            var mask = players[1].transform.GetChild(i).GetChild(0);
            var flag = mask.localPosition.y > 0;
            if (flag)
            {
                list.Add(i);
            }
            //Debug.Log(mask.localPosition.y);
        }
        if (list.Count == 0)
        {
            return;
        }
        foreach (var option in options)
        {
            if (option.SequenceEqual(list))
            {
                var data = "[" + string.Join(",", list) + "]";
                StartCoroutine(RequestPlay(data));
                break;
            }
        }
        //Debug.Log(data);
    }

    public void OnPass()
    {
        var data = "[]";
        StartCoroutine(RequestPlay(data));
    }

    IEnumerator RequestPolling()
    {
        using (var webRequest = UnityWebRequest.Get(url + "polling/" + id))
        {
            yield return webRequest.SendWebRequest();
            var newN = System.Convert.ToInt32(webRequest.downloadHandler.text);
            if (pollingN != newN)
            {
                StartCoroutine(RequestData());
                pollingN = newN;
                //Debug.Log("Polling " + pollingN);
            }
        }
    }

    IEnumerator RequestData()
    {
        using (var webRequest = UnityWebRequest.Get(url + "data/" + id))
        {
            yield return webRequest.SendWebRequest();
            var jsonString = webRequest.downloadHandler.text;
            var data = JsonConvert.DeserializeObject<Data>(jsonString);
            state = data.state;
            if (state == "wait")
            {
                Refresh();
            } else {
                hands = data.hands;
                lastTop = top; top = data.top;
                player = data.player;
                options = data.options;
                Refresh();
            }
            //Debug.Log("Refresh");
        }
    }

    IEnumerator RequestPlay(string data)
    {
        using (var webRequest = UnityWebRequest.Get(url + "play/" + id + "/" + data))
        {
            yield return webRequest.SendWebRequest();
        }
    }

    ////////////////////////////////////////////////
        
    public void OnData()
    {
        StartCoroutine(GetRequest(url + "data/" + id));
        //Refresh();
    }
    
    public void OnShowButtons()
    {
        //GameObject.Find("ButtonGo").GetComponent<Floating>()
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            var jsonString = webRequest.downloadHandler.text;
            //var data = JsonUtility.FromJson<Data>(text);
            var data = JsonConvert.DeserializeObject<Data>(jsonString);
            hands = data.hands;
            top = data.top;
            Refresh();
            /*
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }*/
        }
    }

    public void Shuffle()
    {
        var nums = RandomList(52).GetRange(0, 13);
        nums.Sort();
        //Debug.Log(System.String.Join(",", pool.ToArray()));
    }

    public List<int> RandomList(int n)
    {
        var items = new List<int>();
        for (var i = 0; i < n; i++)
        {
            items.Add(i);
        }
        for (int i = 0; i < n; i++)
        {
            var j = Random.Range(i, n);
            var temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
        return items;
    }
}
