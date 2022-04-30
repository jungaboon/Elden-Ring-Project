// Created by Ronis Vision. All rights reserved
// 25.01.2021.

namespace RVModules.RVSmartAI.Content.AI.DataProviders
{
    public abstract class StringProvider : DataProvider<string>
    {
        public static implicit operator string(StringProvider _stringProvider) => _stringProvider.GetData();

        public override string ToString() => GetData();
    }
}