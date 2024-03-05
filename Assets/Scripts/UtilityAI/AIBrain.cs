using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UI;

namespace TL.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        //Replace with UnityEvent or Delegate
        public bool finishedDeciding { get; set; }
        //Replace with UnityEvent or Delegate
        public bool finishedExecutingBestAction { get; set; }

        public Action bestAction { get; set; }
        private NPCController npc;

        [SerializeField] private Billboard billBoard;
        [SerializeField] private Action[] actionsAvailable;

        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<NPCController>();
            finishedDeciding = false;
            finishedExecutingBestAction = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Loop through all the available actions 
        // Give me the highest scoring action
        public void DecideBestAction()
        {
            finishedExecutingBestAction = false;

            float score = 0f;
            int nextBestActionIndex = 0;
            for (int i = 0; i < actionsAvailable.Length; i++)
            {
                //Debug.Log("Checking Action " + actionsAvailable[i].Name + ", Score: " + actionsAvailable[i].score);
                if (ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }

            bestAction = actionsAvailable[nextBestActionIndex];
            bestAction.SetRequiredDestination(npc);

            Debug.Log("Best Action: " + bestAction.Name);
            //Debug.Log("------------------------------------");
            finishedDeciding = true;
            billBoard.UpdateBestActionText(bestAction.Name);
        }

        //Loop through all the considerations of the action
        //Score all the considerations
        //Average the consideration scores ==> overall action score
        public float ScoreAction(Action action)
        {
            float score = 1f;

            float modFactor = 1 - (1 / action.considerations.Length);

            for (int i = 0; i < action.considerations.Length; i++)
            {
                //Averaging scheme of overall score
                //Behaviour Mathematics for game AI by Dave Mark
                float considerationScore = action.considerations[i].ScoreConsideration(npc);
                float makeUpValue = (1 - considerationScore) * modFactor;
                score *= considerationScore + (makeUpValue * considerationScore);

                if (score == 0)
                {
                    action.score = 0;
                    return action.score; //No point in computing further
                }
            }

            return action.score = score;
        }
    }
}
