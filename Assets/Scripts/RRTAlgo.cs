using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRTAlgo : MonoBehaviour
{
    float startX;
    float startY;

    public GameObject goal;

    float goalX;
    float goalY;

    GameObject[] obs;
    List<Collider> obCol;

    public GameObject floor;

    Tree tree;

    public float step;
    public int maxSteps;
    int curSteps;
    TreeNode finalNode;

    Stack<TreeNode> points;
    Vector3 curDest;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;

        goal = GameObject.FindWithTag("Exit");
        floor = GameObject.FindWithTag("floor");
        step = 1.5f;
        maxSteps = 10000;
        speed = 10.0f;
        obCol = new List<Collider>();

        obs = GameObject.FindGameObjectsWithTag("obstacles");
        
        int count = 0;
        foreach (GameObject ob in obs)
        {
            obCol.Add(ob.GetComponent<Collider>());
            print(count);
            count++;
        }

        tree = new Tree(transform.position);
        curSteps = 0;
        TreeNode curLeaf = null;
        
        while (curSteps < maxSteps)
        {
            curLeaf = NextLeaf();
            if (goal.GetComponent<Collider>().bounds.Contains(curLeaf.pos))
            {
                finalNode = curLeaf;
                break;
            }
            curSteps++;
        }

        if (curSteps >= maxSteps)
        {
            finalNode = tree.FindClosest(goal.transform.position, tree.root);
            Debug.DrawLine(curLeaf.pos, finalNode.pos, Color.white, 2.5f);

        }

        points = new Stack<TreeNode>();

        TreeNode cur = finalNode;
        while (cur != null)
        {
            points.Push(cur);
            cur = cur.parent;
        }

        curDest = points.Pop().pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        float dis = Vector3.Distance(transform.position, curDest);
        if (dis > .01f)
        {
            
            if (points.Count == 0)
            {
                //enable navmesh agent and get to the end
            }
            /*
            // decide the moveDis for this frame. 
            //(Mathf.Clamp limits the first value, to make sure if the distance between the player and the destination pos is short than you set,
            // it only need to move to the destination. So at that moment, the moveDis should set to the "dis".)
            //float moveDis = Mathf.Clamp(moveDisPerSec * Time.fixedDeltaTime, 0, dis);
            float moveDis = Mathf.Clamp(.5f * Time.fixedDeltaTime, 0, dis);

            //get the unit vector which means the move direction, and multiply by the move distance.
            Vector3 move = (curDest - transform.position).normalized * moveDis;
            transform.Translate(move);

            //float walk = speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, curDest, walk);
            transform.rotation = Quaternion.Slerp(transform.rotation,
 Quaternion.LookRotation(curDest - transform.position), 3 * Time.deltaTime);

            //code for following the player
            transform.position += transform.forward * speed * Time.deltaTime;*/

            float walk = speed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, curDest, walk);
            //print("still in this pos");

        }
        else if (points.Count > 0)
        {
            curDest = points.Pop().pos;
            //print("moving to next pos");
        }
    }

    TreeNode NextLeaf()
    {
        Vector3 x;
        MeshFilter m = floor.GetComponent<MeshFilter>();

        Vector3 min = m.mesh.bounds.min;
        Vector3 max = m.mesh.bounds.max;
        Vector3 nextStep;

        bool flag = false;

        while (true)
        {
            flag = false;
            x = floor.transform.position - new Vector3(Random.Range(min.x * 5, max.x * 5), floor.transform.position.y, Random.Range(min.z * 15, max.z * 15));

            TreeNode closest = tree.FindClosest(x, tree.root);

            //if (Vector3.Distance(FindWithTag("Exit").transform, closest.pos) < step)
            //    nextStep = Vector3.Lerp(closest.pos, x, step / (closest - x).Length());

            // nextStep = Vector3.Lerp(closest.pos, x,  step / (closest.pos - x).Length());
            nextStep = Vector3.Lerp(closest.pos, x, step / Vector3.Distance(closest.pos, x));

            

            foreach (Collider col in obCol)
                if (col.bounds.Contains(nextStep))
                    flag = true;

            if (flag)
            {
                Debug.DrawLine(closest.pos, nextStep, Color.red, 2.5f);
                continue;
            }

            else
            {
                Debug.DrawLine(closest.pos, nextStep, Color.white, 2.5f);
                TreeNode newLeaf = new TreeNode(new Vector3(nextStep.x, 0f, nextStep.z));
                closest.AddChild(newLeaf);
                return newLeaf;
            }
        }

        
    }

}


public class TreeNode
{
    public Vector3 pos;
    public TreeNode parent;
    public List<TreeNode> children;
    
    public TreeNode(Vector3 v)
    {
        parent = null;
        children = new List<TreeNode>();
        pos = v;
    }

    public void AddChild(TreeNode t)
    {
        t.parent = this;
        children.Add(t);
    }
}

public class Tree
{
    public TreeNode root;

    public Tree(Vector3 v)
    {
        root = new TreeNode(v);
    }

    public TreeNode FindClosest(Vector3 v, TreeNode t)
    {
        TreeNode close = t;
        float curDist = Vector3.Distance(v, t.pos);
        
        foreach (TreeNode c in t.children)
        {
            TreeNode temp = FindClosest(v, c);
            float tempDist = Vector3.Distance(temp.pos, v);

            if (tempDist < curDist)
            {
                curDist = tempDist;
                close = temp;
            }
        }

        return close;
    }
}
  
