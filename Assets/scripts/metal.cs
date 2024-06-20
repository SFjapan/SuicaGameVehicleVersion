using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class metal : MonoBehaviour
{
    // 初期段階はドラゴンクエストのメタル系のスライムで開発していたのですがUnityRoomに投稿するにあたり著作権などの問題で車両に変更しました
    public int number;
    [SerializeField] public Crane crane;
    public bool on;
    public int isEvo = 0;
    public int score;
    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("collision");
        on = true;
        if(!other.gameObject.GetComponent<metal>())return;
        if(!crane)return;
        if(!other.gameObject.GetComponent<metal>().crane)return;
        if(other.gameObject.GetComponent<metal>().number == number){
            crane.Evolution(gameObject.transform,number,score);
            Destroy(gameObject);
        }
    }

 
}
