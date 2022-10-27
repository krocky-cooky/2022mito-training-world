using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace communication
{
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
            spdLimit = 20.0f;
        }

        private string DEFAULT_TARGET = "trq";
        public string target;
        public float trq;
        public float spd;
        public float spdLimit;
        public float trqLimit;

        public void setTorque(float torque, float inputSpdLimit = 20.0f)
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
            tension = -1.0f;
        }
        
        public float tension;
        
    }
}