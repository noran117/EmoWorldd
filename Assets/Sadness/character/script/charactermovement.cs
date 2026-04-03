using UnityEngine;
using System.Collections;


public class charactermovement : MonoBehaviour
        {
    [Header("External Control")]
    public bool startDisabled = true;
    private bool canStart = false;

    [Header("Run Path")]
    public Transform[] runPoints;
    public float runSpeed = 4f;

    bool startRunning = false;
    int runIndex = 0;


    bool playedCountingSound = false;


    public GameObject basket;
    public GameObject butterflies;

    public AudioSource voiceSource;
    public AudioClip walkingVoice;
    public AudioClip danceInviteVoice;
    public AudioClip danceVoice;
    public AudioClip hellovoice;
    public AudioClip readyvoice;
    public AudioClip basketvoice;
   public AudioClip countingvoice;


    public Transform[] points;
    public float speed = 2f;
    public float rotationSpeed = 180f;
    public float startDelay = 3f;
    public Animator animator;

    int index = 0;
    bool startWalking = false;
    bool greetedMid = false;

    bool useRootMotion = false;


    public followPlayer companion;//

    void Start()
    {
        animator.applyRootMotion = false;
        animator.SetBool("isWalking", false);

        if (!startDisabled)
        {
            StartCoroutine(StartAfterDelay());
        }
    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);

        useRootMotion = false;
        animator.applyRootMotion = false;

        startWalking = true;
        animator.SetBool("isWalking", true);
    }

    public void BeginSequence()
    {
        if (canStart) return;

        canStart = true;
        StartCoroutine(StartAfterDelay());
    }
    void Update()
    {

        if (!canStart) return;
        AnimatorStateInfo state1 = animator.GetCurrentAnimatorStateInfo(0);

        if (state1.IsName("Counting") && !playedCountingSound)
        {
            voiceSource.PlayOneShot(countingvoice); 
            playedCountingSound = true;
        }

        if (!state1.IsName("Counting"))
        {
            playedCountingSound = false;
        }



        if (startRunning)
        {
            RunAlongPath();
            return;
        }

        if (!startWalking) return;
        if (index >= points.Length) return;

        Transform target = points[index];
        Vector3 direction = (target.position - transform.position);
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (distance < 0.2f)
        {
            if (index == 3 && !greetedMid)
            {
                StartCoroutine(MidGreeting());
                greetedMid = true;
                return;
            }

            index++;

            if (index >= points.Length)
            {
                startWalking = false;
                animator.SetBool("isWalking", false);

                if (voiceSource && hellovoice)
                    voiceSource.PlayOneShot(hellovoice);

                animator.SetTrigger("doCheer");
                StartCoroutine(StartTalkingAfterCheer());
                return;
            }
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
    }
    void RunAlongPath()
    {
            if (runIndex >= runPoints.Length)
            {
                startRunning = false;
                animator.SetBool("isRunning", false);
                animator.Play("Standing Idle", 0, 0f);

                return;
            }

            Transform target = runPoints[runIndex];
            Vector3 direction = (target.position - transform.position);
            float distance = direction.magnitude;

            if (distance > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                transform.position += transform.forward * runSpeed * Time.deltaTime;
            }

            if (distance < 0.2f)
            {
                runIndex++;
            }
        }
    IEnumerator MidGreeting()
    {
        startWalking = false;
        animator.SetBool("isWalking", false);

        if (voiceSource && walkingVoice)
            voiceSource.PlayOneShot(walkingVoice);

        animator.SetTrigger("doCheer");


        // 🔥 خلي الرفيق يعمل Happy
        if (companion != null)
            companion.ReactHappy();//

        yield return null;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Cheering"));

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f);

        startWalking = true;
        animator.SetBool("isWalking", true);




    }

    IEnumerator StartTalkingAfterCheer()
    {
        yield return new WaitForSeconds(2.5f);

        if (voiceSource && danceInviteVoice)
            voiceSource.PlayOneShot(danceInviteVoice);
        animator.SetBool("isTalking", true);

        yield return new WaitForSeconds(2.5f);

        animator.SetBool("isTalking", false);
        StartCoroutine(StartThumbsUp());
    }

    IEnumerator StartThumbsUp()
    {
        if (voiceSource && readyvoice)
            voiceSource.PlayOneShot(readyvoice);
        animator.SetBool("isThumbsUp", true);

        yield return new WaitForSeconds(2f); 
       
        animator.SetBool("isThumbsUp", false);

        StartCoroutine(StartDance());
    }



    IEnumerator StartDance()
    {
        animator.SetBool("startDance", true);



        if (companion != null)
            companion.ReactToDance();//
        yield return null;

        useRootMotion = true;
        animator.applyRootMotion = true;
        if (voiceSource && danceVoice)
            voiceSource.PlayOneShot(danceVoice);




        
        StartCoroutine(StopDanceAfterFinish()); 

        yield return new WaitForSeconds(20f);


        butterflies.SetActive(true);



       

    }
    IEnumerator StopDanceAfterFinish()
    {
        yield return null;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Gangnam Style"));

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);


        useRootMotion = true;
        animator.applyRootMotion = true;

        animator.SetBool("startDance", false);

        if (companion != null)
            companion.StopDance();//

        butterflies.SetActive(false);

        BasketManager.Instance.ShowBasket();



        if (voiceSource &&basketvoice)
            voiceSource.PlayOneShot(basketvoice);
        animator.SetBool("doButtonPush", true);

        yield return null; 
        animator.SetBool("doButtonPush", false);

        yield return new WaitForSeconds(.03f);
        animator.SetTrigger("doTurnLeft");
        yield return new WaitForSeconds(1.2f);
        animator.SetTrigger("doPoint");

        //if (companion != null)
        //{
        //    Vector3 leftDir = transform.position - transform.right * 5f; // 👈 شمال

        //    companion.PointLeftSequence(leftDir);
        //}
        yield return new WaitUntil(() =>
    animator.GetCurrentAnimatorStateInfo(0).IsName("Counting") &&
    animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f
);


        if (companion != null)
        {
            companion.PlayGoGesture();
        }

        useRootMotion = false;
        animator.applyRootMotion = false;

        animator.SetBool("isRunning", true);

        StartRunning();
    }
    void OnAnimatorMove()
    {
        if (useRootMotion)
        {
            transform.position += animator.deltaPosition;
            transform.rotation *= animator.deltaRotation;
        }
    }
    void StartRunning()
    {
        runIndex = 0;
        startRunning = true;
        animator.SetBool("isRunning", true);
    }

}

