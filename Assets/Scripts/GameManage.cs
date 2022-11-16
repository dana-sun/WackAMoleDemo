using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WackAMole
{
    public class GameManage : MonoBehaviour
    {

        public int numMoles = 9;
        public List<GameObject> selectMoles = new List<GameObject>();
        public GameObject mole;

        
        public TextMeshPro timerText;
        public TextMeshPro scoreText;
        public int score = 0;
        public float gameTimer = 30f;
        private float elapseTime;
        public float showTimer = 0.5f;
        public float riseAndHideTimer = 0.3f;
        
        public Vector3 visiblePosition;
        private Vector3 startPosition;
        public Vector3 hiddenPosition;
        

        void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            startPosition = transform.position;

        }

        // Update is called once per frame
        void Update()
        {
            if (selectMoles.Count == 9)
            {    
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    // GameObject hitObject = hit.transform.gameObject.GetComponent<Mole>();
                    if (Physics.Raycast(ray, out hit, 100.0f)) {
                        
                        if (hit.transform != null && hit.transform.gameObject.GetComponentInParent<Mole>() != null && hit.transform.gameObject.GetComponentInParent<Mole>().isShowing == true){
                            StartCoroutine(hit.transform.gameObject.GetComponentInParent<Mole>().HideMole(hit.transform.gameObject, riseAndHideTimer));
                            score += 10;
                            scoreText.text = "Score: " + score;
                        }
                    }   
                    }
                if (gameTimer == 30f){
                    StartCoroutine(GameRoutine(showTimer));
                   
                }
                else{
                    timerText.text = "Game Over";
                }
                gameTimer -= Time.deltaTime;
                timerText.text = "Time Remaining: " + Mathf.Floor(gameTimer);
            }                        
        }

        IEnumerator GameRoutine(float localShowTimer)
        {
            while(gameTimer > 0f){
                //if Physics.Raycast(ray, out hit)
                //check if raycast is formed and the hit is where the selected mole is and if it reaches the mole then hidemole, 
                
                localShowTimer -= Time.deltaTime;
                int rando = Random.Range(0, selectMoles.Count);
                mole = selectMoles[rando];
                StartCoroutine(mole.GetComponent<Mole>().ShowMole(mole, riseAndHideTimer));
                
             
                yield return new WaitForSeconds(showTimer);
                // Debug.Log("gameManage" + mole.transform.position);
                StartCoroutine(mole.GetComponent<Mole>().HideMole(mole, riseAndHideTimer));
            }
            


        }

  

    }
}