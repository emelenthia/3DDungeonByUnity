using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraMover : MonoBehaviour
{
    // WASD：前後左右の移動
    // QE：上昇・降下
    // 右ドラッグ：カメラの回転
    // 左ドラッグ：前後左右の移動
    // スペース：カメラ操作の有効・無効の切り替え
    // P：回転を実行時の状態に初期化する

    //カメラの移動量
    [SerializeField, Range(0.1f, 10.0f)]
    private float _positionStep = 2.0f;

    //マウス感度
    [SerializeField, Range(30.0f, 150.0f)]
    private float _mouseSensitive = 90.0f;

    //カメラ操作の有効無効
    private bool _cameraMoveActive = true;
    //カメラのtransform  
    private Transform _camTransform;
    //マウスの始点 
    private Vector3 _startMousePos;
    //カメラ回転の始点情報
    private Vector3 _presentCamRotation;
    private Vector3 _presentCamPos;
    //初期状態 Rotation
    private Quaternion _initialCamRotation;
    //UIメッセージの表示
    private bool _uiMessageActiv;
    private static Timer myTimer;
    private static bool isMoving = false; 
    private static int moveTime = 0;
    private static Vector3 tempPos;
    private const int MOVE_FRAME = 30;
    private static float tempTransformX = 0.0f;
    private static float tempTransformY = 0.0f;
    private Direction direction = Direction.Forward;

    enum Direction {
        Forward = 0,
        Right = 1,
        Back = 2,
        Left = 3
    }

    void Start ()
    {
        _camTransform = this.gameObject.transform;

        //初期回転の保存
        _initialCamRotation = this.gameObject.transform.rotation;
    }

    void Update () {

        CamControlIsActive(); //カメラ操作の有効無効

        if (_cameraMoveActive)
        {
            ResetCameraRotation(); //回転角度のみリセット
            CameraRotationMouseControl(); //カメラの回転 マウス
            CameraSlideMouseControl(); //カメラの縦横移動 マウス
            CameraPositionKeyControl(); //カメラのローカル移動 キー
        }
    }

    //カメラ操作の有効無効
    public void CamControlIsActive()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _cameraMoveActive = !_cameraMoveActive;

            if (_uiMessageActiv == false)
            {
                StartCoroutine(DisplayUiMessage());
            }            
            Debug.Log("CamControl : " + _cameraMoveActive);
        }
    }

    //回転を初期状態にする
    private void ResetCameraRotation()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            this.gameObject.transform.rotation = _initialCamRotation;
            Debug.Log("Cam Rotate : " + _initialCamRotation.ToString());    
        }
    }

    //カメラの回転 マウス
    private void CameraRotationMouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startMousePos = Input.mousePosition;
            _presentCamRotation.x = _camTransform.transform.eulerAngles.x;
            _presentCamRotation.y = _camTransform.transform.eulerAngles.y;
        }

        if (Input.GetMouseButton(0))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (_startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (_startMousePos.y - Input.mousePosition.y) / Screen.height;

            //回転開始角度 ＋ マウスの変化量 * マウス感度
            float eulerX = _presentCamRotation.x + y * _mouseSensitive;
            float eulerY = _presentCamRotation.y + x * _mouseSensitive;

            _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
        }
    }

    //カメラの移動 マウス
    private void CameraSlideMouseControl()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _startMousePos = Input.mousePosition;
            _presentCamPos = _camTransform.position;
        }

        if (Input.GetMouseButton(1))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (_startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (_startMousePos.y - Input.mousePosition.y) / Screen.height;

            x = x * _positionStep;
            y = y * _positionStep;

            Vector3 velocity = _camTransform.rotation * new Vector3(x, y, 0);
            velocity = velocity + _presentCamPos;
            _camTransform.position = velocity;
        }
    }

    //カメラのローカル移動 キー
    private void CameraPositionKeyControl()
    {
        if (isMoving) {
            return;
        }

        Vector3 campos = _camTransform.position;
        tempPos = campos;
        tempTransformX = _camTransform.transform.eulerAngles.x;
        tempTransformY = _camTransform.transform.eulerAngles.y;

        if (Input.GetKey(KeyCode.D)) { 
            isMoving = true;
            InvokeRepeating("rotateRight", 0.0f, (1.0f / MOVE_FRAME));
        }
        if (Input.GetKey(KeyCode.A)) { 
            isMoving = true;
            InvokeRepeating("rotateLeft", 0.0f, (1.0f / MOVE_FRAME));
        }
        
        if (Input.GetKey(KeyCode.W)) { 
            isMoving = true;
            switch (direction) {
                case Direction.Forward:
                    InvokeRepeating("moveForward", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Right:
                    InvokeRepeating("moveRight", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Back:
                    InvokeRepeating("moveBack", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Left:
                    InvokeRepeating("moveLeft", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                default:
                    break;
            }
        }
        if (Input.GetKey(KeyCode.S)) { 
            isMoving = true;
            switch (direction) {
                case Direction.Forward:
                    InvokeRepeating("moveBack", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Right:
                    InvokeRepeating("moveLeft", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Back:
                    InvokeRepeating("moveForward", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                case Direction.Left:
                    InvokeRepeating("moveright", 0.0f, (1.0f / MOVE_FRAME));
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKey(KeyCode.E)) { campos += _camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.Q)) { campos -= _camTransform.up * Time.deltaTime * _positionStep; }

        _camTransform.position = campos;
    }

    private void moveForward() {
        Vector3 campos = _camTransform.position;
        campos = new Vector3(campos.x + (1.0f / MOVE_FRAME), campos.y, campos.z);
        moveTime++;
        Debug.Log("ee");
        if (moveTime > MOVE_FRAME) {
            Debug.Log("ff");
            campos = new Vector3(tempPos.x + 1, campos.y, campos.z);
            isMoving = false;
            moveTime = 0;
            CancelInvoke();
        }
        _camTransform.position = campos;
    }

    private void moveRight() {
        Vector3 campos = _camTransform.position;
        campos = new Vector3(campos.x, campos.y, campos.z - (1.0f / MOVE_FRAME));
        moveTime++;
        Debug.Log("ee");
        if (moveTime > MOVE_FRAME) {
            Debug.Log("ff");
            campos = new Vector3(tempPos.x, campos.y, campos.z - 1);
            isMoving = false;
            moveTime = 0;
            CancelInvoke();
        }
        _camTransform.position = campos;
    }
    private void moveLeft() {
        Vector3 campos = _camTransform.position;
        campos = new Vector3(campos.x, campos.y, campos.z + (1.0f / MOVE_FRAME));
        moveTime++;
        Debug.Log("ee");
        if (moveTime > MOVE_FRAME) {
            Debug.Log("ff");
            campos = new Vector3(tempPos.x, campos.y, campos.z + 1);
            isMoving = false;
            moveTime = 0;
            CancelInvoke();
        }
        _camTransform.position = campos;
    }
    private void moveBack() {
        Vector3 campos = _camTransform.position;
        campos = new Vector3(campos.x - (1.0f / MOVE_FRAME), campos.y, campos.z);
        moveTime++;
        Debug.Log("ee");
        if (moveTime > MOVE_FRAME) {
            Debug.Log("ff");
            campos = new Vector3(tempPos.x - 1, campos.y, campos.z);
            isMoving = false;
            moveTime = 0;
            CancelInvoke();
        }
        _camTransform.position = campos;
    }

    private void rotateRight() {
        float eulerX = _camTransform.transform.eulerAngles.x;
        float eulerY = _camTransform.transform.eulerAngles.y + (90.0f / MOVE_FRAME);
        Debug.Log(tempTransformY);
        if (moveTime < MOVE_FRAME) {
            moveTime++;

            switch (direction)
            {
                case Direction.Forward:
                    _camTransform.position = new Vector3(_camTransform.position.x + (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z + (0.5f / MOVE_FRAME));
                    break;
                case Direction.Right:
                    _camTransform.position = new Vector3(_camTransform.position.x + (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z - (0.5f / MOVE_FRAME));
                    break;
                case Direction.Back:
                    _camTransform.position = new Vector3(_camTransform.position.x - (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z - (0.5f / MOVE_FRAME));
                    break;
                case Direction.Left:
                    _camTransform.position = new Vector3(_camTransform.position.x - (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z + (0.5f / MOVE_FRAME));
                    break;
                default:
                    break;
            }
            _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
            return;
        }
        eulerY = tempTransformY + 90;
            switch (direction)
            {
                case Direction.Forward:
                    direction = Direction.Right;
                    _camTransform.position = new Vector3(tempPos.x + 0.5f, tempPos.y, tempPos.z + 0.5f);
                    break;
                case Direction.Right:
                    direction = Direction.Back;
                    _camTransform.position = new Vector3(tempPos.x + 0.5f, tempPos.y, tempPos.z - 0.5f);
                    break;
                case Direction.Back:
                    direction = Direction.Left;
                    _camTransform.position = new Vector3(tempPos.x - 0.5f, tempPos.y, tempPos.z - 0.5f);
                    break;
                case Direction.Left:
                    direction = Direction.Forward;
                    _camTransform.position = new Vector3(tempPos.x - 0.5f, tempPos.y, tempPos.z + 0.5f);
                    break;
                default:
                    break;
            }
        _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);

        isMoving = false;
        moveTime = 0;
        CancelInvoke();
    }
    private void rotateLeft() {
        float eulerX = _camTransform.transform.eulerAngles.x;
        float eulerY = _camTransform.transform.eulerAngles.y - (90.0f / MOVE_FRAME);
        if (moveTime < MOVE_FRAME) {
            moveTime++;

            switch (direction)
            {
                case Direction.Forward:
                    _camTransform.position = new Vector3(_camTransform.position.x + (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z - (0.5f / MOVE_FRAME));
                    break;
                case Direction.Right:
                    _camTransform.position = new Vector3(_camTransform.position.x - (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z - (0.5f / MOVE_FRAME));
                    break;
                case Direction.Back:
                    _camTransform.position = new Vector3(_camTransform.position.x - (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z + (0.5f / MOVE_FRAME));
                    break;
                case Direction.Left:
                    _camTransform.position = new Vector3(_camTransform.position.x + (0.5f / MOVE_FRAME), tempPos.y, _camTransform.position.z + (0.5f / MOVE_FRAME));
                    break;
                default:
                    break;
            }
            _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
            return;
        }
        Debug.Log(tempTransformY);
        eulerY = tempTransformY - 90;
            switch (direction)
            {
                case Direction.Forward:
                    direction = Direction.Left;
                    _camTransform.position = new Vector3(tempPos.x + 0.5f, tempPos.y, tempPos.z - 0.5f);
                    break;
                case Direction.Right:
                    direction = Direction.Forward;
                    _camTransform.position = new Vector3(tempPos.x - 0.5f, tempPos.y, tempPos.z - 0.5f);
                    break;
                case Direction.Back:
                    direction = Direction.Right;
                    _camTransform.position = new Vector3(tempPos.x - 0.5f, tempPos.y, tempPos.z + 0.5f);
                    break;
                case Direction.Left:
                    direction = Direction.Back;
                    _camTransform.position = new Vector3(tempPos.x + 0.5f, tempPos.y, tempPos.z + 0.5f);
                    break;
                default:
                    break;
            }
        _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);

        isMoving = false;
        moveTime = 0;
        CancelInvoke();
    }


    //UIメッセージの表示
    private IEnumerator DisplayUiMessage()
    {
        _uiMessageActiv = true;
        float time = 0;
        while (time < 2)
        {
            time = time + Time.deltaTime;
            yield return null;
        }
        _uiMessageActiv = false;
    }

    void OnGUI()
    {
        if (_uiMessageActiv == false) { return; }
        GUI.color = Color.black;
        if (_cameraMoveActive == true)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 30, 100, 20), "カメラ操作 有効");
        }

        if (_cameraMoveActive == false)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 30, 100, 20), "カメラ操作 無効");
        }
    }

}
