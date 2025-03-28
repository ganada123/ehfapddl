using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// �̱��� ������ �����ϴ� �߻� Ŭ����
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(); // ���� ������ �ش� Ÿ���� ��ü�� ã��
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name; // ��ü �̸��� Ŭ���� �̸����� ����
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
            SceneManager.sceneLoaded += OnSceneLoaded; // ���� �ε�� �� ȣ��Ǵ� �׼� �޼��� �Ҵ�
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);
    
}