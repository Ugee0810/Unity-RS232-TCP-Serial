using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[InitializeOnLoad]
public class GameManager : MonoBehaviour
{
    //[SerializeField] public Canvas can1;
    //[SerializeField] public Canvas can2;
    //[SerializeField] public AudioSource information;
    //[SerializeField] public AudioSource Informaiton1;
    //[SerializeField] public GameObject cube;
    //[SerializeField] public GameObject cube2;
    //[SerializeField] public Collider collider;
    //[SerializeField] public Collider collider2;

    public bool startBtn;

    private static GameManager startInstance;
    public static GameManager Instance
    {
        get
        {
            if (!startInstance)
                startInstance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return
                startInstance;
        }
    }

    private void Awake()
    {
        if (startInstance == null)
            startInstance = this;
        else if (startInstance != this)
            Destroy(gameObject);
    }


    private void Update()
    {
        everyScript();
    }

    public void everyScript()
    {
        if (startBtn)
        {
            //AudioInvokeStart();
            //StartCoroutine("Info1Audio");
            //StartCoroutine("CubeDestroy");
            startBtn = false;
        }
    }

    //public void AudioInvokeStart()
    //{
    //    information.Play();        
    //}    

    //IEnumerator Info1Audio()
    //{
    //    yield return new WaitForSeconds(12.0f);
    //    Informaiton1.Play();
    //    Destroy(can1);
    //    Debug.Log("Audio");
    //}

    //IEnumerator CubeDestroy()
    //{
    //    yield return new WaitForSeconds(40.0f);
    //    Destroy(can2);
    //    Destroy(cube);
    //    Destroy(collider);
    //    Destroy(cube2);
    //    Destroy(collider2);
    //    Debug.Log("DsCube");
    //}
}
