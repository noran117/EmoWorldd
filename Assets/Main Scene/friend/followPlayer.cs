using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class followPlayer : MonoBehaviour
{
    public Transform player;
    public float sideOffset = 1.5f;
    public float forwardOffset = 0.5f;

    private NavMeshAgent agent;
    private Animator anim;

    private AudioSource talkAudio;


    public GameObject waveMessage;
    public GameObject talkMessage;

    public GameObject goMessage;

    bool introPlaying = true;

    private bool reacting = false;



    public bool isSadScene = false;

    private bool isDancing = false;

    public GameObject happyMessage;
    public GameObject danceMessage;
    public GameObject waitMessage;
    public GameObject acceptancemessage;

    public GameObject startdanceMessage;
    bool isShocked = false;

    bool isHappy = false;



    public GameObject denielMessage1;
    public GameObject denielMessage2;
    public GameObject denielMessage3;






    bool canFollow = true;


    bool isTalkingState = false;//

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        StartCoroutine(IntroWave());

        talkAudio = GetComponent<AudioSource>();

        if (isSadScene)
        {
            anim.Play("Sad Idle");
        }

    }

    void Update()
    {

        if (!isTalkingState)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }











        if (isShocked)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
        }

        if (isHappy)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 5f
            );
        }

        if (isDancing)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 5f
            );
        }

        // if (introPlaying) return;

        if (introPlaying || !canFollow) return;

        if (player == null) return;

        if (reacting)
        {
            float dist = Vector3.Distance(player.position, transform.position);

            if (dist > 2f)
            {
                reacting = false;
                agent.isStopped = false;

              //  anim.ResetTrigger("Surprise");

            }
            else
            {
                anim.SetFloat("Speed", 0);
                return;
            }
        }

        Vector3 desiredPosition =
            player.position +
            player.right * sideOffset +
            player.forward * forwardOffset;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(desiredPosition, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (player == null) return;

        Vector3 lookPos = player.position - player.right * 12f;

        anim.SetLookAtWeight(0.5f, 0.2f, 0.4f, 0.7f, 0.5f);
        anim.SetLookAtPosition(lookPos);
    }

    public void SetCheer(bool value)
    {
        anim.SetBool("IsCheering", value);
    }

    public void ReactToShell()
    {
        reacting = true;

        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir);

        anim.SetTrigger("Surprise");
    }

    public void ReactHappy()
    {
        agent.isStopped = true;

        anim.SetTrigger("Cheer"); 
        isHappy = true; 

        if (happyMessage != null)
            happyMessage.SetActive(true);

        StartCoroutine(StopHappy());
    }

    IEnumerator StopHappy()
    {
        yield return new WaitForSeconds(4f);

        if (happyMessage != null)
            happyMessage.SetActive(false);
        isHappy = false; 

        agent.isStopped = false;
    }





    public void StopFollowing()
    {
        canFollow = false;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        anim.SetFloat("Speed", 0);
    }



    public void ResumeFollowing()
    {
        canFollow = true;

        agent.isStopped = false;
    }





    public void PlayAcceptanceMoment()
    {
        StartCoroutine(AcceptanceMomentRoutine());
    }



    IEnumerator AcceptanceMomentRoutine()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        anim.SetTrigger("Grateful");

        yield return new WaitForSeconds(5f);

        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if (acceptancemessage != null)
            acceptancemessage.SetActive(true);
    }























    public void ReactToDance()
    {
        //if (!isSadScene) return;

        //agent.isStopped = true;

        //anim.SetTrigger("StartDance");


        if (!isSadScene) return;

        agent.isStopped = true;

        isDancing = true; 

        anim.SetTrigger("StartDance");


        StartCoroutine(ShowMessageDelayed());
    }



    IEnumerator ShowMessageDelayed()
    {

        if (startdanceMessage != null)

            startdanceMessage.SetActive(true);

        yield return new WaitForSeconds(15f); 
        if (danceMessage != null)

            danceMessage.SetActive(true);

       // yield return new WaitForSeconds(3f);

        //danceMessage.SetActive(false);
    }
    public void StopDance()
    {
        //anim.SetTrigger("StopDance"); 

        //agent.isStopped = false;
        anim.SetTrigger("StopDance");

        isDancing = false; 

        agent.isStopped = false;
    }

   




    public void PlayGoGesture()
    {
        StartCoroutine(GoGestureRoutine());
    }

    IEnumerator GoGestureRoutine()
    {
        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        yield return new WaitForSeconds(0.2f); 

        anim.SetTrigger("Gesture");

        if (goMessage != null)
            goMessage.SetActive(true);

        yield return new WaitForSeconds(3f); 

        if (goMessage != null)
            goMessage.SetActive(false);

        agent.isStopped = false;
    }





   







    public void PlayDenialSequence()
    {
        StartCoroutine(DenialSequenceRoutine());
    }

    IEnumerator DenialSequenceRoutine()
    {
        GameObject[] messages = new GameObject[]
        {
        denielMessage1,
        denielMessage2
        };

        for (int i = 0; i < messages.Length; i++)
        {
            yield return new WaitForSeconds(5f);

            agent.isStopped = true;

            if (player != null)
            {
                Vector3 dir = player.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
            }

            anim.SetFloat("Speed", 0); 
            anim.SetTrigger("StartTalking");

            if (messages[i] != null)
                messages[i].SetActive(true);

            yield return new WaitForSeconds(5f);

            if (messages[i] != null)
                messages[i].SetActive(false);

            agent.isStopped = false;

            if (i < messages.Length - 1)
            {
                yield return new WaitForSeconds(5f);
            }
        }
    }









    



























    public void ReactToShock()
    {
        if (!isSadScene) return;

        agent.isStopped = true;

        isDancing = false;
        isHappy = false;

        isShocked = true;

        anim.SetTrigger("Shock");

        StartCoroutine(StopShockAfterTime()); 
    }

    IEnumerator StopShockAfterTime()
    {
        yield return new WaitForSeconds(2.5f); 

        isShocked = false;

        agent.isStopped = false;
    }
    public void PointLeftSequence(Vector3 leftTarget)
    {
        StartCoroutine(PointSequence(leftTarget));
    }

    IEnumerator PointSequence(Vector3 leftTarget)
    {
        agent.isStopped = true;

        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0;

        transform.rotation = Quaternion.LookRotation(dirToPlayer);

        if (goMessage != null)
            goMessage.SetActive(true);

        yield return new WaitForSeconds(2f); 

        Vector3 dirLeft = leftTarget - transform.position;
        dirLeft.y = 0;

        transform.rotation = Quaternion.LookRotation(dirLeft);

        anim.SetTrigger("Point");

        yield return new WaitForSeconds(2.5f);

        if (goMessage != null)
            goMessage.SetActive(false);

        agent.isStopped = false;
    }

    IEnumerator StopPointing()
    {
        yield return new WaitForSeconds(5f);

        if (goMessage != null)
            goMessage.SetActive(false);

        agent.isStopped = false;
    }

    IEnumerator ResumeAfterAction(float time)
    {
        yield return new WaitForSeconds(time);
        agent.isStopped = false;
    }
    IEnumerator IntroWave()
    {
        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir);

        yield return new WaitForSeconds(5f);

        waveMessage.SetActive(true);

        anim.SetTrigger("Wave");

        yield return new WaitForSeconds(5f);

        waveMessage.SetActive(false);

        // introPlaying = false;
        // agent.isStopped = false;


        if (isSadScene)
        {
            StartCoroutine(IntroHappyAfterWave());
        }
        else
        {
            introPlaying = false;
            agent.isStopped = false;






        }
    }




   




    IEnumerator IntroHappyAfterWave()
    {
        introPlaying = false;

        agent.isStopped = false;

        yield return new WaitForSeconds(3f);

        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        isHappy = true;

        anim.SetTrigger("start");

        if (waitMessage != null)
            waitMessage.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (waitMessage != null)
            waitMessage.SetActive(false);

        isHappy = false;

        agent.isStopped = false;
    }






















    public void ExplainGates(Transform player)
    {
        StartCoroutine(TalkRoutine(player));
    }

    IEnumerator TalkRoutine(Transform player)
    {
        
        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir);

        talkMessage.SetActive(true);

        anim.SetBool("Talking", true);
        if (talkAudio != null)
            talkAudio.Play();


        yield return new WaitForSeconds(5f);

        anim.SetBool("Talking", false);

        talkMessage.SetActive(false);


        agent.isStopped = false;
    }
}