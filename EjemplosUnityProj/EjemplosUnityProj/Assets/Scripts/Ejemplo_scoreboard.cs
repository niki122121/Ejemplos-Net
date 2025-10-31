using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ejemplo_scoreboard : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] int score;
    [SerializeField] int port = 5140;
    string url = "http://localhost";

    public void addScore()
    {
        StartCoroutine(httpCor("addScore/" + name + "/" + score + "/"));
    }
    public void getScores()
    {
        StartCoroutine(httpCor("getScores/"));
    }
    public void getScoresPretty()
    {
        StartCoroutine(httpCor("getScoresPretty/"));
    }

    IEnumerator httpCor(string header)
    {
        UnityWebRequest www = new UnityWebRequest(url + ":" + port + "/" + header, "GET", new DownloadHandlerBuffer(), new UploadHandlerRaw(new byte[0]));

        yield return www.SendWebRequest();
        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text.ToString());
        }
        else
        {
            Debug.Log("HTTP ERROR:");
            Debug.Log(www.error);
        }
        www.Dispose();
    }
}
