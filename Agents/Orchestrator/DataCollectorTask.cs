﻿using System;
using System.Threading.Tasks;

namespace AboutCleanCode.Orchestrator;

class DataCollectorTask : AbstractAgent
{
    public DataCollectorTask(ILogger logger)
        : base(logger)
    { }

    private void Process(IAgent sender, Guid jobId)
    {
        try
        {
            sender.Post(this, new TaskStartedEvent(jobId));

            // TODO: collect all necessary data which takes quite some time

            object payload = null; // TODO: carries the collected data

            sender.Post(this, new TaskCompletedEvent(jobId, payload));
        }
        catch (Exception exception)
        {
            sender.Post(this, new TaskFailedEvent(jobId, exception));
        }
    }

    protected override void OnReceive(IAgent sender, object message)
    {
        if (message is CollectDataCommand cdc)
        {
            Process(sender, cdc.JobId);
        }
        else
        {
            throw new NotSupportedException(message.GetType().FullName);
        }
    }
}