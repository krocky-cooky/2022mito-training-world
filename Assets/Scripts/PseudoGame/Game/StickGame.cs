using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private float _wallSpeed = 0.05f;
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
        [SerializeField]
        private GameObject _dataMonitor;
        [SerializeField]
        private Text _restLifeText;
        [SerializeField]
        private float _dataMonitorEndY = -0.35f;

        private Vector3 _stickStartPosition;
        private bool _gameStart = false;
        private Vector3 _initialWallPosition;
        private Master _gameMaster;
        private AudioSource _collisionAudioSource;
        private bool _collisionActive = false;

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
            _restLifeText.text = $"残りライフ\n{_gameOverCollisionCount}";

            Vector3 monitorPos = _dataMonitor.transform.position;
            monitorPos.y = -1.9f;
            
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
            if(_dataMonitor.transform.position.y < _dataMonitorEndY)
            {
                Vector3 monitorPos = _dataMonitor.transform.position;
                monitorPos.y += 0.03f;
                _dataMonitor.transform.position = monitorPos;
            }
            
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
            _restLifeText.text = $"残りライフ\n{_gameOverCollisionCount - _collisionCount}";
            
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
            if(_collisionActive)
            {
                _collisionAudioSource.clip = _collisionSound;
                _collisionAudioSource.Play();
                OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.LTouch);
                OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.RTouch);
                ++_collisionCount;
                _restLifeText.text = $"残りライフ\n{_gameOverCollisionCount - _collisionCount}";
            }
            
        }

        public void CollisionExitEvent()
        {

            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch);
            StartCoroutine(CollisionDeactivateColloutine());
            
        }

        public void GetPoint()
        {
            _collisionAudioSource.clip = _getPointSound;
            _collisionAudioSource.Play();
            _liftCount++;
        }

        private IEnumerator CollisionDeactivateColloutine()
        {
            _collisionActive = false;
            yield return new WaitForSeconds(0.8f);
            _collisionActive = true;
        }
        
    }
}