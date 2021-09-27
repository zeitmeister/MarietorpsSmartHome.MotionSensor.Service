using MarietorpsSmartHome.MotionSensor.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarietorpsSmartHome.MotionSensor.Service.HelperFunctions
{
    public interface IHelperFunctions
    {
        bool ThresholdIsReached(List<ThresholdModel> thresholdModels);
        bool DifferenceBetweenFirstAndThirdIsLessThan3Mins(List<ThresholdModel> thresholdModels);
    }

    public class HelperFunctions : IHelperFunctions
    {
        public bool ThresholdIsReached(List<ThresholdModel> thresholdModels)
        {
            return thresholdModels.Count.Equals(3) && DifferenceBetweenFirstAndThirdIsLessThan3Mins(thresholdModels);

        }

        public virtual bool DifferenceBetweenFirstAndThirdIsLessThan3Mins(List<ThresholdModel> thresholdModels)
        {
            return (thresholdModels[2].TimeStamp - thresholdModels[0].TimeStamp).TotalMinutes <= 3.1
                && (thresholdModels[0].TimeStamp.Ticks != thresholdModels[2].TimeStamp.Ticks);
                //&& !((thresholdModels[2].TimeStamp - thresholdModels[0].TimeStamp).TotalMinutes <);
        }
    }
}
