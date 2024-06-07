using System;
using Unity.VisualScripting;
using UnityEngine;
public class Timer
{ 
    private class TimersUpdater : MonoBehaviour
    {
        public Action action;
        private void Update()
        {
            if (action != null)
            {
                action();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private Func<bool> func;
    private float timer;
    private bool timerIsOver = false;


    public static Timer Create(float time, Func<bool> func)
    {
        Timer timer = new Timer(time, func);
        return timer;         
    }
    public static Timer Create(Func<bool> func)
    {
        Timer timer = new Timer(func);
        return timer;
    }

    private static TimersUpdater updater;
    public Timer(float timer, Func<bool> func)
    {
        this.func = func;
        this.timer = timer;
        TimersUpdater timersUpdater;
        if (updater == null)
        {
            timersUpdater = new GameObject("Updater", typeof(TimersUpdater)).GetComponent<TimersUpdater>();
            updater = timersUpdater;
        }
        else
        {
            timersUpdater = updater;
        }
        timersUpdater.action += Update;        
    }

    public Timer(Func<bool> func)
    {
        this.func = func;
        TimersUpdater timersUpdater;
        if (updater == null)
        {
            timersUpdater = new GameObject("Updater", typeof(TimersUpdater)).GetComponent<TimersUpdater>();
            updater = timersUpdater;
        }
        else
        {
            timersUpdater = updater;
        }
        timersUpdater.action += UpdateFunc;
    }

    public void UpdateFunc()
    {
        if(func())
        {
            updater.action -= UpdateFunc;
        }
    }

    public void Update()
    {
        if(!timerIsOver)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timerIsOver = true;
                func();
                updater.action -= Update;
            }
        }
    }

    public void UpdateAction()
    {

    }
    public void Cancel()
    {
        if(!timerIsOver)
        {
            timerIsOver = true;
            updater.action -= Update;
        }
    }

}
