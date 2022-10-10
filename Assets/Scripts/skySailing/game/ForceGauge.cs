/*
フォースゲージ(荷重測定器)をコントローラとして使うためのクラス
握力計や張力計が該当する
力を受け取り、0~1の間で位置情報outputPositionを返す. 例えば、0.5が返されたら、最大位置と最小位置の間を意味する
位置の計算方法は複数用意してあり、inspectorから選べる
PositionProportionalToForceモードは、位置が力と比例するもの。力が最大と最小の中間なら、出力位置は0.5
VelocityProportionalToForceモードは、速度が力と比例するもの。力だ最大なら、位置の変化(速度)が最大になる
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;

public class ForceGauge : MonoBehaviour
{
    // Start is called before the first frame update
    // 力の最大と最小
    public float maxForce = 0.0f;
    public float minForce = 0.0f;

    // 出力する位置の計算方法
    public enum PositionCalculationMethod
    {
        PositionProportionalToForce,
        VelocityProportionalToForce,
    }
    public PositionCalculationMethod positionCalculationMethod;

    // 出力する位置情報を0~1で表す。例えば、0.5が返されたら、最大位置と最小位置の間を意味する
    // 他スクリプトからアクセス可能にするが、inspectorに表示しない
    // [System.NonSerialized] public float outputPosition = 0.0f;
    public float outputPosition = 0.0f;


    // // 力に対する出力速度の比例定数
    // [SerializeField]
    // private float proportionalityConstantOfVelocityToForce = 0.0f;
    // 出力位置に関する最大速度
    [SerializeField]
    private float maxOutputVelocity = 0.0f;
    [SerializeField]
    private GripCommunicationInterface gripCommunicationInterface;

    // private webSocketClient _socketClient;
    
    // 現在の力の大きさ
    [System.NonSerialized] public float currentForce = 0.0f;

    // 出力位置の変化速度
    private float _outputVelocity = 0.0f;

    private bool ret;

    void Start(){
        maxForce = (float)Screen.width;
        minForce = 0.0f;
        // _socketClient = GameObject.FindWithTag("webSocketClient").GetComponent<webSocketClient>();
    }

    void Update(){
        currentForce = Input.mousePosition.x;

        Debug.Log("grip value is " + gripCommunicationInterface.getReceivedData().tension);
        if (gripCommunicationInterface.isConnected){
            currentForce = gripCommunicationInterface.getReceivedData().tension;
        }

        _outputVelocity = CalculateVelocityProportionalToForce();
        outputPosition = outputPosition + _outputVelocity * Time.deltaTime;

        if (positionCalculationMethod == PositionCalculationMethod.PositionProportionalToForce) {
            outputPosition = CalculatePositionProportionalToForce();
        }

         if (positionCalculationMethod == PositionCalculationMethod.VelocityProportionalToForce) {
            _outputVelocity = CalculateVelocityProportionalToForce();
            outputPosition = outputPosition + _outputVelocity * Time.deltaTime;
        }

        // 出力位置を0.0から1.0の範囲内に収める
        outputPosition = Mathf.Clamp01(outputPosition);

        Debug.Log("outputPosition is " + outputPosition.ToString());


        // マシンのハンドル等のストロークポジション登録
        if(Input.GetMouseButtonDown(2))
        {
            minForce = currentForce;
            maxForce = currentForce;
            Debug.Log("Input.GetMouseButtonDown(2)");
        }
        if(Input.GetMouseButton(2))
        {
            Debug.Log("Input.GetMouseButton(2)");
            if (minForce > currentForce){
                minForce = currentForce;
            }
            if (maxForce < currentForce){
                maxForce = currentForce;
            }
        }
    }

    float CalculatePositionProportionalToForce(){
        return (currentForce - minForce) / (maxForce - minForce);
    }

    float CalculateVelocityProportionalToForce(){
        float middleForce = (maxForce - minForce) / 2.0f;
        return maxOutputVelocity * (currentForce - middleForce) / (maxForce - middleForce);
    }
}