using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    public enum States { Idle, Forward, GoCurvy, Do }

    public NavMeshAgent agent;
    private Transform target;
    public States currentState;
    public bool isDoor1_opened;
    public GameObject box;
    public Animator animator;
    private NavMeshHit hit;
    public bool cantHandleTheBox;
    private GameObject player;
    private Rigidbody aiRigidbody;
    // protected CapsuleCollider capsuleCollider;
    public float distance;
    // private GameManager gameManager;

    //For Quadratic Bezier Curve
    public LineRenderer lineRenderer;
    public Transform point0, point1, point2;
    private int numPoints = 50;
    private Vector3[] positions = new Vector3[50];
    public bool a;
    private bool isLooking;
    public GameObject boxInTheHand;
    public GameObject FlyingBoxPrefab;
    public Transform hadoukenPoint;

    void Start()
    {
        lineRenderer.positionCount = numPoints;

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>(); animator = GetComponent<Animator>();
        }

        target = GameObject.Find("Target").GetComponent<Transform>(); ;
        player = GameObject.Find("ThirdPersonController");
        aiRigidbody = GetComponent<Rigidbody>();
        currentState = States.Idle;
        // gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        DrawQuadraticCurve();

        if (!isDoor1_opened)
        {
            isDoor1_opened = GameObject.Find("Door 1").GetComponent<DoorMove>().opened;
        }
        if (isDoor1_opened)
        {
            animator.SetBool("isDoorOpened", true);
        }

        UpdateStates();

        distance = Vector3.Distance(agent.transform.position, target.transform.position);

        if (isDoor1_opened && distance > 1)
        {
            currentState = States.Forward;
        }
        if (distance <= 0.5)
        {
            //currentState = States.Idle;
            //animator.SetFloat("Speed", 0);
            //target= GameObject.Find("Target(Table)");
            //animator.SetTrigger("Start");
            agent.destination = positions[0];

            a = true;
        }
        if (a == true)
        {
            currentState = States.GoCurvy;
        }
        if (Vector3.Distance(agent.transform.position, positions[numPoints - 1]) <= 1)
        {
            currentState = States.Do;
            //currentState = States.Idle;
        }

    }

    private void UpdateStates()
    {
        switch (currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Forward:
                Forward();
                break;
            case States.GoCurvy:
                GoCurvy();
                break;
            case States.Do:
                Do();
                break;
        }
    }

    private void Idle()
    {
        //animator.SetFloat("Speed", 0);
    }

    private void Forward()
    {
        animator.SetFloat("Speed", 1);
        transform.LookAt(target.transform.position);
        agent.SetDestination(target.transform.position);
    }

    private void GoCurvy()
    {
        animator.SetFloat("Speed", 1);
        QuadraticMove();
    }

    private void Do()
    {
        animator.SetBool("isDoorOpened", false);
        StartCoroutine(LookAtTheCharacter());
        if (isLooking)
            transform.LookAt(player.transform.position);
        animator.SetTrigger("Grab");
    }
    IEnumerator LookAtTheCharacter()
    {
        yield return new WaitForSeconds(3);
        isLooking = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            StartCoroutine(GetTheBox());
            StartCoroutine(ThrowTheBox());
        }
    }
    IEnumerator GetTheBox()
    {
        yield return new WaitForSeconds(1.2f);
        boxInTheHand.SetActive(true);
    }
    IEnumerator ThrowTheBox()
    {
        yield return new WaitForSeconds(4.5f);
        boxInTheHand.SetActive(false);
        Instantiate(FlyingBoxPrefab, hadoukenPoint);
    }


    //For Quadratic Bezier Curve

    private void QuadraticMove()
    {
        for (int i = 1; i < numPoints; i++)
        {
            if (Vector3.Distance(agent.transform.position, positions[i - 1]) <= 2)
            {
                agent.destination = positions[i];
            }
        }
    }

    private void DrawQuadraticCurve()
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float t = i / (float)numPoints;
            positions[i - 1] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, point2.position);
        }
        lineRenderer.SetPositions(positions);
    }
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        //B(t) = (1-t)2 P0 + 2(1-t)tP1 + t2P2 , 0 < t < 1
        //         uu           u        tt
        //         uu*p0   + 2 *u*t*p1 + tt*p2

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}