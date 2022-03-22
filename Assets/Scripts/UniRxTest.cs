using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class UniRxTest : MonoBehaviour
{
    public Subject<int> subjectTimer = new Subject<int>();

    public bool isPausse;
    void Start()
    {
        //Observable.EveryUpdate()
        //    .Where(_ => Input.GetMouseButtonDown(0))
        //    .Select(_ => Input.mousePosition)
        //    .Subscribe(pos => Debug.Log($"ClickMouse {pos}"));

        WaitCoroutineTest();
    }

    private void WaitCoroutineTest()
    {
        StartCoroutine(WaitCoroutine());
    }

    private IEnumerator WaitCoroutine()
    {
        Debug.Log("wait for 1 second");
        yield return Observable.Timer(TimeSpan.FromSeconds(1))
            .ToYieldInstruction();
        Debug.Log("press any key");
        yield return this.UpdateAsObservable()
            .FirstOrDefault(_ => Input.anyKeyDown)
            .ToYieldInstruction(); //FirstOrDefault: 조건 만족시 OnNext,OnCompleted를 모두 발행 .ToYieldInstruction();

        Debug.Log("pressed");

    }

    private void FromCoroutineTest()
    {
        Observable.FromCoroutine<long>(observer => MyCoroutine(observer))
                .Subscribe(
                x => Debug.Log($"OnNext {x}"),
                () => Debug.Log("OnCompleted")
                ).AddTo(gameObject);
    }

    private IEnumerator MyCoroutine(System.IObserver<long> observer)
    {
        Debug.Log("Start Coroutine");
        long count = 0;
        float deltaTime = 0;
        while (true)
        {
            if (!isPausse)
            {

                deltaTime += Time.deltaTime;
                if (deltaTime >= 1.0f)
                {
                    var intergerPart = Mathf.FloorToInt(deltaTime);
                    deltaTime -= intergerPart;
                    count += intergerPart;
                    observer.OnNext(count);
                    Debug.Log($"count {count}");
                }
            }
            yield return null;
        }
        //yield return new WaitForSeconds(3.0f);
        Debug.Log("End Coroutine");
    }

    private IEnumerator TimerTest()
    {
        int time = 1;
        while (time < 100)
        {
            subjectTimer.OnNext(time);
            //eventTimer(time);
            time++;
            yield return new WaitForSeconds(1);
        }
    }


    private void SubjectTest()
    {
        Subject<string> subject = new Subject<string>();
        subject.Subscribe(msg => Debug.Log($"Subscribe1 {msg}"));
        subject.Subscribe(msg => Debug.Log($"Subscribe2 {msg}"));
        subject.Subscribe(msg => Debug.Log($"Subscribe3 {msg}"));

        subject.OnNext("Hello");
        subject.OnNext("Hi");
    }
}
