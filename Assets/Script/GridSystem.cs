using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSystem : Helper
{
    public static GridSystem instance = null;

    public Material newMaterial;

    //Text objects for ui purposes
    public Text pointText;
    public Text moveText;
    public Text alarmText;
    public GameObject gameoverPanel;

    int point = 0;
    int move = 0;
    int bombLimitMove = 6;



//    private int selectionStatus;

    public List<Material> hexagonMaterials;

    //Hexagon prefabs
    public Transform hexPrefab;
    public Transform bombHexPrefab;

    //Main variables to generate grid
    public int gridWidth = 8;
    public int gridHeight = 9;
    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    public float gap = 0.0f;
    Vector3 startPos;

    //Valid side hexes
    List<Hexagon> sideHexes = new List<Hexagon>();
    Hexagon[,] hexes;
    List<Material> materials = new List<Material>();

    void Awake()
    {
        if (PlayerPrefs.GetInt("WIDTH") != 0 && PlayerPrefs.GetInt("HEIGHT") != 0)
        {
            gridWidth = PlayerPrefs.GetInt("WIDTH");
            gridHeight = PlayerPrefs.GetInt("HEIGHT");
        }
        if (instance == null) {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {

            hexes = new Hexagon[gridHeight, gridWidth];
        addGap();
        calcStartPos();
        materials = generateRandomMaterials();
        createGrid();
    }

    //This function add gap between hexagons
    void addGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    //This function is calculate start point to generate grid
    void calcStartPos()
    {
        float offset = 0;
        if (gridWidth / 2 % 2 != 0)
            offset = hexHeight / 2;

        float x = -hexWidth * (gridWidth / 2) - offset;
        float y = hexHeight * 0.75f * (gridHeight / 2);

        startPos = new Vector3(x, y, 0);
    }

    Vector3 calcWorldPos(Vector2 gridPos)
    {
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = startPos.x + gridPos.x * hexWidth + offset;
        float y = startPos.y - gridPos.y * hexHeight * 0.75f;

        return new Vector3(x, y, 0);
    }


    void createGrid()
    {
        int k = 0;
        for (int y = 0; y < gridWidth; y++)
        {
            for (int x = 0; x < gridHeight; x++)
            {
                Transform hex = Instantiate(hexPrefab) as Transform;
                hex.gameObject.GetComponent<Hexagon>().setMaterial(materials[k]);
                Vector2 gridPos = new Vector2(x, y);
                hex.position = calcWorldPos(gridPos);
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;
                hexes[x, y] = hex.gameObject.GetComponent<Hexagon>();
                hexes[x, y].setX(x);
                hexes[x, y].setY(y);
                hexes[x, y].setMaterial(materials[k]);
                k++;
            }

        }
        this.transform.Rotate(0, 0, -90);
    }


    //This function generates random colors which are not repeatitive in column
    List<Material> generateRandomMaterials()
    {
        List<Material> materials = new List<Material>();
        int randomMaterial = Random.Range(0, hexagonMaterials.Count);
        int preMaterial = randomMaterial;
        materials.Add(hexagonMaterials[randomMaterial]);
        for (int i = 1; i < gridHeight * gridWidth; i++)
        {
            randomMaterial = Random.Range(0, hexagonMaterials.Count);
            while (randomMaterial == preMaterial)
            {
                randomMaterial = Random.Range(0, hexagonMaterials.Count);
            }
            preMaterial = randomMaterial;
            materials.Add(hexagonMaterials[randomMaterial]);
        }
        return materials;
    }



    public void findThreeValidHexagons(Hexagon selectedHexagon, int selectionStatus)
    {
        Vector2 firstPos, secondPos;

        stopActiveHexesAnimation("scale",sideHexes);
        findAroundHexagons(out firstPos, out secondPos, selectedHexagon, selectionStatus);
        sideHexes.Clear();
        sideHexes.Add(selectedHexagon);
        sideHexes.Add(hexes[(int)firstPos.x,(int)firstPos.y]);
        sideHexes.Add(hexes[(int)secondPos.x, (int)secondPos.y]);
        startActiveHexesAnimation("scale", sideHexes);
    }

    private void stopActiveHexesAnimation(string animationName, List<Hexagon> activeHexes) {
        for (int i = 0; i < activeHexes.Count; i++)
        {
            Animator animator;
            if (sideHexes[i] != null)
            {
                animator = activeHexes[i].GetComponent<Animator>();
                animator.SetBool(animationName, false);
            }
        }
    }
    private void startActiveHexesAnimation(string animationName, List<Hexagon> activeHexes)
    {
        stopActiveHexesAnimation(animationName, activeHexes);
        for (int i = 0; i < activeHexes.Count; i++)
        {
            Animator animator;
            animator = activeHexes[i].GetComponent<Animator>();
            animator.SetBool(animationName, true);
        }
    }

    private void startBombHexagonAnimation(string animationName, GameObject hexagon)
    {
        Animator animator;
        animator = hexagon.GetComponent<Animator>();
        animator.SetBool(animationName, true);
    }


    //This function is to find valid side hexagons
    private void findAroundHexagons(out Vector2 first, out Vector2 second, Hexagon selectedHexagon, int selectionStatus)
    {
        Hexagon.NeighbourHexes neighbours = selectedHexagon.getSidehexes();
        bool breakLoop = false;


        //Detect valid side hexagons
        do
        {
            switch (selectionStatus)
            {
                case 0: first = neighbours.up; second = neighbours.upRight; break;
                case 1: first = neighbours.upRight; second = neighbours.downRight; break;
                case 2: first = neighbours.downRight; second = neighbours.down; break;
                case 3: first = neighbours.down; second = neighbours.downLeft; break;
                case 4: first = neighbours.downLeft; second = neighbours.upLeft; break;
                case 5: first = neighbours.upLeft; second = neighbours.up; break;
                default: first = Vector2.one; second = Vector2.one; break;
            }

            //Loop until appropriate condition occures
            if (first.x < zero || first.x >= gridHeight || first.y < zero || first.y >= gridWidth || second.x < zero || second.x >= gridHeight || second.y < zero || second.y >= gridWidth)
            {
                selectionStatus = (++selectionStatus) % selectionCounter;
            }
            else
            {
                breakLoop = true;
            }
        } while (!breakLoop);
    }
    public static bool isEvenColumn(int y)
    {
        return (y % 2 == 0);
    }


    //Function for rotating hexagons
    public IEnumerator swapHexagons(bool clockWise)
    {
        rotating = true;
        for (int i = 0; i < sideHexes.Count; i++)
        {
            int x1, x2, x3, y1, y2, y3;
            Vector2 pos1, pos2, pos3;
            Hexagon first, second, third;

            first = sideHexes[0];
            second = sideHexes[1];
            third = sideHexes[2];

            x1 = first.getX();
            x2 = second.getX();
            x3 = third.getX();

            y1 = first.getY();
            y2 = second.getY();
            y3 = third.getY();

            pos1 = first.transform.position;
            pos2 = second.transform.position;
            pos3 = third.transform.position;

            if (clockWise)
            {
                first.rotate(x2, y2, pos2);
                hexes[x2,y2] = first;

                second.rotate(x3, y3, pos3);
                hexes[x3,y3] = second;

                third.rotate(x1, y1, pos1);
                hexes[x1,y1] = third;
            }
            else
            {
                first.rotate(x3, y3, pos3);
                hexes[x3,y3] = first;

                second.rotate(x1, y1, pos1);
                hexes[x1,y1] = second;

                third.rotate(x2, y2, pos2);
                hexes[x2,y2] = third;
            }

            yield return new WaitForSeconds(0.5f);

            List<Hexagon> explosiveHexagons = new List<Hexagon>();

            explosiveHexagons = checkExplosion(hexes);

            //Find all explosive area
            if (explosiveHexagons.Count > zero)
            {
                if (bombIsOn)
                {
                    bombLimitMove--;
                    alarmText.text = "You have " + bombLimitMove + " chances to defuse the bomb!!";
                    if (bombLimitMove == 0)
                    {
                        gameOver = true;
                        gameoverPanel.SetActive(true);
                        this.gameObject.SetActive(false);
                    }
                }
                move += 1;
                stopActiveHexesAnimation("scale",sideHexes);
                while (explosiveHexagons.Count > 0)
                {
                    checkingExplosion = true;
                    explodeHexagons(explosiveHexagons);
                    explosiveHexagons = checkExplosion(hexes);
                }
                rotating = false;
                checkingExplosion = false;
                break;
            }

        }
        rotating = false;
    }

    //This function starts to detect null places to shift up hexagons to below
    private void reassignHexagons(List<Hexagon> explosiveHexagons)
    {
        reassigning = true;
        List<int> emptyPlace = new List<int>();
        Vector2 newPosition;
        for (int i = 0; i < explosiveHexagons.Count; i++)
        {
            for (int y = 0; y < gridWidth; y++)
            {
                for (int x = 0; x < gridHeight; x++)
                {
                    if (hexes[x, y] == null && x != 0 && hexes[x - 1, y] != null)
                    {
                        newPosition.x = hexes[x - 1, y].transform.position.x;
                        newPosition.y = hexes[x - 1, y].transform.position.y - 1.8186f;
                        hexes[x - 1, y].transform.position = newPosition;
                        hexes[x, y] = hexes[x - 1, y];
                        hexes[x, y].setX(hexes[x - 1, y].getX() + 1);
                        hexes[x - 1, y] = null;
                    }
                }
            }
        }
        reassigning = false;
        produceHexagonForEmptySpace();
    }

    private void produceHexagonForEmptySpace() {
        producing = true;
        if (point % 1000 == 0 && !bombIsOn)
        {
            bombPermission = true;
        }
        Vector2 helperPos;
        int k = 0;
        for (int y = 0; y < gridWidth; y++)
        {
            for (int x = 0; x < gridHeight; x++)
            {
                if (hexes[x, y] == null)
                {
                    for (int i = 0; i < gridHeight; i++)
                    {
                        if (hexes[x + i, y] != null)
                        {
                            int distance = i;
                            helperPos.x = hexes[x + i, y].transform.position.x;
                            helperPos.y = hexes[x + i, y].transform.position.y + (1.8186f * distance);

                            Transform hex;
                            if (!bombPermission)
                            {
                                hex = Instantiate(hexPrefab) as Transform;
                                hex.name = "Hexagon" + x + "|" + y;
                            }
                            else
                            {
                                hex = Instantiate(bombHexPrefab) as Transform;
                                startBombHexagonAnimation("rotate", hex.gameObject);
                                hex.name = "Bomb Hexagon" + x + "|" + y;
                                bombPermission = false;
                                bombIsOn = true;
                                alarmText.gameObject.SetActive(true);
                                alarmText.text = "You have " + bombLimitMove + " chances to defuse the bomb!!";
                            }

                            hex.gameObject.GetComponent<Hexagon>().setMaterial(materials[k]);
                            hex.position = helperPos;
                            hex.parent = this.transform;
                            hex.gameObject.GetComponent<Hexagon>().setX(x);
                            hex.gameObject.GetComponent<Hexagon>().setY(y);

                            hexes[x, y] = hex.gameObject.GetComponent<Hexagon>();
                            hexes[x, y].setX(x);
                            hexes[x, y].setY(y);
                            hexes[x, y].setMaterial(materials[k]);
                            hex.Rotate(0, 90, 0);
                            k++;
                            break;
                        }
                    }
                }
            }
        }
        producing = false;
    }

    private IEnumerator textDeactiveDelay(float delayTime, Text textToDeactive, string alarm) {
        textToDeactive.text = alarm;
        yield return new WaitForSeconds(delayTime);
        alarmText.gameObject.SetActive(false);
    }

    private void explodeHexagons(List<Hexagon> list)
    {
        exploding = true;
        //Removing explosive hexagons
        foreach (Hexagon hex in list)
        {
            hexes[hex.getX(), hex.getY()] = null;
            if (hex.gameObject.name.Contains("Bomb"))
            {
                bombIsOn = false;
                StartCoroutine(textDeactiveDelay(4f, alarmText, "Bomb has been defused!!"));
            }
            Destroy(hex.gameObject);
        }

        point += 15;
        pointText.text = "Points: " + point;
        moveText.text = "#Moves: " + move;
        exploding = false;
        reassignHexagons(list);
    }

    private List<Hexagon> checkExplosion(Hexagon[,] listToCheck)
    {
        List<Hexagon> neighbourList = new List<Hexagon>();
        List<Hexagon> explosiveList = new List<Hexagon>();
        Hexagon currentHexagon;
        Hexagon.NeighbourHexes currentNeighbours;
        Material currentColor;

        for (int y = 0; y < gridWidth; y++)
        {
            for (int x = 0; x < gridHeight; x++)
            {
                currentHexagon = listToCheck[x, y];
                currentColor = currentHexagon.getMaterial();


                currentNeighbours = currentHexagon.getSidehexes();

                //Realizing valid side haxagons which is stars from left of grid
                if (isValidHexagon(currentNeighbours.up)) neighbourList.Add(hexes[(int)currentNeighbours.up.x, (int)currentNeighbours.up.y]);
                else neighbourList.Add(null);

                if (isValidHexagon(currentNeighbours.upRight)) neighbourList.Add(hexes[(int)currentNeighbours.upRight.x, (int)currentNeighbours.upRight.y]);
                else neighbourList.Add(null);

                if (isValidHexagon(currentNeighbours.downRight)) neighbourList.Add(hexes[(int)currentNeighbours.downRight.x, (int)currentNeighbours.downRight.y]);
                else neighbourList.Add(null);

                //Detect 3 hexagons that are exist like triangle
                for (int k = 0; k < neighbourList.Count - 1; ++k)
                {
                    if (neighbourList[k] != null && neighbourList[k + 1] != null)
                    {
                        if (neighbourList[k].getMaterial() == currentColor && neighbourList[k + 1].getMaterial() == currentColor)
                        {
                            if (!explosiveList.Contains(neighbourList[k]))
                                explosiveList.Add(neighbourList[k]);
                            if (!explosiveList.Contains(neighbourList[k + 1]))
                                explosiveList.Add(neighbourList[k + 1]);
                            if (!explosiveList.Contains(currentHexagon))
                                explosiveList.Add(currentHexagon);
                        }
                    }
                }

                neighbourList.Clear();
            }
        }

        return explosiveList;
    }


    //This function is for realizing valid hexagon which is must be in grid area
    private bool isValidHexagon(Vector2 pos)
    {
        return pos.x >= zero && pos.x < gridHeight && pos.y >= zero && pos.y < gridWidth;
    }
}



