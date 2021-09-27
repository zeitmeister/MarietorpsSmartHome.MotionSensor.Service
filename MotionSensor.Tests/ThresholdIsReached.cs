using MarietorpsSmartHome.MotionSensor.Service.HelperFunctions;
using MarietorpsSmartHome.MotionSensor.Service.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionSensor.Tests
{
    public class ThresholdIsReached
    {
        public List<ThresholdModel> ThresholdModels { get; set; }
        public IHelperFunctions HelperFunctions { get; set; }

        [SetUp]
        public void Setup()
        {
            ThresholdModels = new List<ThresholdModel>();
            HelperFunctions = new HelperFunctions();
        }

        [Test]
        public void ShouldReturnTrueIfAllChecksOut()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });
            
            Assert.IsTrue(HelperFunctions.ThresholdIsReached(ThresholdModels));
            
            

        }

        [Test]
        public void ShouldReturnFalseIfOnlyTwoModels()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });

            Assert.IsFalse(HelperFunctions.ThresholdIsReached(ThresholdModels));
        }

        [Test]
        public void ShouldReturnFalseIfMoreThanThreeModels()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });

            Assert.IsFalse(HelperFunctions.ThresholdIsReached(ThresholdModels));
        }
    }
}
