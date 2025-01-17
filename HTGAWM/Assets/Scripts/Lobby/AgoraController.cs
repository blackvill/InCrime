﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameInfo;

public class AgoraController : MonoBehaviour
{
    private static AgoraController agoraController = null; // 싱글톤

    public CamObject camObject;
    public Button toggleButton;
    static WebAgoraUnityVideo app;      // 비디오용

    private CanvasGroup camCanvasGroup;

    void Awake()
    {
        // 싱글톤
        if (agoraController == null)
        {
            agoraController = this;
            
            DontDestroyOnLoad(this.gameObject);
            if (ReferenceEquals(app, null) && !ReferenceEquals(camObject, null))
            {
                app = WebAgoraUnityVideo.GetTestHelloUnityVideoInstance();
                Debug.Log("캠 인스턴스 생성");
                app.loadEngine("87d658834c824bc9afbe668d75dd4c0b");
                app.SetCamObject(camObject); // create app
            }
            camCanvasGroup = camObject.gameObject.GetComponent<CanvasGroup>();

            SceneManager.sceneLoaded += OnSceneLoadedAgora;
        }
        else
        {
            Debug.Log("AgoraController 삭제");
            Destroy(this.gameObject);
        }
    }

    public static AgoraController GetAgoraControllerInstance()
    {
        return agoraController;
    }

    public bool isAppNull()
    {
        return app == null;
    }

    // Start is called before the first frame update
    void Start()
    { 
        
    }

    void CamShow()
    {
        //camCanvasGroup.alpha = 1;
        //camCanvasGroup.interactable = true;
        //camCanvasGroup.blocksRaycasts = true;
        camObject.gameObject.SetActive(true);
    }
    void CamHide()
    {
        //camCanvasGroup.alpha = 0;
        //camCanvasGroup.interactable = false;
        //camCanvasGroup.blocksRaycasts = false;

        camObject.gameObject.SetActive(false);
    }

    private void OnSceneLoadedAgora(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + "아고라로 접속합니다.");
        if(app != null)
        {
            if (GameRoomInfo.agoraVideoScene.Contains(scene.name))
            {
                Debug.Log(scene.name + "agora 비디오 연결");
                JoinAgoraRoom(GameRoomInfo.roomNo + scene.name, true, GameRoomInfo.userNoByRoom);
                CamShow();
                toggleButton.gameObject.SetActive(true);
            }
            else if (GameRoomInfo.agoraKeepScene.Contains(scene.name))
            {
                Debug.Log(scene.name + "agora 상태 유지");
                //빈블록: 그냥 아고라의 이전 상태를 유지하기 위함임
            }
            else
            {
                Debug.Log(scene.name + "agora 나가기");
                app.leave();
                CamHide();
                toggleButton.gameObject.SetActive(false);
            }
        }
        
    }

    public void SetVolumeByUserId(int userNo, float volume)
    {
        if(userNo != 0 && app != null)
            app.SetUserVolumeByUserId((uint)userNo, volume);
    }

    public void SetMyVolume(float volume)
    {
        if(app != null)
        {
            app.SetMyVolume(volume);
        }
    }

    public void JoinAgoraRoom(string joinRoomID, bool audioFlag, int userNo)
    {
        Debug.Log("아고라에 가입 요청 UID: " + userNo);
        if (userNo != 0 && app != null)
            app.join(joinRoomID, audioFlag, (uint)userNo);
    }

    //아고라 나가는 메서드
    public void LeaveAgoraRoom()
    {
        // 아고라 나가기 테스트
        if(app != null)
            app.leave(); // leave channel
    }

    public void LoadNewAgoraInstance()
    {
        app.leave();
        app.unloadEngine();
        app.loadEngine("87d658834c824bc9afbe668d75dd4c0b");
    }

    public void CamToggle()
    {
        if (camObject.gameObject.activeSelf)
        {
            //camCanvasGroup.alpha = 0;
            //camCanvasGroup.interactable = false;
            //camCanvasGroup.blocksRaycasts = false;
            camObject.gameObject.SetActive(false);
            toggleButton.GetComponentInChildren<Text>().text = "∨";
        }
        else
        {
            //camCanvasGroup.alpha = 1;
            //camCanvasGroup.interactable = true;
            //camCanvasGroup.blocksRaycasts = true;
            camObject.gameObject.SetActive(true);
            toggleButton.GetComponentInChildren<Text>().text = "∧";
        }
    }
    
}
