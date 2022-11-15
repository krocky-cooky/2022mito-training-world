using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace communication
{

    public enum  TrainingDeviceType
    {
        TrainingDevice,
        ForceGauge,
    }

    public enum  TsunahikiStateType
    {
        SetUp,
        GetReady,
        Battle,
        GameSet,
    }

    public class ReceivingDataFormat
    {
        public string target;
        public float trq;
        public float spd;
        public float pos;
        public float integrationAngle;
        public int timestamp;
    }

    public class SendingDataFormat
    {
        
        public SendingDataFormat()
        {
            target = "trq";
            trq = -0.1f;
            spd = -0.1f;
            trqLimit = 6.0f;
            spdLimit = 6.0f;
        }

        private string DEFAULT_TARGET = "trq";
        public string target;
        public float trq;
        public float spd;
        public float spdLimit;
        public float trqLimit;

        public void setTorque(float torque, float inputSpdLimit = 10.0f)
        {
            target = "trq";
            trq = torque;
            spdLimit = inputSpdLimit;
        }

        public void setSpeed(float speed, float trqLimit = 2.0f)
        {
            target = "spd";
            spd = speed;
        }
    }

    public class SwitchMotorFormat
    {
        public SwitchMotorFormat(int _motor)
        {
            motor = _motor;
        }

        public int motor;
    }

    public class ReceivingGripDataFormat 
    {
        public ReceivingGripDataFormat()
        {
            force = -1.0f;
        }
        
        public float force;
        
    }

    public class ForceGaugeDataFormat
    {
        public ForceGaugeDataFormat()
        {
            force = 1.0f;
        }
        public float force;
    }

    // // AWSを介したリモートWebSocket通信用のデータフォーマット
    // public class ReceivingRemoteDataFormat
    // {
    //     public ReceivingRemoteDataFormat()
    //     {
    //         action = "sendmessage";
    //         message = "0.0";
    //     }
    //     public string action;
    //     public string message;
    // }



    //リモート綱引き用データ交換フォーマット
    public class RemoteTsunahikiDataFormat
    {
        public RemoteTsunahikiDataFormat () 
        {
            normalizedData = 0.0f;
            deviceInterface = (int)TrainingDeviceType.TrainingDevice;
            stateId = 0;
            superiority = (int)TrainingDeviceType.TrainingDevice;
        }
        public float normalizedData;

        //enum DeviceTypeを利用する ex) (int)DeviceType.TrainingDevice
        public int deviceInterface;

        public int stateId;

        //enum DeviceTypeを利用する ex) (int)DeviceType.TrainingDevice
        public int superiority;
    }
}