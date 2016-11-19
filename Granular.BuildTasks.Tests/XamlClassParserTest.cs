using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.BuildTasks.Tests
{
    public class TestElement : ContentControl
    {
        //
    }

    [TestClass]
    public class XamlClassParserTest
    {
        [TestMethod]
        public void XamlClassParserBasicTest()
        {
            string text = @"
            <ContentControl x:Class='Granular.BuildTasks.Tests.TestClass'
                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                xmlns:test='clr-namespace:Granular.BuildTasks.Tests'>
                <Control.Background>
                    <SolidColorBrush x:Name='brush1' Color='Red'/>
                </Control.Background>

                <ContentControl.Content>
                    <test:TestElement x:Name='content1'>
                        <FrameworkElement x:Name='content2'/>
                    </test:TestElement>
                </ContentControl.Content>
            </ContentControl>";

            XamlElement root = XamlParser.Parse(text);

            ClassDefinition classDefinition = XamlClassParser.Parse(root, new TestTypeParser());

            MemberDefinition[] expectedMembers = new[]
            {
                new MemberDefinition("brush1", "System.Windows.Media.SolidColorBrush"),
                new MemberDefinition("content1", "Granular.BuildTasks.Tests.TestElement"),
                new MemberDefinition("content2", "System.Windows.FrameworkElement")
            };

            Assert.AreEqual("System.Windows.Controls.ContentControl", classDefinition.BaseTypeName);
            Assert.AreEqual("TestClass", classDefinition.Name);
            Assert.AreEqual("Granular.BuildTasks.Tests", classDefinition.Namespace);
            CollectionAssert.AreEqual(expectedMembers, classDefinition.Members.ToArray());
        }
    }
}
