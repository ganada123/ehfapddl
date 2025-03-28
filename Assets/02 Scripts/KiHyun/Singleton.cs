using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// 싱글톤 패턴을 제공하는 추상 클래스
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(); // 현재 씬에서 해당 타입의 객체를 찾음
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name; // 객체 이름을 클래스 이름으로 설정
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때 호출되는 액션 메서드 할당
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);
    
}