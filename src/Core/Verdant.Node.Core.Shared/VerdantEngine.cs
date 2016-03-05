using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using System.Threading;
using System.Collections;

namespace Verdant.Node.Core
{
    public class VerdantEngine : IVerdantEngine
    {
        private IDriver[] _drivers;
        private IAgent[] _agents;
        private Hashtable _allAgents = new Hashtable();
        private ArrayList _queue = new ArrayList();
        private ArrayList _mergeList = new ArrayList();
        private AutoResetEvent _scheduleChangedEvent = new AutoResetEvent(false);
        private bool _fShutdown = false;

        public VerdantEngine()
        {
        }

        public void Initialize()
        {
            var driverFactory = (IDriverFactory)DiContainer.Instance.Resolve(typeof(IDriverFactory));
            var primary = driverFactory.CreatePrimaryDrivers();
            foreach (var driver in primary)
            {
                try
                {
                    driver.Start();
                }
                catch (Exception exDriverStart)
                {
                    Debug.Print("Exception during agent start : " + exDriverStart);
                }
            }

            // initialize the settings subsystem
            var settings = (ISettingsProvider)DiContainer.Instance.Resolve(typeof(ISettingsProvider));
            settings.Initialize();

            settings.BootCount += 1;
            settings.CommitChanges();

            var secondary = driverFactory.CreateSecondaryDrivers();
            foreach (var driver in secondary)
            {
                try
                {
                    driver.Start();
                }
                catch (Exception exDriverStart)
                {
                    Debug.Print("Exception during agent start : " + exDriverStart);
                }
            }
            _drivers = new IDriver[primary.Length + secondary.Length];
            primary.CopyTo(_drivers, 0);
            secondary.CopyTo(_drivers, primary.Length);

            var agentFactory = (IAgentFactory)DiContainer.Instance.Resolve(typeof(IAgentFactory));
            _agents = agentFactory.CreateAgentsForState(EngineStates.Startup);
            foreach (var agent in _agents)
            {
                try
                {
                    var firstRun = agent.Start();
                    if (firstRun != DateTime.MaxValue)
                        this.ScheduleNextRun(agent, firstRun);
                }
                catch (Exception exAgentStart)
                {
                    Debug.Print("Exception during agent start : " + exAgentStart);
                }
            }
        }

        public void ScheduleNextRun(IAgent agent, DateTime runAt)
        {
            ScheduleNextRun(_queue, agent, runAt);
            _scheduleChangedEvent.Set();
        }

        private void ScheduleNextRun(ArrayList list, IAgent agent, DateTime runAt)
        {
            ScheduleItem item = null;

            // dequeue the agent if it is already in the schedule
            for (int i = 0; i < list.Count; ++i)
            {
                if (((ScheduleItem)list[i]).Target == agent)
                {
                    item = (ScheduleItem)list[i];
                    list.RemoveAt(i);
                    item.RunAt = runAt;
                    break;
                }
            }

            if (item == null)
            {
                item = new ScheduleItem(agent, runAt);
            }

            ScheduleNextRun(list, item);
        }

        private void ScheduleNextRun(ArrayList list, ScheduleItem item)
        {
            bool fAdded = false;
            for (int i = 0; i < list.Count; ++i)
            {
                if (((ScheduleItem)list[i]).RunAt > item.RunAt)
                {
                    list.Insert(i, item);
                    fAdded = true;
                    break;
                }
            }
            if (!fAdded)
                list.Add(item);
        }

        private void Merge()
        {
            foreach (var item in _mergeList)
            {
                ScheduleNextRun(_queue, (ScheduleItem)item);
            }
            _mergeList.Clear();
        }

        public void Run()
        {
            while (true)
            {
                if (_queue.Count == 0)
                {
                    // The queue is empty - wait for something to get inserted
                    _scheduleChangedEvent.WaitOne();
                }
                else
                {
                    var nextTime = ((ScheduleItem)_queue[0]).RunAt;
                    int delay = TimeSpan.FromTicks(nextTime.Ticks - DateTime.UtcNow.Ticks).Milliseconds;
                    if (delay > 0)
                        _scheduleChangedEvent.WaitOne(delay, false);
                }

                if (_fShutdown)
                    break;

                // Re-scheduled items get put in a side list (the mergeList) so that re-insertions with an immediate
                //   execution time don't monopolize the schedule. All 'ready' agents get run before re-scheduled agents
                //   even if the re-scheduled time would have passed.
                do
                {
                    DateTime now = DateTime.UtcNow;

                    if (_queue.Count == 0)
                        break;

                    // Examine the head item
                    var item = (ScheduleItem)_queue[0];

                    if (item.RunAt > now)
                        break; // we need to wait a bit

                    try
                    {
                        // Dequeue the head item
                        _queue.RemoveAt(0);
                        // Process it
                        var runAt = item.Target.Process(now);
                        // Re-schedule it in the merge list
                        if (runAt != DateTime.MaxValue)
                            ScheduleNextRun(_mergeList, item.Target, runAt);
                    }
                    catch (Exception exRun)
                    {
                        Debug.Print("An exception ocurred while attempting to run an agent : " + exRun.ToString());

                        // Attempt to recover the agent
                        try
                        {
                            item.Target.Stop();
                            item.RunAt = item.Target.Start();
                            if (item.RunAt != DateTime.MaxValue)
                                this.ScheduleNextRun(_mergeList, item);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print("Exception while trying to recover a faulted agent : " + ex.ToString());
                        }
                    }

                } while (!_fShutdown);

                if (_mergeList.Count > 0)
                    Merge();
            }

            // Execute shutdown code
            foreach (var agent in _agents)
            {
                try
                {
                    agent.Stop();
                }
                catch (Exception exAgent)
                {
                    Debug.Print("Exception during agent stop : " + exAgent);
                }
            }

            foreach (var driver in _drivers)
            {
                try
                {
                    driver.Stop();
                }
                catch (Exception exDriver)
                {
                    Debug.Print("Exception during agent start : " + exDriver);
                }
            }
        }

        public void Stop()
        {
            _fShutdown = true;
            _scheduleChangedEvent.Set();
        }

        public void NavigateToEngineState(string newState)
        {
            //TODO: send a message to the run process that it shoudld shut down running agents and reconfigure to the new state. Use a synthetic/placeholder agent, defined in .Core, for this purpose
            throw new NotImplementedException();
        }

        private class ScheduleItem
        {
            public ScheduleItem(IAgent target, DateTime runAt)
            {
                this.Target = target;
                this.RunAt = runAt;
            }

            public DateTime RunAt { get; set; }
            public IAgent Target { get; private set; }
        }
    }
}
