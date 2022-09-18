using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace skySailing.game
{
    
    public class FittnessDevice : MonoBehaviour
    {
        // ハンドル等の最大位置と最小位置
        public float maxPosition = 0.0f;
        public float minPosition = 0.0f;
        public float currentAbsPosition = 0.0f;
        // ハンドル等がストローク全体の中で相対的にどの位置にあるかを返す
        // 例えば、ハンドルの最低位置が10cm、最高位置が110cmで、現在地が20cmなら、ストローク全体100cmの中で下から10cmのところにあるので、0.1である
        public float currentRelativePosition = 0.0f;

        public FittnessDevice(float inputMaxPosition, float inputMinPosition)
        {
            maxPosition = inputMaxPosition;
            minPosition = inputMinPosition;
            currentAbsPosition = 0.0f;
            // ハンドル等がストローク全体の中で相対的にどの位置にあるかを返す
            // 例えば、ハンドルの最低位置が10cm、最高位置が110cmで、現在地が20cmなら、ストローク全体100cmの中で下から10cmのところにあるので、0.1である
            currentRelativePosition = 0.0f;
            }

        // デバイスのハンドル等の位置を受け取って、その位置がハンドル等のストローク全体の中で相対的にどの位置にあるかを返す
        // 例えば、ハンドルの最低位置が10cm、最高位置が110cmで、現在地の20cmを受け取ったら、ストローク全体100cmの中で下から10cmのところにあるので、0.1を返す
        public float getCurrentRelativePosition(float rawInputData){
            currentAbsPosition = rawInputData;
            currentRelativePosition = (currentAbsPosition - minPosition) / (maxPosition - minPosition);
            return currentRelativePosition;
        }

       
    }
}