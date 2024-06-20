using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using unityroom.Api;

public class Crane : MonoBehaviour
{
    [SerializeField] GameObject[] vehicles; //車両一覧
    [SerializeField] Transform spwanPos; //初期位置
    [SerializeField] Text GameOverText; //ゲーム終了時のテキスト
    [SerializeField] Button GameOverButton; //再開するためのボタン
    int currentNumber; //現在の車両の番号
    Rigidbody2D rig; //rig
    public float speed = 10f; //クレーンの移動速度
    float timer = 0; //次の車両までの時間
    float waitTime = 3; //次の車両召喚までの時間
    bool evoluting; //進化中か
    public bool holding; //保持中か
    public Transform BorderLine; //満杯判定の場所
    [SerializeField] Text score; //スコアのテキスト
    [SerializeField] AudioClip sound; //bgm
    [SerializeField] AudioSource levelup; //進化時の効果音

    [SerializeField]GameObject currentVehicle; //現在の車両
    public bool Game = true; //ゲーム中か
    int current_score = 0; //スコア
    void Start()
    {
        //最初の車両召喚
        SpwanVehicle();
        holding = true;
        rig = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        //ゲーム中なら処理する
        if (Game)
        {
            //親子関係にすると大きさのバグがあったので召喚場所をクレーンのちょっとしたに追跡するようにしている
            spwanPos.transform.position = new Vector2(transform.position.x, spwanPos.transform.position.y);
            
            //クレーンの移動処理
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(transform.right * -speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(transform.right * speed * Time.deltaTime);
            }
            else
            {
                rig.AddForce(new Vector2(0, 0));
            }

            //スペースを押したら落下
            if (Input.GetKeyDown(KeyCode.Space) && holding )
            {
                Drop();
            }

            //車両の追跡
            if (currentVehicle)
            {
                currentVehicle.transform.localPosition = spwanPos.transform.position;
            }
            
        }
        else
        {
            GameOver();
        }

    }

    public void Drop()
    {
        holding = false;
        Debug.Log("pressed");
        if (currentVehicle)
        {
            //落下させるまで移動してほしくないのでここでRIG追加
            
            currentVehicle.AddComponent<Rigidbody2D>();
            currentVehicle.transform.SetParent(transform.parent);
            currentVehicle.AddComponent<Vehicle>();
            currentVehicle.GetComponent<Vehicle>().number = currentNumber;
            currentVehicle.GetComponent<Vehicle>().score = (currentNumber + 1) * 1500;
            currentVehicle.GetComponent<Vehicle>().crane = this;
            
            //車両によってコライダーの形が変わるので分岐
            //落下させてから当たり判定をつけないと保持しているときに邪魔になる(満杯付近の時)
            if (currentVehicle.GetComponent<BoxCollider2D>())
            {
                currentVehicle.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (currentVehicle.GetComponent<CircleCollider2D>())
            {
                currentVehicle.GetComponent<CircleCollider2D>().enabled = true;
            }
            else if (currentVehicle.GetComponent<CapsuleCollider2D>())
            {
                currentVehicle.GetComponent<CapsuleCollider2D>().enabled = true;
            }
            currentVehicle = null;
        }
        //２秒後に次の車両召喚
        Invoke("SpwanVehicle",2f);

    }


    public void MouseDrop(){
        //落とす
        if ( holding)
        {
            Drop();
        }
    }
    public void SpwanVehicle()
    {
        //新規車両召喚
        if (!holding)
        {
            holding = true;
            //ランダムでとってくる
            currentNumber = Random.Range(0, 4);
            currentVehicle = Instantiate(vehicles[currentNumber], spwanPos.parent);
            //召喚したときは当たり判定が邪魔なので消す
            if (currentVehicle.GetComponent<BoxCollider2D>())
            {
                currentVehicle.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if (currentVehicle.GetComponent<CircleCollider2D>())
            {
                currentVehicle.GetComponent<CircleCollider2D>().enabled = false;
            }
            else if (currentVehicle.GetComponent<CapsuleCollider2D>())
            {
                currentVehicle.GetComponent<CapsuleCollider2D>().enabled = false;
            }
            //クレーンの下にポジションを変更
            currentVehicle.transform.localPosition = spwanPos.transform.position;

        }

    }

    public void Evolution(Transform pos, int number, int score)
    {
        //進化の処理
        //最大値なら処理しない
        if (number + 1 >= vehicles.Length) return;
        //音出す
        levelup.PlayOneShot(sound);

        //進化中ではなかったら進化する(衝突したときに２回呼ばれる対策)
        if (!evoluting)
        {
            evoluting = true;
            GameObject evo = Instantiate(vehicles[number + 1], pos.parent);
            evo.transform.position = pos.position;
            evo.transform.SetParent(transform.parent);
            evo.AddComponent<Rigidbody2D>();
            evo.AddComponent<Vehicle>();
            evo.GetComponent<Vehicle>().number = number + 1;
            evo.GetComponent<Vehicle>().overline = BorderLine;
            evo.GetComponent<Vehicle>().score = (number + 1) * 1500;
            evo.GetComponent<Vehicle>().crane = this;
            //進化した際にスコア追加
            setScore(score);
            //ちょっと待ってから進化中をfalseにする
            Invoke("evoevo", 0.00001f);
        }
    }

    void evoevo()
    {
        //進化中をfalseにする
        evoluting = false;
    }

    public void setScore(int n)
    {
        //スコア更新
        current_score += n;
        score.text = "SCORE:" + current_score;
    }

    public void GameOver()
    {
        //ゲーム終了処理
        Game = false;
        //再開に必要なテキストなど表示
        GameOverButton.enabled = true;
        GameOverButton.image.enabled = true;
        GameOverButton.GetComponentInChildren<Text>().enabled = true;
        GameOverText.enabled = true;
        //残っている車両を消す
        foreach (var vehicle in GameObject.FindGameObjectsWithTag("Vehicle"))
        {
            Destroy(vehicle.gameObject);
        }
        
    }

    public void Restart()
    {
        //再開処理
        if (!Game)
        {
            GameOverButton.enabled = false;
            GameOverButton.image.enabled = false;
            GameOverButton.GetComponentInChildren<Text>().enabled = false;
            GameOverText.enabled = false;
            Game = true;
            UnityroomApiClient.Instance.SendScore(1, current_score, ScoreboardWriteMode.HighScoreDesc);
            current_score = 0;
            score.text = "SCORE:" + current_score;
            holding = false;
            SpwanVehicle();
        }

    }
}
