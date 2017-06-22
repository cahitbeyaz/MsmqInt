using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniBus.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniBus.Infrastructure.Tests
{
    [TestClass()]
    public class MsmqInstallerTests
    {
        [TestMethod()]
        public void InstallTest()
        {
            if (!MqInstaller.IsInstalled)
            {
                MqInstaller.Install();
            }
            Assert.IsTrue(MqInstaller.IsInstalled);
        }
    }
}