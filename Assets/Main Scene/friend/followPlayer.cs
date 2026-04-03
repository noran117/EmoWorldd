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
    public GameObject startdanceMessage;
    bool isShocked = false;

    bool isHappy = false;



    public GameObject denielMessage1;
    public GameObject denielMessage2;
    public GameObject denielMessage3;


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





    /*
    public void PlayStoryHappy()
    {
        StartCoroutine(StoryHappyRoutine());
    }

    IEnumerator StoryHappyRoutine()
    {
        agent.isStopped = true;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        transform.rotation = Quaternion.LookRotation(dir);

        // 🎬 تشغيل Happy
        anim.SetBool("isStoryPlaying", true);

        yield return new WaitForSeconds(8f); // مدة القصة (عدليها حسب طول الصوت)

        // 🔚 رجوع للوضع الطبيعي
        anim.SetBool("isStoryPlaying", false);

        agent.isStopped = false;
    }
    */
    // 🎉 الرقص
    public void ReactToDance()
    {
        //if (!isSadScene) return;

        //agent.isStopped = true;

        //anim.SetTrigger("StartDance"); // 🔥 Trigger بدل Bool


        if (!isSadScene) return;

        agent.isStopped = true;

        isDancing = true; // 🔥 فعلنا الرقص

        anim.SetTrigger("StartDance");


        StartCoroutine(ShowMessageDelayed());
    }



    IEnumerator ShowMessageDelayed()
    {

        if (startdanceMessage != null)

            startdanceMessage.SetActive(true);

        yield return new WaitForSeconds(15f); // حسب طول الرقصة

        if (danceMessage != null)

            danceMessage.SetActive(true);

       // yield return new WaitForSeconds(3f);

        //danceMessage.SetActive(false);
    }
    public void StopDance()
    {
        //anim.SetTrigger("StopDance"); // 🔥 Trigger للإيقاف

        //agent.isStopped = false;
        anim.SetTrigger("StopDance");

        isDancing = false; // 🔥 وقفنا التتبع

        agent.isStopped = false;
    }

    // ⚡ الحادث
    //public void ReactToShock()
    //{
    //    if (!isSadScene) return;

    //    agent.isStopped = true;
    //    anim.SetTrigger("Shock");
    //}











    public void PlayGoGesture()
    {
        StartCoroutine(GoGestureRoutine());
    }

    IEnumerator GoGestureRoutine()
    {
        agent.isStopped = true;

        // 🔵 لف على اللاعب أولاً
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        yield return new WaitForSeconds(0.2f); // نعطيه لحظة يثبت اتجاهه

        // ✨ شغل الحركة
        anim.SetTrigger("Gesture");

        // 💬 الرسالة تبدأ مع الحركة
        if (goMessage != null)
            goMessage.SetActive(true);

        yield return new WaitForSeconds(3f); // مدة الحركة

        // ❌ اخفاء الرسالة
        if (goMessage != null)
            goMessage.SetActive(false);

        agent.isStopped = false;
    }





    /*
    public void PlayDenialTalk(string message)
    {
        if (player == null) return;

        // وقف الحركة
        agent.isStopped = true;

        // لف على اللاعب
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        // شغل التوكينج
        anim.SetBool("Talkingg", true);

        // عرض المسج
        if (talkMessage != null)
        {
            talkMessage.SetActive(true);
            talkMessage.GetComponentInChildren<TextMesh>().text = message;
        }

        // صوت (اختياري)
        if (talkAudio != null)
            talkAudio.Play();

        // وقف بعد وقت
        StartCoroutine(StopDenialTalk());
    }

    IEnumerator StopDenialTalk()
    {
        yield return new WaitForSeconds(3f);

        anim.SetBool("Talkingg", false);

        if (talkMessage != null)
            talkMessage.SetActive(false);

        agent.isStopped = false;
    }


    */








    public void PlayDenialSequence()
    {
        StartCoroutine(DenialSequenceRoutine());
    }

    //IEnumerator DenialSequenceRoutine()
    //{
    //    GameObject[] messages = new GameObject[]
    //    {
    //     denielMessage1,
    //    denielMessage2,
    //    denielMessage3
    //    };

    //    for (int i = 0; i < messages.Length; i++)
    //    {
    //        // 🛑 وقف الحركة
    //        agent.isStopped = true;

    //        // 🔄 لف على اللاعب
    //        if (player != null)
    //        {
    //            Vector3 dir = player.position - transform.position;
    //            dir.y = 0;
    //            transform.rotation = Quaternion.LookRotation(dir);
    //        }

    //        // 🎤 تشغيل التوكينج
    //        anim.SetBool("Talkingg", true);

    //        // 💬 إظهار المسج
    //        if (messages[i] != null)
    //            messages[i].SetActive(true);

    //        yield return new WaitForSeconds(3f);

    //        // ❌ إخفاء المسج
    //        if (messages[i] != null)
    //            messages[i].SetActive(false);

    //        // 📴 وقف التوكينج
    //        anim.SetBool("Talking", false);

    //        yield return new WaitForSeconds(2f); // هيك صار مجموعهم 5 ثواني
    //    }

    //    // 🔓 يرجع طبيعي
    //    agent.isStopped = false;
    //}





    //IEnumerator DenialSequenceRoutine()
    //{
    //    GameObject[] messages = new GameObject[]
    //    {
    //    denielMessage1,
    //    denielMessage2,
    //    denielMessage3
    //    };

    //    int index = 0;

    //    while (true) // 🔥 يضل يكرر طول المرحلة
    //    {
    //        yield return new WaitForSeconds(5f);

    //        // 🔄 لف على اللاعب (بدون ما يوقف)
    //        if (player != null)
    //        {
    //            Vector3 dir = player.position - transform.position;
    //            dir.y = 0;

    //            transform.rotation = Quaternion.LookRotation(dir);
    //        }

    //        // 💬 عرض المسج الحالي
    //        if (messages[index] != null)
    //            messages[index].SetActive(true);

    //        yield return new WaitForSeconds(2f);

    //        // ❌ اخفاء المسج
    //        if (messages[index] != null)
    //            messages[index].SetActive(false);

    //        // 🔁 ننتقل للمسج اللي بعده
    //        index++;

    //        if (index >= messages.Length)
    //            index = 0; // يرجع يعيد من الأول
    //    }
    //}

    IEnumerator DenialSequenceRoutine()
    {
        GameObject[] messages = new GameObject[]
        {
        denielMessage1,
        denielMessage2,
        denielMessage3
        };

        for (int i = 0; i < messages.Length; i++)
        {
            // ⏱️ استنى 5 ثواني قبل كل رسالة
            yield return new WaitForSeconds(5f);

            // 👀 لف على اللاعب
            if (player != null)
            {
                Vector3 dir = player.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.LookRotation(dir);
            }

            // 💬 إظهار الرسالة
            if (messages[i] != null)
                messages[i].SetActive(true);

            // ⏱️ تبقى ظاهرة 5 ثواني
            yield return new WaitForSeconds(5f);

            // ❌ إخفاء الرسالة
            if (messages[i] != null)
                messages[i].SetActive(false);

            // ⏱️ 10 ثواني بدون رسالة
            yield return new WaitForSeconds(10f);
        }

        // ✅ انتهى — ما رح يكرر
    }




























    public void ReactToShock()
    {
        if (!isSadScene) return;

        agent.isStopped = true;

        isDancing = false;
        isHappy = false;

        isShocked = true;

        anim.SetTrigger("Shock");

        StartCoroutine(StopShockAfterTime()); // 🔥 المهم
    }

    IEnumerator StopShockAfterTime()
    {
        yield return new WaitForSeconds(2.5f); // ⏱️ مدة الصدمة (عدليها)

        isShocked = false;

        agent.isStopped = false; // 🔥 يرجع يتبع اللاعب
    }
    public void PointLeftSequence(Vector3 leftTarget)
    {
        StartCoroutine(PointSequence(leftTarget));
    }

    IEnumerator PointSequence(Vector3 leftTarget)
    {
        agent.isStopped = true;

        // 🟢 1. لف على اللاعب
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0;

        transform.rotation = Quaternion.LookRotation(dirToPlayer);

        // 💬 الرسالة تظهر وهو بحكي معه
        if (goMessage != null)
            goMessage.SetActive(true);

        yield return new WaitForSeconds(2f); // مدة الكلام

        // 🔵 2. لف للشمال
        Vector3 dirLeft = leftTarget - transform.position;
        dirLeft.y = 0;

        transform.rotation = Quaternion.LookRotation(dirLeft);

        // 👉 3. حركة التأشير
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

    // ⏱️ يرجع يتحرك
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