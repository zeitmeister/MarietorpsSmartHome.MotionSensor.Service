using MarietorpsSmartHome.MotionSensor.Service.BackgroundServices;
using MarietorpsSmartHome.MotionSensor.Service.HelperFunctions;
using MarietorpsSmartHome.MotionSensor.Service.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MotionSensor.Tests
{
    public class Tests
    {
        public List<ThresholdModel> ThresholdModels { get; set; }
        public HelperFunctions HelperFunctions { get; set; }
        [SetUp]
        public void Setup()
        {
            HelperFunctions = new HelperFunctions();
            ThresholdModels = new List<ThresholdModel>();
        }

       
        [Test]
        public void ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
            
        }

        [Test]
        public void ShouldReturnFalseIfAllModelsAreSameTime()
        {
            var datetime = DateTime.Now;
            ThresholdModels.Add(new ThresholdModel { TimeStamp = datetime });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = datetime });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = datetime });
            Assert.IsFalse(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }

        [Test]
        public void ShouldReturnFalseIfTwoModelsAreSameTime()
        {
            var datetime = DateTime.Now;
            ThresholdModels.Add(new ThresholdModel { TimeStamp = datetime });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(1) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = datetime });
            Assert.IsFalse(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }

        [Test]
        public void ShouldReturnFalseIfAdding5MinToLastModel()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            Assert.IsFalse(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }

        [Test]
        public void ShouldReturnTrueIfAdding5MinToSecond()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });
            Assert.IsTrue(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }

        [Test]
        public void ShouldReturnTrueHappyPath()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(1) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(3) });
            Assert.IsTrue(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }

        [Test]
        public void ShouldReturnFalseIfFirstIsLaterThanThird()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(2) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddMinutes(5) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            Assert.IsFalse(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }
        [Test]
        public void ShouldReturnFalse()
        {
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddSeconds(2) });
            ThresholdModels.Add(new ThresholdModel { TimeStamp = DateTime.Now.AddSeconds(5) });
            Assert.IsFalse(HelperFunctions.DifferenceBetweenFirstAndThirdIsLessThan3Mins(ThresholdModels));
        }
    }
}
