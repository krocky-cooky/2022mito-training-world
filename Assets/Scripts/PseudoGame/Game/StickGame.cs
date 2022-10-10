using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace game
{
    public class StickGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject _slidingWalls;
        [SerializeField]
        private GameObject _stickObject;
        [SerializeField]
        private GameObject _rightController;
        [SerializeField]
        private float _stickXOffset = 0.2f;
        [SerializeField]
        private float _wallsYOffset = 0.25f;
        [SerializeField]
        private float _CDRatio = 1.3f;

        private float _speed = 0.05f;
        private Vector3 _stickStartPosition;

        private bool _gameStart = false;
        private Vector3 _initialWallPosition;
        private Master _gameMaster;
        

        // Start is called before the first frame update
        void Start()
        {
            _initialWallPosition = _slidingWalls.transform.position;
            _gameMaster = transform.parent.gameObject.GetComponent<Master>();
            
        }

        // Update is called once per frame
        void Update()
        {
            if(_gameStart)
            {
                move(_slidingWalls);
                
            }

            
        }

        private void move(GameObject obj)
        {
            Vector3 position = obj.transform.position;
            position.x += _speed;
            obj.transform.position = position;
        }

        public void GameStart()
        {
            StartCoroutine(GameCoroutine());
        }

        private IEnumerator GameCoroutine()
        {
            if(_gameStart)
            {
                Debug.Log("game has already started");
                yield break;
            }
            
            Vector3 wallPosition = _slidingWalls.transform.position;
            _stickStartPosition = _rightController.transform.position;
            wallPosition.y = _stickStartPosition.y - _wallsYOffset;
            _slidingWalls.transform.position = wallPosition;

            _gameStart = true;

            while(_slidingWalls.transform.position.x < 40.0f)
            {
                Vector3 stickPosition = _stickObject.transform.position;
                float tmp = stickPosition.z;
                stickPosition = _rightController.transform.position;
                stickPosition.x += _stickXOffset;
                if(_slidingWalls.transform.position.x >= 20.0f)
                {
                    float ydiff = stickPosition.y - _stickStartPosition.y;
                    stickPosition.y = _stickStartPosition.y + ydiff*_CDRatio;                    
                }
                stickPosition.z = tmp;
                _stickObject.transform.position = stickPosition;
                
                yield return new WaitForSeconds(0.1f);
            }

            _gameStart = false;
            _slidingWalls.transform.position = _initialWallPosition;
            _gameMaster.state = GameState.Idle;



        }
    }
}