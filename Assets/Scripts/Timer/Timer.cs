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

    private Action action;
    private float timer;
    private bool timerIsOver = false;

    public static Timer Create(float time, Action action)
    {
        Timer timer = new Timer(time, action);
        return timer;         
    }

    private static TimersUpdater updater;
    public Timer(float timer,Action action)
    {
        this.action = action;
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
    public void Update()
    {
        if(!timerIsOver)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timerIsOver = true;
                action();
                updater.action -= Update;
            }
        }
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
