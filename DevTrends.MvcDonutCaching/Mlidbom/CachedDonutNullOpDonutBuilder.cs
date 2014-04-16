﻿using System;
using System.Linq;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary>
    /// Takes the place of a real donut builder when a cached donut needs to participate in the building of new donuts
    /// </summary>
    public class CachedDonutNullOpDonutBuilder : IDonutBuilder
    {
        private readonly IDonut _donut;
       
        public CachedDonutNullOpDonutBuilder(IDonut source)
        {
            _donut = source;
        }

        override public string ToString()
        {
            return string.Format("{0} -> {1}", _donut.ControllerAction, _donut.SortedOutputSegments.FirstOrDefault());
        }

        public void ResultExecuted(bool wasException)
        {
        }

        public IDonut GetDonut()
        {
            return _donut;
        }

        public string PrepareChildOutput(Guid childId, string childOutput)
        {
            return childOutput;
        }

        public void ChildResultExecuted(IDonut child)
        {
        }
    }
}
