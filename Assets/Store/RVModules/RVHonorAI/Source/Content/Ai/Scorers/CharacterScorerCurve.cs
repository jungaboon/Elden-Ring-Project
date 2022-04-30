// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer;
using RVModules.RVSmartAI.Content.AI.Scorers;

namespace RVHonorAI.Content.AI.Scorers
{
    public abstract class CharacterScorerCurve : CharacterScorerCurveBase, I<AiAgentScorerCurve>, I<CharacterScorer>
    {
    }
}