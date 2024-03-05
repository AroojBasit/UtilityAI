using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "UtilityAI/Considerations/Hunger Consideration")]
    public class HungerConsideration : Consideration
    {
        //Full = 0, Starving = 100
        //GameWorldData = x
        //ResponseCurve f(x) = a(x-b)^c + d
        //ConsiderationScore = f(x)
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.hunger / 100f));
            return score;
        }
    }
}
