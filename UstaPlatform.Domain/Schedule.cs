using System;
using System.Collections.Generic;
using System.Linq;

namespace UstaPlatform.Domain
{
    public class Schedule
    {
        private readonly Dictionary<DateOnly, List<WorkOrder>> _workOrders = new();

        public List<WorkOrder> this[DateOnly day]
        {
            get
            {
                if (!_workOrders.ContainsKey(day))
                {
                    _workOrders[day] = new List<WorkOrder>();
                }
                return _workOrders[day];
            }
        }
        public void AddWorkOrder(WorkOrder workOrder, DateOnly day)
        {
            this[day].Add(workOrder);
        }
    }
}