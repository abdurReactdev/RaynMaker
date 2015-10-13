﻿using Microsoft.Practices.Prism.PubSubEvents;

namespace RaynMaker.Infrastructure.Events
{
    /// <summary>
    /// Publishes the GUID of the deleted asset.
    /// </summary>
    public class AssetDeletedEvent : PubSubEvent<string>
    {
    }
}
