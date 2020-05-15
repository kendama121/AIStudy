using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class RollerAgent : Agent
{
    Rigidbody _rBody;
    public float _speed = 10;//自分のスピード
    public Transform _target;
    
    
    
    void Start()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    
    //自分の行動の仕方や報酬の与え方を決める処理（毎フレーム呼ばれる）
    public override void OnActionReceived(float[] vectorAction)
    {
        //vectorActionは-1～1までのfloat値が最初はランダムに与えられる
        //徐々に学習が進むとAIが考えた値が与えられる
        
        //行動
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        _rBody.AddForce(controlSignal * _speed);

        
        
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
            _target.localPosition);
        
        //リセットと、報酬を与える場所
        if (distanceToTarget < 1.42f)//ターゲットに当たった時
        {
            SetReward(1.0f);
            EndEpisode();//リセット
        }

        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
        
        
        //スピードを出したいとき（最短距離で移動してもらいたいとき）
        //AddReward(-0.001f);
        

    }
    

    //リセットされるとき
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0)
        {
            // If the Agent fell, zero its momentum
            this._rBody.angularVelocity = Vector3.zero;
            this._rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        _target.localPosition = new Vector3(Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4);
    }
    
    
    //使う環境の数値を全て教えてあげる処理(AIの目）
    //今回は
    //　・ターゲットのポジション
    //　・自分のポジション
    //　・自分の速度
    //を教えてあげる
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(_target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(_rBody.velocity.x);
        sensor.AddObservation(_rBody.velocity.z);
    }
    
    
    

    //実際に動くか手動で動かす処理（Debug用）
    public override float[] Heuristic(){
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        
        return action;

    }


}