using Microsoft.VisualStudio.TestTools.UnitTesting;
using HondaFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HondaFinder.entity;
using Moq;

namespace HondaFinder.Tests
{
    [TestClass()]
    public class VehicleDBTests
    {
        [TestMethod()]
        public void VehicleDB_GetAllAccords_ShouldBeOnlyVehicleInDatabase()
        {
            //arrange 
            var context = new Mock<HondaDBContext>();

            //act

            //assert
        }
    }
}