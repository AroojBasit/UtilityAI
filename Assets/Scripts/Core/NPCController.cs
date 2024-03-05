using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using System;
using System.Threading.Tasks;

namespace TL.Core
{
    public enum State
    {
        decide,
        move,
        execute
    }

    public class NPCController : MonoBehaviour
    {
        public MoveController mover { get; set; }
        public AIBrain aiBrain { get; set; }
        public NPCInventory Inventory { get; set; }
        public Stats stats { get; set; }

        public Context context;

        public State currentState { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            currentState = State.decide;
        }

        // Update is called once per frame
        void Update()
        {
            FSMTick();
        }

        public void FSMTick()
        {
            if (currentState == State.decide)
            {
                aiBrain.DecideBestAction();

                if (Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    currentState = State.move;
                }
            }
            else if (currentState == State.move)
            {
                float distance = Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position);
                if (distance < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    mover.MoveTo(aiBrain.bestAction.RequiredDestination.position);
                }
            }
            else if (currentState == State.execute)
            {
                if (aiBrain.finishedExecutingBestAction == false)
                {
                    aiBrain.bestAction.Execute(this);
                }
                else if (aiBrain.finishedExecutingBestAction == true)
                {
                    currentState = State.decide;
                }
            }
        }

        #region Workhorse methods

        //Replace with UnityEvent or Delegate
        public void OnFinishedAction()
        {
            aiBrain.DecideBestAction();
        }

        public bool AmIAtRestDestination()
        {
            return Vector3.Distance(this.transform.position, context.home.transform.position) <= context.MinDistance;
        }

        #endregion

        #region Async

        public async void DoWork(int time)
        {
            await Task.Delay(TimeSpan.FromSeconds(time));
            Inventory.AddResource(ResourceType.wood, 10);
            aiBrain.finishedExecutingBestAction = true;
        }

        public async void DoSleep(int time)
        {
            await Task.Delay(TimeSpan.FromSeconds(time));
            stats.energy += 5;
            aiBrain.finishedExecutingBestAction = true;
        }

        #endregion
    }
}