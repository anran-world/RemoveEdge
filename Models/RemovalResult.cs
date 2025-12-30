using System;
using System.Collections.Generic;

namespace RemoveMSEdgeGUI.Models
{
    public class RemovalResult
    {
        public bool Success { get; set; }
        public List<string> RemovedItems { get; set; }
        public List<string> Errors { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;

        public RemovalResult()
        {
            RemovedItems = new List<string>();
            Errors = new List<string>();
            StartTime = DateTime.Now;
        }

        public void AddRemovedItem(string item)
        {
            RemovedItems.Add(item);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void MarkAsComplete()
        {
            EndTime = DateTime.Now;
            Success = Errors.Count == 0;
        }
    }
}
