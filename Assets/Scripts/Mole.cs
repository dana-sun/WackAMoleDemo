using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WackAMole;



public class Mole : MonoBehaviour
{

    public bool isInitiated;
    public GameObject GameManageObj;
    public bool isShowing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitiated){
            MeshRenderer childObject = gameObject.GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
            if (childObject != null) {
                isInitiated = true;
                GameManageObj = GameObject.FindGameObjectWithTag("GameManager");
                GameManageObj.GetComponent<GameManage>().selectMoles.Add(this.gameObject);
            }
        }
    }

    public IEnumerator ShowMole(GameObject mole, float duration){
        float time = 0;
        Vector3 startPosition = transform.position;
        Vector3 visibleHeight = new Vector3(transform.position.x, 1, transform.position.z);
        while (time < duration){
            transform.position = Vector3.Lerp(startPosition, visibleHeight, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = visibleHeight;
        isShowing = true;
        // Debug.Log("showMole" + transform.position);
    }

    public IEnumerator HideMole(GameObject mole, float duration){
        float time = 0;
        Vector3 startPosition = transform.position;
        Debug.Log(startPosition);
        Vector3 hiddenHeight = new Vector3(transform.position.x, -2, transform.position.z);
        while (time < duration){
            transform.position = Vector3.Lerp(startPosition, hiddenHeight, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = hiddenHeight;
        isShowing = false;
        // Debug.Log("hide");
    }
    
}
