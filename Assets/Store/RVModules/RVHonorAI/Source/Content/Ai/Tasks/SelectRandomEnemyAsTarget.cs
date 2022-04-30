// Created by Ronis Vision. All rights reserved
// 02.04.2021.

//using RVHonorAI.Source;
//using RVModules.RVSmartAI.Content.Code.AI.Tasks;
//
//namespace Honor_AI.Content.Tasks
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class SelectRandomEnemyAsTarget : AiAgentBaseTask
//    {
//        #region Not public methods
//
//        IRelationshipProvider relationshipProvider;
//        ITargetProvider targetProvider;
//        private IEnemiesProvider enemiesProvider;
//
//        protected override void OnContextUpdated()
//        {
//            base.OnContextUpdated();
//            relationshipProvider = ContextAs<IRelationshipProvider>();
//            targetProvider = ContextAs<ITargetProvider>();
//            enemiesProvider = ContextAs<IEnemiesProvider>();
//        }
//
//        protected override void Execute(float _deltaTime)
//        {
//            foreach (var nearbyObject in enemiesProvider.Enemies)
//            {
//                //if (!(nearbyObject is ITarget target)) continue;
//                //if (!(nearbyObject is ICharacter chara)) continue;
//                //if (!relationshipProvider.IsEnemy(chara)) continue;
//    
//                targetProvider.Target = chara;
//                break;
//            }
//        }
//
//        #endregion
//    }
//}

