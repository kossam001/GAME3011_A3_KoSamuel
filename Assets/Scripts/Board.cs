using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform rightBound;
    public Transform leftBound;
    public Transform bottomBound;
    
    public int rows;
    public int columns;

    public List<Sprite> tileSprites;

    // Start is called before the first frame update
    void Start()
    {
        rightBound.position = new Vector3(columns, 0.0f);

        leftBound.localScale = new Vector3(1.0f, rows);
        rightBound.localScale = new Vector3(1.0f, rows);

        bottomBound.localScale = new Vector3(columns, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
