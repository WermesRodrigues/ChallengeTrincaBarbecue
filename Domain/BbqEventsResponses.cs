using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class BbqEventsResponses
    {
        public BbqEventsResponses(string messageEvent, bool isOk, object snapshotObj)
        {
            MessageEvent = messageEvent;
            IsOk = isOk;
            SnapshotObj = snapshotObj;
        }

        public BbqEventsResponses(string messageEvent, bool isOk)
        {
            MessageEvent = messageEvent;
            IsOk = isOk;
        }

        public bool IsOk { get; set; }
        public object SnapshotObj { get; set; }
        public string MessageEvent { get; set; }
    }
}
