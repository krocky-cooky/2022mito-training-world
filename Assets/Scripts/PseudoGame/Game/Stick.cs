using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace pseudogame.game
{
    public class Stick : MonoBehaviour
    {
        [SerializeField]
        private StickGame _stickGame;


        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "PseudoTrigger")
            {
                _stickGame._applyCDRatio = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "GameEnd")
            {
                _stickGame._gameEnd = true;
            }
            if(other.gameObject.tag == "WallCollision")
            {
                _stickGame.CollisionEnterEvent();
            }
            if(other.gameObject.tag == "GetPoint")
            {
                _stickGame.GetPoint();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.tag == "PseudoTrigger")
            {
                _stickGame._applyCDRatio = false;
            }
            if(other.gameObject.tag == "GameEnd")
            {
                _stickGame._gameEnd = false;
            }
            if(other.gameObject.tag == "WallCollision")
            {
                _stickGame.CollisionExitEvent();
            }
        }

    }
}