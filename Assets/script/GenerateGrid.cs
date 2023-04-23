using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GenerateGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject cell;
    [SerializeField] private List<cell> cells = new List<cell>();
    [SerializeField] private float speed;

    private bool is_pathFinding = false;
    private bool generated=false;
    private bool isPause = false;
    public GameObject maze_ui;
    public GameObject A_star_ui;
    public GameObject pause_button;
    public Sprite pause_sprite;
    public Sprite continue_sprite;
    private void Start()
    {
        width = 10;
        height = 10;
        speed = 0.5f;
        Init();
    }

    void Init()
    {
        StopAllCoroutines();
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        
        cells.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject newCell;
                newCell=Instantiate(cell,this.transform);
                newCell.transform.position = new Vector3(j, i, 0);
                cells.Add(newCell.GetComponent<cell>());
            }
        }

        for (int i = 0; i < cells.Count; i++)
        {
            foreach (var c in cells)
            {
                if(c==cells[i]) continue;
                if (Vector2.Distance(new Vector2(cells[i].transform.localPosition.x,cells[i].transform.localPosition.y),
                        new Vector2(c.transform.localPosition.x,c.transform.localPosition.y))<=1)
                {
                    cells[i].cellaround.Add(c);
                }
            }
        }
        Camera.main.transform.position = new Vector3(width / 2.0f, width / 2.0f + 1.5f, -width - 3);
    }

    private void Update()
    {
        if (generated)
        {
            maze_ui.SetActive(false);
            A_star_ui.SetActive(true);
        }else if (!is_pathFinding)
        {
            maze_ui.SetActive(true);
            A_star_ui.SetActive(false);
        }
    }
    
    /// Buttons /////////////////////////////////////////////////
    public void startGenerate()
    {
        Init();
        is_pathFinding = false;
        generated = false;
        StartCoroutine(DFS(cells));
        
    }
    public void startPathfinding()
    {
        //innit();
        if (generated)
        {
            StartCoroutine(A_star());
            is_pathFinding = true;
        }
        
        else
        {
            Debug.Log("hold");
        }
    }
    
    /// Algorithm /////////////////////////////////////////////////////////
    /// depth first search maze generation
    IEnumerator DFS(List<cell> _cells)
    {
        cell current = _cells[0];
        Stack<cell> _Scells = new Stack<cell>();
        _Scells.Push(current);

        while (_Scells.Count > 0)
        {
            current = _Scells.Pop();
            
            if (current.getUnvisited().Count > 0)
            {
                _Scells.Push(current);

                cell chosen = current.getUnvisited()[Random.Range(0, current.getUnvisited().Count)];
                RemoveWall(current,chosen);
                
                current.neightbor.Add(chosen);
                chosen.neightbor.Add(current);
                
                LeanTween.color(current.transform.GetChild(4).gameObject, Color.yellow, 0.01f);
                LeanTween.color(chosen.transform.GetChild(4).gameObject, Color.blue, 0.01f);
                
                yield return new WaitForSeconds(this.speed);
                
                chosen.visitedforDFS = true;
                _Scells.Push(chosen);
                
                LeanTween.color(current.transform.GetChild(4).gameObject, Color.white, 0.01f);
                LeanTween.color(chosen.transform.GetChild(4).gameObject, Color.white, 0.01f);
            }
            
        }
        yield return new WaitForSeconds(this.speed/2);
        LeanTween.color(cells[0].transform.GetChild(4).gameObject, Color.green, 0.01f);
        LeanTween.color(cells[cells.Count-1].transform.GetChild(4).gameObject, Color.red, 0.01f);
        generated = true;
        yield return null;
    }
    
    void RemoveWall(cell current, cell chosen)
    {
        float dx = chosen.transform.localPosition.x - current.transform.localPosition.x;
        float dy = chosen.transform.localPosition.y - current.transform.localPosition.y;

        if (dx >= 1)
        {
            current.transform.GetChild(1).gameObject.SetActive(false);
            chosen.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (dx <= -1)
        {
            current.transform.GetChild(0).gameObject.SetActive(false);
            chosen.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (dy >= 1)
        {
            current.transform.GetChild(2).gameObject.SetActive(false);
            chosen.transform.GetChild(3).gameObject.SetActive(false);
        }
        if (dy <= -1)
        {
            current.transform.GetChild(3).gameObject.SetActive(false);
            chosen.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    
    //A* path finding
    IEnumerator A_star()
    {
        //set start and target and 2 lists
        cell start = cells[0];
        cell target = cells[cells.Count - 1];
        List<cell> toCalculate = new List<cell>();
        List<cell> alreadyCalculate = new List<cell>();
        //adding first cell
        toCalculate.Add(start);
        
        //loopthrough first list
        while (toCalculate.Count > 0)
        {   
            cell current = toCalculate[0];
            //finding the lowest f in list
            for (int i = 0; i < toCalculate.Count; i++)
            {
                if (toCalculate[i].f < current.f)
                {
                    current = toCalculate[i];
                }
            }
            
            
            //if find the target return the path 
            if (current == target)
            {
                //yield return new WaitForSeconds(this.speed*2);
                foreach (var c in alreadyCalculate)
                {
                    //yield return new WaitForSeconds(this.speed);
                    LeanTween.color(c.transform.GetChild(4).gameObject, Color.white, 0.01f);
                }
                foreach (var c in toCalculate)
                {
                    //yield return new WaitForSeconds(this.speed);
                    LeanTween.color(c.transform.GetChild(4).gameObject, Color.white, 0.01f);
                }
                foreach (var c in ReconstructPath(current))
                {
                    yield return new WaitForSeconds(this.speed);
                    LeanTween.color(c.transform.GetChild(4).gameObject, Color.green, 0.01f);
                }

                generated = false;
                is_pathFinding = false;
                toCalculate.Clear();
                break;
            }
            //remove from open to close list
            toCalculate.Remove(current);
            alreadyCalculate.Add(current);
            foreach (var c in alreadyCalculate)
            {
                LeanTween.color(c.transform.GetChild(4).gameObject, Color.grey, 0.01f);
            }
            
            yield return new WaitForSeconds(this.speed);
            foreach (var c in current.neightbor)
            {
                LeanTween.color(c.transform.GetChild(4).gameObject, Color.blue, 0.01f);
                
                //skip neighbor that have been calculated
                if (alreadyCalculate.Contains(c))
                {
                    continue;
                }
                //calculate current g score
                float tentativeG = current.g + 1;
                
                //if not already in calculate list add in
                if (!toCalculate.Contains(c))
                {
                    toCalculate.Add(c);
                    
                    
                }else if (tentativeG >= c.g) //skip if it g is bigger than neighbor g
                {
                    continue;
                }
                //record the parent and calculate g and f
                c.ComeFrom = current;
                c.g = tentativeG;
                c.f = c.f + c.getH(target);
            }
        }
        
        yield return null;
    }
    
    private List<cell> ReconstructPath(cell c) {
        List<cell> path = new List<cell>() { c };
        cell current = c;
        while (current.ComeFrom != null) {
            current = current.ComeFrom;
            path.Insert(0, current);
        }
        return path;
    }
    
    /*--------------------------------------------------------------*/
    
    /*--------------------------------------------------------------*/
    public void SpeedFromSlider(float speed) => this.speed = speed;
   
    public void SetSize(float number)
    {
        this.width = (int)number;
        this.height = (int)number;
        Init();
    }

    public void OnPauseButton()
    {
        if(isPause)
        {
            isPause = !isPause;
            pause_button.GetComponent<Image>().sprite = pause_sprite;
            Time.timeScale = 1;
        }else
        {
            isPause = !isPause;
            pause_button.GetComponent<Image>().sprite = continue_sprite;
            Time.timeScale = 0;
        }
    }

    public void OnBackToMenuButton()
    {
        Init();
        generated = false;
        is_pathFinding = false;
        isPause = false;
        pause_button.GetComponent<Image>().sprite = pause_sprite;
        Time.timeScale = 1;
        
    }
    /*--------------------------------------------------------------*/
}
