using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Sorting : MonoBehaviour
{
    [SerializeField] private int numberOfCubes;
    [SerializeField] private float speed;

    private List<GameObject> cubes;

    // UI
    private bool isPause = false;
    private bool isSorting = false;
    public GameObject sorting_ui;
    public GameObject unsorting_ui;
    public GameObject pause_button;
    public Sprite pause_sprite;
    public Sprite continue_sprite;
    public TMP_Text algorithm_name;

    // For Bug Fixing...
    GameObject temp;
    GameObject[] tempList; // for merge sort

    private void Start()
    {
        numberOfCubes = 10;
        speed = 0.5f;
        cubes = new List<GameObject>();
        Init();
    }

    void Init()
    {
        StopAllCoroutines();
        // Clear All Cubes
        if(cubes != null)
        {
            for (int i = 0; i < cubes.Count; i++)
                Destroy(cubes[i].gameObject);
            cubes.Clear();
        }

        Destroy(temp);

        cubes = new List<GameObject>();

        for(int i=0; i<numberOfCubes; i++)
        {
            int randomNumber = Random.Range(1, numberOfCubes + 1);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.9f, randomNumber, 1.0f);
            cube.transform.position = new Vector3(i, randomNumber / 2.0f, 0);

            cube.transform.SetParent(this.transform);

            cubes.Add(cube);
        }
        Camera.main.transform.position = new Vector3(numberOfCubes / 2.0f, numberOfCubes / 2.0f + 1.5f, -numberOfCubes - 3);
    }

    private void Update()
    {
        if(isSorting)
        {
            sorting_ui.SetActive(true);
            unsorting_ui.SetActive(false);
        }
        else
        {
            sorting_ui.SetActive(false);
            unsorting_ui.SetActive(true);
        }
    }
    
    /// Buttons /////////////////////////////////////////////////
    public void OnBubbleSort()
    {
        if (IsSorted(cubes))
            Init();
        isSorting = true;
        algorithm_name.text = "Bubble Sort";
        StartCoroutine(BubbleSort(cubes));
    }
    
    public void OnQuickSort()
    {
        if (IsSorted(cubes))
            Init();
        isSorting = true;
        algorithm_name.text = "Quick Sort";
        StartCoroutine(QuickSort(cubes, 0, cubes.Count - 1));
    }

    public void OnHeapSort()
    {
        if (IsSorted(cubes))
            Init();
        isSorting = true;
        algorithm_name.text = "Heap Sort";
        StartCoroutine(HeapSort(cubes));
    }

    public void OnMergeSort()
    {
        if (IsSorted(cubes))
            Init();
        isSorting = true;
        algorithm_name.text = "Merge Sort";
        StartCoroutine(MergeSort(cubes, 0, cubes.Count-1));
    }
    /*--------------------------------------------------------------*/

    /// Algorithm /////////////////////////////////////////////////////////

    // Bubble Sort
    IEnumerator BubbleSort(List<GameObject> c)
    {
        for(int i=0; i<c.Count; i++)
        {
            for(int j=0; j<c.Count - i -1; j++)
            {
                yield return new WaitForSeconds(speed);
                if(j != i)
                    LeanTween.color(c[j], Color.yellow, 0.01f);
                LeanTween.color(c[j+1], Color.blue, 0.01f);

                if(c[j].transform.localScale.y > c[j+1].transform.localScale.y)
                {
                    // Swap
                    temp = c[j];
                    yield return new WaitForSeconds(speed);
                    LeanTween.moveLocalX(c[j], c[j+1].transform.localPosition.x, speed);
                    LeanTween.moveLocalZ(c[j], -1.5f, speed).setLoopPingPong(1);
                    c[j] = c[j + 1];

                    LeanTween.moveLocalX(c[j+1], temp.transform.localPosition.x, speed);
                    LeanTween.moveLocalZ(c[j + 1], 1.5f, speed).setLoopPingPong(1);
                    c[j + 1] = temp;

                    yield return new WaitForSeconds(speed);
                }
                yield return new WaitForSeconds(speed);
                LeanTween.color(c[j], Color.white, 0.01f);
                LeanTween.color(c[j+1], Color.white, 0.01f);
            }
        }

        // Complete
        if (IsSorted(c))
        {
            StartCoroutine(Complete());
        }
    }
    
    // Quick Sort
    IEnumerator QuickSort(List<GameObject> c, int left, int right)
    {
        if (left < right)
        {
            int pivot = (int)c[right].transform.localScale.y;
            LeanTween.color(c[right], Color.red, 0.01f);

            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                yield return new WaitForSeconds(speed);
                LeanTween.color(c[j], Color.blue, 0.01f);

                if (c[j].transform.localScale.y < pivot)
                {
                    yield return new WaitForSeconds(speed);
                    i++;
                    LeanTween.color(c[i], Color.yellow, 0.01f);

                    yield return new WaitForSeconds(speed * 1.5f);
                    // Swap
                    temp = c[i];
                    Vector3 tempPosition = c[i].transform.localPosition;

                    LeanTween.moveLocalX(c[i], c[j].transform.localPosition.x, speed);
                    LeanTween.moveZ(c[i], -1.5f, speed).setLoopPingPong(1);
                    c[i] = c[j];

                    LeanTween.moveLocalX(c[j], tempPosition.x, speed);
                    LeanTween.moveZ(c[j], 1.5f, speed).setLoopPingPong(1);
                    c[j] = temp;

                    yield return new WaitForSeconds(speed * 1.5f);
                    LeanTween.color(c[i], Color.white, 0.01f);
                }
                yield return new WaitForSeconds(speed);
                LeanTween.color(c[j], Color.white, 0.01f);
            }

            yield return new WaitForSeconds(speed * 1.5f);
            // Swap Again
            temp = c[i + 1];
            Vector3 tP = c[i + 1].transform.localPosition;

            LeanTween.moveLocalX(c[i + 1], c[right].transform.localPosition.x, speed);
            LeanTween.moveZ(c[i + 1], -1.5f, speed).setLoopPingPong(1);
            c[i + 1] = c[right];

            LeanTween.moveLocalX(c[right], tP.x, speed);
            LeanTween.moveZ(c[right], 1.5f, speed).setLoopPingPong(1);
            c[right] = temp;

            LeanTween.color(c[i+1], Color.white, 0.01f);
            yield return new WaitForSeconds(speed  * 1.5f);

            // Partiction End !!!

            int p = i + 1;
            yield return new WaitForSeconds(speed * 1.5f);
            StartCoroutine(QuickSort(c, p + 1, right));
            yield return new WaitForSeconds(speed * 1.5f);
            StartCoroutine(QuickSort(c, left, p - 1));
        }

        // Complete
        if (IsSorted(cubes))
        {
            yield return new WaitForSeconds(speed);
            StartCoroutine(Complete());
        }
    }

    // Heap Sort
    IEnumerator HeapSort(List<GameObject> c)
    {
        int n = c.Count;

        for (int i = n / 2 - 1; i >= 0; i--)
        {
            yield return BuildHeap(c, n, i);
        }

        for(int i=n-1; i>=0; i--)
        {
            yield return new WaitForSeconds(speed);
            temp = c[0];
            int tempX = (int)c[0].transform.localPosition.x;

            LeanTween.color(c[0], Color.cyan, 0.01f);
            LeanTween.moveLocalX(c[0], c[i].transform.localPosition.x, speed);
            LeanTween.moveLocalZ(c[0], -1.5f, speed).setLoopPingPong(1);
            c[0] = c[i];

            LeanTween.color(c[i], Color.white, 0.01f);
            LeanTween.moveLocalX(c[i], tempX, speed);
            LeanTween.moveLocalZ(c[i], 1.5f, speed).setLoopPingPong(1);
            c[i] = temp;

            yield return BuildHeap(c, i, 0);
        }

        if(IsSorted(cubes))
        {
            yield return new WaitForSeconds(speed);
            StartCoroutine(Complete());
        }
    }   

    IEnumerator BuildHeap(List<GameObject> c, int n, int i)
    {
        int largest = i;
        int l = i * 2 + 1;
        int r = i * 2 + 2;

        LeanTween.color(c[largest], Color.red, 0.01f);

        yield return new WaitForSeconds(speed);
        if(l < n && c[l].transform.localScale.y > c[largest].transform.localScale.y)
        {
            largest = l;
            LeanTween.color(c[largest], Color.yellow, 0.01f);
        }

        if (r < n && c[r].transform.localScale.y > c[largest].transform.localScale.y)
        {
            largest = r;
            LeanTween.color(c[largest], Color.yellow, 0.01f);
        }

        if(largest != i)
        {
            yield return new WaitForSeconds(speed);
            // Swap
            temp = c[i];
            int tempX = (int)c[i].transform.localPosition.x;

            LeanTween.moveLocalX(c[i], c[largest].transform.localPosition.x, speed);
            LeanTween.moveLocalZ(c[i], -1.5f, speed).setLoopPingPong(1);
            c[i] = c[largest];

            LeanTween.moveLocalX(c[largest], tempX, speed);
            LeanTween.moveLocalZ(c[largest], 1.5f, speed).setLoopPingPong(1);
            c[largest] = temp;

            LeanTween.color(c[i], Color.white, 0.01f);
            LeanTween.color(c[largest], Color.white, 0.01f);
            yield return BuildHeap(c, n, largest);
        }

        LeanTween.color(c[largest], Color.white, 0.01f);
    }

    // Merge Sort
    IEnumerator MergeSort(List<GameObject> c, int low, int high)
    {
        if(low < high)
        {
            yield return new WaitForSeconds(speed);
            int mid = (low + high) / 2;

            LeanTween.color(c[mid], Color.red, 0.01f);
            yield return new WaitForSeconds(speed);
            LeanTween.color(c[mid], Color.white, 0.01f);
            

            yield return MergeSort(c, low, mid);
            yield return MergeSort(c, mid+1, high);

            yield return Merge(c, low, high, mid);
        }

        if(IsSorted(cubes))
        {
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(Complete());
        }
    }

    IEnumerator Merge(List<GameObject> c, int low, int high, int mid)
    {
        yield return new WaitForSeconds(speed);

        int leftIndex = low;
        int rightIndex = mid + 1;
        int mergeIndex = low;
        
        for (int i = low; i<= high; i++)
        {
            LeanTween.moveLocalZ(c[i], c[i].transform.localPosition.z + 1.5f, speed);
            LeanTween.color(c[i], Color.cyan, 0.01f);
        }
        

        tempList = new GameObject[numberOfCubes];

        while(leftIndex <= mid && rightIndex <= high)
        {
            yield return new WaitForSeconds(speed);

            if(c[leftIndex].transform.localScale.y < c[rightIndex].transform.localScale.y)
            {
                LeanTween.color(c[leftIndex], Color.yellow, 0.01f);

                tempList[mergeIndex] = c[leftIndex];
                leftIndex++;
                yield return new WaitForSeconds(speed);
                LeanTween.color(c[leftIndex-1], Color.white, 0.01f);
            }
            else
            {
                LeanTween.color(c[rightIndex], Color.yellow, 0.01f);

                tempList[mergeIndex] = c[rightIndex];
                rightIndex++;
                yield return new WaitForSeconds(speed);
                LeanTween.color(c[rightIndex-1], Color.white, 0.01f);
            }
            mergeIndex++;
        }

        while(leftIndex <= mid)
        {
            LeanTween.color(c[leftIndex], Color.yellow, 0.01f);
            yield return new WaitForSeconds(speed);
            LeanTween.color(c[leftIndex], Color.white, 0.01f);
            
            tempList[mergeIndex] = c[leftIndex];
            mergeIndex++;
            leftIndex++;
        }

        while(rightIndex <= high)
        {
            LeanTween.color(c[rightIndex], Color.yellow, 0.01f);
            yield return new WaitForSeconds(speed);
            LeanTween.color(c[rightIndex], Color.white, 0.01f);
            tempList[mergeIndex] = c[rightIndex];
            mergeIndex++;
            rightIndex++;
        }

        for (int i=low;i<mergeIndex;i++)
        {
            yield return new WaitForSeconds(speed);
            LeanTween.moveLocalX(tempList[i], i, speed);
            
            c[i] = tempList[i];
            
            LeanTween.moveLocalZ(c[i], 0f, speed);
            LeanTween.color(c[i], Color.white, speed);
        }
    }
    /*--------------------------------------------------------------*/
    
    private bool IsSorted(List<GameObject> o)
    {
        for(int i=1; i< o.Count; i++)
        {
            int front = (int)o[i-1].transform.localScale.y;
            int back = (int)o[i].transform.localScale.y;

            if(front > back)
                return false;
        }
        return true;
    }

    IEnumerator Complete()
    {
        for(int i=0; i<cubes.Count; i++)
        {
            yield return new WaitForSeconds(0.03f);
            LeanTween.color(cubes[i], Color.green, 0.01f);
            LeanTween.moveLocalZ(cubes[i], 0, speed);
        }
        yield return new WaitForSeconds(0.05f);
        isSorting = false;
        StopAllCoroutines();
    }

    public void SpeedFromSlider(float speed) => this.speed = speed;
   
    public void MaxCubeFromSlider(float number)
    {
        this.numberOfCubes = (int)number;
        Init();
    }

    public void OnPauseButton()
    {
        if(isPause && isSorting)
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
        StopAllCoroutines();
        isSorting = false;
        isPause = false;
        pause_button.GetComponent<Image>().sprite = pause_sprite;
        Time.timeScale = 1;
        Init();
    }
    /*--------------------------------------------------------------*/
}