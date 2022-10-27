using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace pseudogame.game
{
    public enum WallHeight
    {
        height30,
        height40
    }

    public class StickGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject _slidingWalls;
        [SerializeField]
        private GameObject _wall30;
        [SerializeField]
        private GameObject _wall40;
        [SerializeField]
        private GameObject _endObject;
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
        [SerializeField]
        private float _wallStartX = -5.0f;
        [SerializeField]
        private float _wallPosZ = -0.5f;
        [SerializeField]
        private float _spaceBetweenWall = 7.5f;
        [SerializeField]
        private float _trainingTorque = 2.0f;
        [SerializeField]
        private int _gameOverCollisionCount = 5;
        [SerializeField]
        private int _wallSpeed = 0.05f;
        [SerializeField]
        private List<WallHeight> _wallHeight = new List<WallHeight>()
        {
            WallHeight.height30,
            WallHeight.height30,
            WallHeight.height30,
            WallHeight.height40,
            WallHeight.height40,
            WallHeight.height40
        };
        [SerializeField]
        private AudioClip _collisionSound;
        [SerializeField]
        private AudioClip _getPointSound;

        private Vector3 _stickStartPosition;
        private bool _gameStart = false;
        private Vector3 _initialWallPosition;
        private Master _gameMaster;
        private AudioSource _collisionAudioSource;

        public int _collisionCount = 0;
        public bool _applyCDRatio = false;
        public bool _gameEnd = false;
        public int _liftCount = 0;
        

        // Start is called before the first frame update
        void Start()
        {
            _initialWallPosition = _slidingWalls.transform.position;
            _gameMaster = transform.parent.gameObject.GetComponent<Master>();
            float objX = _wallStartX;
            for(int i = 0;i < _wallHeight.Count; ++i)
            {
                GameObject obj = new GameObject();
                
                if(_wallHeight[i] == WallHeight.height30)obj = Instantiate(_wall30);
                else if(_wallHeight[i] == WallHeight.height40)obj = Instantiate(_wall40);
                obj.transform.parent = _slidingWalls.transform;
                Vector3 pos = new Vector3(objX,0,_wallPosZ);
                obj.transform.position = pos;
                objX -= _spaceBetweenWall;
            }
            GameObject endObject = Instantiate(_endObject);
            endObject.transform.parent = _slidingWalls.transform;
            Vector3 posend = new Vector3(objX,0,_wallPosZ);
            endObject.transform.position = posend;
            _collisionAudioSource = GetComponent<AudioSource>();

            
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
            position.x += _wallSpeed;
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
            _collisionCount = 0;
            _liftCount = 0;
            _gameMaster.setTorque(_trainingTorque);
            
            Vector3 wallPosition = _slidingWalls.transform.position;
            _stickStartPosition = _rightController.transform.position;
            wallPosition.y = _stickStartPosition.y - _wallsYOffset;
            _slidingWalls.transform.position = wallPosition;

            _gameStart = true;

            while(!_gameEnd)
            {
                Vector3 stickPosition = _stickObject.transform.position;
                float tmp = stickPosition.z;
                stickPosition = _rightController.transform.position;
                stickPosition.x += _stickXOffset;
                if(_applyCDRatio)
                {
                    float ydiff = stickPosition.y - _stickStartPosition.y;
                    stickPosition.y = _stickStartPosition.y + ydiff*_CDRatio;                    
                }
                stickPosition.z = tmp;
                _stickObject.transform.position = stickPosition;
                if(_collisionCount >= _gameOverCollisionCount)
                {
                    break;
                }

    
                
                yield return new WaitForSeconds(0.1f);
            }

            _gameMaster.restore();

            _gameStart = false;
            _slidingWalls.transform.position = _initialWallPosition;
            _gameMaster.state = GameState.Idle;



        }

        public void CollisionEnterEvent()
        {
            _collisionAudioSource.clip = _collisionSound;
            _collisionAudioSource.Play();
            OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.LTouch);
        }

        public void CollisionExitEvent()
        {
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.LTouch);
            ++_collisionCount;
        }

        public void GetPoint()
        {
            _collisionAudioSource.clip = _getPointSound;
            _collisionAudioSource.Play();
            _liftCount++;
        }
        
    }
}