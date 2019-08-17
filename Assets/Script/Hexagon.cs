using UnityEngine;

public class Hexagon : Helper {

    int x;
    int y;
    public Vector2 lerpRotatePosition;
    public Vector2 lerpDownPosition;
    public bool rotateLerp;
    public bool downLerp;
    public Material material;
    GridSystem gridSystem;



    public struct NeighbourHexes
    {
        public Vector2 up;
        public Vector2 upLeft;
        public Vector2 upRight;
        public Vector2 down;
        public Vector2 downLeft;
        public Vector2 downRight;
    }

    void Start() {
        gridSystem = GridSystem.instance;
    }

    void Update()
    {
        if (rotateLerp)
        {
            float newX = Mathf.Lerp(transform.position.x, lerpRotatePosition.x, Time.deltaTime * hexagonRotateConstant);
            float newY = Mathf.Lerp(transform.position.y, lerpRotatePosition.y, Time.deltaTime * hexagonRotateConstant);
            transform.position = new Vector2(newX, newY);


            if (Vector3.Distance(transform.position, lerpRotatePosition) < rotateThreshold)
            {
                transform.position = lerpRotatePosition;
                rotateLerp = false;
            }
        }

        if (downLerp)
        {
            float newX = Mathf.Lerp(transform.position.x, lerpDownPosition.x, Time.deltaTime * hexagonRotateConstant);
            float newY = Mathf.Lerp(transform.position.y, lerpDownPosition.y, Time.deltaTime * hexagonRotateConstant);
            transform.position = new Vector2(newX, -newY);


            if (Vector3.Distance(transform.position, lerpDownPosition) < rotateThreshold)
            {
                transform.position = lerpDownPosition;
                downLerp = false;
            }
        }
    }


    public void rotate(int newX, int newY, Vector2 newPos)
    {
        lerpRotatePosition = newPos;
        setX(newX);
        setY(newY);
        rotateLerp = true;
    }

    public void lerpDown(Vector2 newPos) {
        lerpDownPosition = newPos;
        downLerp = true;
    }

    public NeighbourHexes getSidehexes()
    {
        bool isEven = GridSystem.isEvenColumn(this.getY());

        NeighbourHexes neighbours;

            neighbours.down = new Vector2(x + 1, y);
            neighbours.up = new Vector2(x - 1, y);
            neighbours.downRight = new Vector2(isEven ? x : x + 1, isEven ? y - 1 : y - 1);//
            neighbours.upLeft = new Vector2(isEven ? x - 1 : x, isEven ? y + 1 : y + 1);//
            neighbours.upRight = new Vector2(isEven ? x - 1 : x, isEven ? y - 1 : y - 1);//
            neighbours.downLeft = new Vector2(isEven ? x : x + 1, isEven ? y + 1 : y + 1);//


        return neighbours;
    }


    //setters

    public void setX(int value) {
        x = value;
    }

    public void setY(int value) {
        y = value;
    }

    public void setMaterial(Material newMaterial) { GetComponent<MeshRenderer>().sharedMaterial = newMaterial; material = newMaterial; }

    public void setPosition(Vector3 position) {
        transform.position = new Vector2(position.x,position.y);
    }

    //getters

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public Material getMaterial() { return GetComponent<MeshRenderer>().sharedMaterial; }

}
