using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class transformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}

public class NewVegetationGenerator : MonoBehaviour
{
    public GameObject MainPanel;

    public GameObject Branch;
    public GameObject Leaf;
    private GameObject[] PlantSegments;

    public float angle = 25;

    public int angleMutationValue = 5;

    public int iterations = 3;

    public float branchLength = 0.5f;

    public float originalBranchStartWidth = 0.1f;
    public float originalBranchEndWidth = 0.075f;

    private float BranchStartWidth = 0.1f;
    private float BranchEndWidth = 0.075f;

    public float BranchSrinkingValue = 0.75f;

    public Color32 branchColor = new Color32(53, 43, 25, 255);

    public float leafLength = 0.5f;

    public float leafStartWidth = 0.01f;
    public float leafMiddleWidth = 0.25f;

    public Color32 leafColor = new Color32(25, 53, 35, 255);

    public char Letter1 = 'B';
    public char Letter2 = 'L';
    public char Letter3 = '3';
    public char Letter4 = '4';
    public char Letter5 = '5';
    public char Letter6 = '6';

    public char StartingLetter = 'B';
    public char EndingLetter = 'L'; 

    public string Letter1Rules = "B[+B]B[-B]B"; // B[+BL]B[-BL]B = nice plant with leafs
    public string Letter2Rules = "L";
    public string Letter3Rules = "";
    public string Letter4Rules = "";
    public string Letter5Rules = "";
    public string Letter6Rules = "";

    public bool GeneratePlant = false;

    public int PlantSegmentsCreated = 0;
    public int seed = 0;

    public bool RandomizeSeed = false;

    private Stack<transformInfo> transformStack;
    private Dictionary<char, string> rules;
    public string currentString = string.Empty;


    float Up = 0;
    float Down = 0;
    float Right = 0;
    float Left = 0;


    void Start()
    {
        seed = UnityEngine.Random.Range(100000, 999999);
        UnityEngine.Random.InitState(seed);

        Branch = Resources.Load<GameObject>("Prefabs/Branch");
        Leaf = Resources.Load<GameObject>("Prefabs/Leaf");

        PlantSegments = new GameObject[100000];

        transformStack = new Stack<transformInfo>();

        rules = new Dictionary<char, string>
        {
            {Letter1, Letter1Rules },
            {Letter2, Letter2Rules },
            {Letter3, Letter3Rules },
            {Letter4, Letter4Rules },
            {Letter5, Letter5Rules },
            {Letter6, Letter6Rules }
        };
        
        Generate();
    }

    void FixedUpdate()
    {
        if (RandomizeSeed)
        {
            RandomizeSeed = false;

            seed = UnityEngine.Random.Range(100000, 999999);

            GeneratePlant = true;
        }

        UnityEngine.Random.InitState(seed);

        rules = new Dictionary<char, string>
        {
            {Letter1, Letter1Rules },
            {Letter2, Letter2Rules },
            {Letter3, Letter3Rules },
            {Letter4, Letter4Rules },
            {Letter5, Letter5Rules },
            {Letter6, Letter6Rules }
        };


        if (GeneratePlant)
        {
            Up = 0;
            Down = 0;
            Right = 0;
            Left = 0;

            GeneratePlant = false;

            BranchStartWidth = originalBranchStartWidth;
            BranchEndWidth = originalBranchEndWidth;

            Generate();
        }
    }

    private void Generate()
    {
        if (PlantSegmentsCreated > 0)
        {
            transform.position = new Vector3(0, 0, 0);
            transform.rotation = new Quaternion(0, 0, 0, 0);

            for (int i = 0; i < PlantSegmentsCreated; i++)
            {
                Destroy(PlantSegments[i]);
            }
            PlantSegmentsCreated = 0;
        }


        currentString = StartingLetter.ToString();

        StringBuilder sb = new StringBuilder();

        int iterationNum = 0;
        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());

                if (iterationNum != i)
                {
                    sb.Append('V');

                    iterationNum = i;
                }
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }


        if (EndingLetter != ' ')
        {
            //currentString = currentString.Replace("B]", "B" + EndingLetter + "]");

            string s1 = "" + StartingLetter + "]";
            string s2 = "" + StartingLetter + "" + EndingLetter + "]";

            currentString = currentString.Replace(s1, s2);

            //currentString = currentString + EndingLetter;
        }

        bool makeBranch = false;
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'B':
                    makeBranch = false;

                    if (transform.rotation.z == 0)
                    {
                        makeBranch = true;
                    }
                    else if (UnityEngine.Random.value > 0.25f)
                    {
                        makeBranch = true;
                    }

                    if (makeBranch)
                    {
                        Vector2 InitialPosition = transform.position;
                        transform.Translate(Vector2.up * branchLength);

                        if (PlantSegmentsCreated == PlantSegments.Length)
                        {
                            MainPanel.GetComponent<VegetationUI>().MakePartsErrorMessage();
                            MainPanel.GetComponent<VegetationUI>().ResetAll();
                            MainPanel.GetComponent<VegetationUI>().RandomizeGo();

                            break;
                        }

                        GameObject TreeSegment = Instantiate(Branch);

                        PlantSegments[PlantSegmentsCreated] = TreeSegment;
                        PlantSegmentsCreated++;

                        TreeSegment.GetComponent<LineRenderer>().SetPosition(0, InitialPosition);
                        TreeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);

                        TreeSegment.GetComponent<LineRenderer>().startWidth = BranchStartWidth * iterations / 2;
                        TreeSegment.GetComponent<LineRenderer>().endWidth = BranchEndWidth * iterations / 2;

                        TreeSegment.GetComponent<LineRenderer>().startColor = branchColor;
                        TreeSegment.GetComponent<LineRenderer>().endColor = branchColor;



                        if (transform.position.y > Up)
                        {
                            Up = transform.position.y;
                        }

                        if (transform.position.y < Down)
                        {
                            Down = transform.position.y;
                        }

                        if (transform.position.x > Right)
                        {
                            Right = transform.position.x;
                        }

                        if (transform.position.x < Left)
                        {
                            Left = transform.position.x;
                        }

                        TreeSegment.name += PlantSegmentsCreated;

                    }
                    break;

                case 'L':
                    if (makeBranch)
                    {
                        Vector2 InitialPosition1 = transform.position;
                        transform.Translate(Vector2.up * leafLength);

                        if (PlantSegmentsCreated == PlantSegments.Length - 1)
                        {
                            MainPanel.GetComponent<VegetationUI>().MakePartsErrorMessage();
                            MainPanel.GetComponent<VegetationUI>().ResetAll();
                            MainPanel.GetComponent<VegetationUI>().RandomizeGo();

                            break;
                        }

                        GameObject TreeSegment1 = Instantiate(Leaf);
                        GameObject TreeSegment2 = Instantiate(Leaf);

                        PlantSegments[PlantSegmentsCreated] = TreeSegment1;
                        PlantSegmentsCreated++;
                        PlantSegments[PlantSegmentsCreated] = TreeSegment2;
                        PlantSegmentsCreated++;

                        Vector3 Pos1 = new Vector3(InitialPosition1.x, InitialPosition1.y, -1);
                        Vector3 Pos2 = (InitialPosition1 * 3 + new Vector2(transform.position.x, transform.position.y)) / 4;
                        Pos2 = new Vector3(Pos2.x, Pos2.y, -1);
                        Vector3 Pos3 = new Vector3(transform.position.x, transform.position.y, -1);

                        TreeSegment1.GetComponent<LineRenderer>().SetPosition(0, Pos1);
                        TreeSegment1.GetComponent<LineRenderer>().SetPosition(1, Pos2);

                        TreeSegment1.GetComponent<LineRenderer>().startWidth = leafStartWidth * iterations / 2;
                        TreeSegment1.GetComponent<LineRenderer>().endWidth = leafMiddleWidth * iterations / 2;

                        TreeSegment1.GetComponent<LineRenderer>().startColor = leafColor;
                        TreeSegment1.GetComponent<LineRenderer>().endColor = leafColor;


                        TreeSegment2.GetComponent<LineRenderer>().SetPosition(0, Pos2);
                        TreeSegment2.GetComponent<LineRenderer>().SetPosition(1, Pos3);

                        TreeSegment2.GetComponent<LineRenderer>().startWidth = leafMiddleWidth * iterations / 2;
                        TreeSegment2.GetComponent<LineRenderer>().endWidth = 0;

                        TreeSegment2.GetComponent<LineRenderer>().startColor = leafColor;
                        TreeSegment2.GetComponent<LineRenderer>().endColor = leafColor;



                        if (transform.position.y > Up)
                        {
                            Up = transform.position.y;
                        }

                        if (transform.position.y < Down)
                        {
                            Down = transform.position.y;
                        }

                        if (transform.position.x > Right)
                        {
                            Right = transform.position.x;
                        }

                        if (transform.position.x < Left)
                        {
                            Left = transform.position.x;
                        }

                        TreeSegment1.name += PlantSegmentsCreated + "a";
                        TreeSegment1.name += PlantSegmentsCreated + "b";

                    }
                    break;

                case '+':
                    int mutationValue1 = UnityEngine.Random.Range(-angleMutationValue, angleMutationValue);
                    transform.Rotate(Vector3.forward * (angle + mutationValue1));
                    break;

                case '3':
                    break;

                case '4':
                    break;

                case '5':
                    break;

                case '6':
                    break;

                case '-':
                    int mutationValue2 = UnityEngine.Random.Range(-angleMutationValue, angleMutationValue);
                    transform.Rotate(Vector3.back * (angle + mutationValue2));
                    break;

                case '[':
                    transformStack.Push(new transformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    transformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                case 'V':
                    BranchStartWidth *= BranchSrinkingValue;
                    BranchEndWidth *= BranchSrinkingValue;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-Tree operation");
            }
        }



        //Camera.main.transform.position = new Vector3((Right + Left) / 2, (Up + Down) / 2, Camera.main.transform.position.z);

        Camera.main.transform.position = new Vector3((Right + Left) / 2, (Up + Down) / 2, Camera.main.transform.position.z);

        if (Up + Down > Right + Left)
        {
            Camera.main.orthographicSize = (Up + Down) / 2 * 1.25f;
        }
        else
        {
            Camera.main.orthographicSize = (Right + Left) / 2 * 2.25f;
        }





        MainPanel.GetComponent<VegetationUI>().GetVals = true;
    }
}
