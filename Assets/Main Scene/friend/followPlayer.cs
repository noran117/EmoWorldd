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


    bool introPlaying = true;

    private bool reacting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        StartCoroutine(IntroWave());

        talkAudio = GetComponent<AudioSource>();

    }

    void Update()
    {

        if (introPlaying) return;

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

        introPlaying = false;
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