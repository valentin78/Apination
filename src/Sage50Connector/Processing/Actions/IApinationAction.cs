﻿using Sage50Connector.API;

namespace Sage50Connector.Processing.Actions
{
    internal interface IApinationAction
    {
        void Execute(Sage50Api api, object payload);
    }
}