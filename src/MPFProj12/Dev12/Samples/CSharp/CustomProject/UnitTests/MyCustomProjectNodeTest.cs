/********************************************************************************************

Copyright (c) Microsoft Corporation 
All rights reserved. 

Microsoft Public License: 

This license governs use of the accompanying software. If you use the software, you 
accept this license. If you do not accept the license, do not use the software. 

1. Definitions 
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the 
same meaning here as under U.S. copyright law. 
A "contribution" is the original software, or any additions or changes to the software. 
A "contributor" is any person that distributes its contribution under this license. 
"Licensed patents" are a contributor's patent claims that read directly on its contribution. 

2. Grant of Rights 
(A) Copyright Grant- Subject to the terms of this license, including the license conditions 
and limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
royalty-free copyright license to reproduce its contribution, prepare derivative works of 
its contribution, and distribute its contribution or any derivative works that you create. 
(B) Patent Grant- Subject to the terms of this license, including the license conditions 
and limitations in section 3, each contributor grants you a non-exclusive, worldwide, 
royalty-free license under its licensed patents to make, have made, use, sell, offer for 
sale, import, and/or otherwise dispose of its contribution in the software or derivative 
works of the contribution in the software. 

3. Conditions and Limitations 
(A) No Trademark License- This license does not grant you rights to use any contributors' 
name, logo, or trademarks. 
(B) If you bring a patent claim against any contributor over patents that you claim are 
infringed by the software, your patent license from such contributor to the software ends 
automatically. 
(C) If you distribute any portion of the software, you must retain all copyright, patent, 
trademark, and attribution notices that are present in the software. 
(D) If you distribute any portion of the software in source code form, you may do so only 
under this license by including a complete copy of this license with your distribution. 
If you distribute any portion of the software in compiled or object code form, you may only 
do so under a license that complies with this license. 
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give 
no express warranties, guarantees or conditions. You may have additional consumer rights 
under your local laws which this license cannot change. To the extent permitted under your 
local laws, the contributors exclude the implied warranties of merchantability, fitness for 
a particular purpose and non-infringement.

********************************************************************************************/

using System;
using System.IO;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Project.Samples.CustomProject;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Project.Samples.CustomProject.UnitTests
{
    [TestClass]
    public class MyCustomProjectNodeTest : BaseTest
    {
        private CustomProjectPackage customProjectPackage;
        private MyCustomProjectFactory customProjectFactory;
        private MyCustomProjectNode projectNode;
        private static string templateFile = @"Program.cs";
        private static string targetFile = @"Program1.cs";

        [ClassInitialize]
        public static void TestClassInitialize(TestContext context)
        {
            projectFile = Path.Combine(context.TestDeploymentDir, projectFile);
            templateFile = Path.Combine(context.TestDeploymentDir, templateFile);
            targetFile = Path.Combine(context.TestDeploymentDir, targetFile);
        }

        [TestInitialize()]
        public override void Initialize()
        {
            base.Initialize();
            customProjectPackage = new CustomProjectPackage();
            ((IVsPackage)customProjectPackage).SetSite(serviceProvider);

            customProjectFactory = new MyCustomProjectFactory(customProjectPackage);

            base.SetMsbuildEngine(customProjectFactory);

            int canCreate;

            if(VSConstants.S_OK == ((IVsProjectFactory)customProjectFactory).CanCreateProject(projectFile, 2, out canCreate))
            {
                PrivateType type = new PrivateType(typeof(MyCustomProjectFactory));
                PrivateObject obj = new PrivateObject(customProjectFactory, type);
                projectNode = (MyCustomProjectNode)obj.Invoke("PreCreateForOuter", new object[] { IntPtr.Zero });

                Guid iidProject = new Guid();
                int pfCanceled;
                projectNode.Load(projectFile, "", "", 2, ref iidProject, out pfCanceled);
            }
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            MyCustomProjectNode customProject = new MyCustomProjectNode(customProjectPackage);
            Assert.IsNotNull(customProject, "Constructor failed");
        }

        [TestMethod()]
        public void ProjectGuidTest()
        {
            Guid actual = typeof(MyCustomProjectFactory).GUID;
            Guid expected = projectNode.ProjectGuid;

            Assert.AreEqual(expected, actual, "ProjectGuid was not set correctly.");
        }

        [TestMethod()]
        public void GetAutomationObjectTest()
        {
            Assert.IsNotNull(projectNode.GetAutomationObject(), "AutomationObject is null");
        }

        ////[TestMethod()]
        ////public void CreateFileNodeTest()
        ////{
        ////    ProjectElement element =
        ////        projectNode.GetProjectElement(new BuildItem("Compile", "AssemblyInfo.cs"));

        ////    Assert.IsNotNull(projectNode.CreateFileNode(element), "FileNode is null");
        ////}

        [TestMethod()]
        public void AddFileFromTemplateTest()
        {
            projectNode.AddFileFromTemplate(templateFile, targetFile);

            string content = File.ReadAllText(targetFile);

            Assert.IsTrue(content.IndexOf('%') == -1, "Parameter replacement did not occurred");
        }

        [TestMethod()]
        public void GetConfigurationIndependentPropertyPagesTest()
        {
            Guid[] expected = new Guid[] { typeof(GeneralPropertyPage).GUID };
            Guid[] actual;

            PrivateType type = new PrivateType(typeof(MyCustomProjectNode));
            PrivateObject obj = new PrivateObject(projectNode, type);
            actual = (Guid[])obj.Invoke("GetConfigurationIndependentPropertyPages", new object[] { });

            CollectionAssert.AreEqual(expected, actual, "IndependentPropertyPages not correctly set");
        }

        [TestMethod()]
        public void GetPriorityProjectDesignerPagesTest()
        {
            Guid[] expected = new Guid[] { typeof(GeneralPropertyPage).GUID };
            Guid[] actual;

            PrivateType type = new PrivateType(typeof(MyCustomProjectNode));
            PrivateObject obj = new PrivateObject(projectNode, type);
            actual = (Guid[])obj.Invoke("GetPriorityProjectDesignerPages", new object[] { });

            CollectionAssert.AreEqual(expected, actual, "DesignerPages not correctly set");
        }
    }
}
