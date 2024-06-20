using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    //車両の処理
    public int number; //車両番号
    [SerializeField] public Crane crane; //クレーン
    public int score; //この車両のスコア
    public Transform overline = null; //満杯か
    void FixedUpdate()
    {
        //満杯の線がありその線より上にいるならゲーム終了
        if(overline && transform.position.y > overline.position.y){
            crane.GameOver();
        }    
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        //接触判定
        Debug.Log("collision");
        if(other.gameObject.tag == "box"){
            Debug.Log("hit");
            overline = crane.BorderLine;
        }
        //車両じゃなかったらこれ以上処理しない
        if(!other.gameObject.GetComponent<Vehicle>())return;
        //同じ番号の車両なら進化する

        if(other.gameObject.GetComponent<Vehicle>().number == number){
            crane.Evolution(gameObject.transform,number,score);
            Destroy(gameObject);
        }
    }
}
