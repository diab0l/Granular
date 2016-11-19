using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Tests.Controls
{
    [TestClass]
    public class ItemsControlTest
    {
        [TestMethod]
        public void ItemsControlBasicTest()
        {
            string text = @"
            <ItemsControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <ItemsControl.Template>
                    <ControlTemplate>
                        <ItemsPresenter ItemContainerGenerator='{TemplateBinding ItemsControl.ItemContainerGenerator}' Template='{TemplateBinding ItemsControl.ItemsPanel}'/>
                    </ControlTemplate>
                </ItemsControl.Template>
            </ItemsControl>";

            ItemsControl itemsControl = XamlLoader.Load(XamlParser.Parse(text)) as ItemsControl;
            itemsControl.Items.Add("item1");
            itemsControl.Items.Add("item2");
            itemsControl.Items.Add("item3");

            ItemsPresenter itemsPresenter = itemsControl.VisualChildren.FirstOrDefault() as ItemsPresenter;
            Assert.IsNotNull(itemsPresenter);

            StackPanel stackPanel = itemsPresenter.VisualChildren.FirstOrDefault() as StackPanel;
            Assert.IsNotNull(stackPanel);
            Assert.AreEqual(3, stackPanel.Children.Count);

            ContentPresenter presenter1 = stackPanel.Children[0] as ContentPresenter;
            ContentPresenter presenter2 = stackPanel.Children[1] as ContentPresenter;
            ContentPresenter presenter3 = stackPanel.Children[2] as ContentPresenter;
            Assert.IsNotNull(presenter1);
            Assert.IsNotNull(presenter2);
            Assert.IsNotNull(presenter3);

            TextBlock textBlock1 = presenter1.VisualChildren.FirstOrDefault() as TextBlock;
            Assert.IsNotNull(textBlock1);
            Assert.AreEqual("item1", textBlock1.Text);
        }

        [TestMethod]
        public void ItemsControlMeasureTest()
        {
            string text = @"
            <ItemsControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <ItemsControl.Template>
                    <ControlTemplate>
                        <ItemsPresenter ItemContainerGenerator='{TemplateBinding ItemsControl.ItemContainerGenerator}' Template='{TemplateBinding ItemsControl.ItemsPanel}'/>
                    </ControlTemplate>
                </ItemsControl.Template>
            </ItemsControl>";

            ItemsControl itemsControl = XamlLoader.Load(XamlParser.Parse(text)) as ItemsControl;
            itemsControl.Items.Add(new Border { Width = 100, Height = 50 });
            itemsControl.Items.Add(new Border { Width = 100, Height = 50 });
            itemsControl.Items.Add(new Border { Width = 100, Height = 50 });

            itemsControl.Measure(Size.Infinity);
            Assert.AreEqual(new Size(100, 150), itemsControl.DesiredSize);
        }

        [TestMethod]
        public void ItemsControlTemplatesTest()
        {
            string text = @"
            <ItemsControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <DockPanel/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.Template>
                    <ControlTemplate>
                        <Grid>
                            <TextBlock Text='Header'/>
                            <ItemsPresenter ItemContainerGenerator='{TemplateBinding ItemsControl.ItemContainerGenerator}' Template='{TemplateBinding ItemsControl.ItemsPanel}'/>
                        </Grid>
                    </ControlTemplate>
                </ItemsControl.Template>
                <ItemsControl.ItemContainerStyle>
                    <Style>
                        <Setter Property='DockPanel.Dock' Value='Bottom'/>
                    </Style>
                </ItemsControl.ItemContainerStyle>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <TextBlock Text='{Binding}'/>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>";

            ItemsControl itemsControl = XamlLoader.Load(XamlParser.Parse(text)) as ItemsControl;

            itemsControl.Items.Add("item1");
            itemsControl.Items.Add("item2");
            itemsControl.Items.Add("item3");

            Grid grid = itemsControl.VisualChildren.FirstOrDefault() as Grid;
            Assert.IsNotNull(grid);
            Assert.AreEqual(2, grid.Children.Count);

            TextBlock textBlock = grid.Children[0] as TextBlock;
            ItemsPresenter itemsPresenter = grid.Children[1] as ItemsPresenter;
            Assert.IsNotNull(textBlock);
            Assert.IsNotNull(itemsPresenter);
            Assert.AreEqual("Header", textBlock.Text);

            DockPanel dockPanel = itemsPresenter.VisualChildren.FirstOrDefault() as DockPanel;
            Assert.IsNotNull(dockPanel);
            Assert.AreEqual(3, dockPanel.Children.Count);

            ContentPresenter presenter1 = dockPanel.Children[0] as ContentPresenter;
            ContentPresenter presenter2 = dockPanel.Children[1] as ContentPresenter;
            ContentPresenter presenter3 = dockPanel.Children[2] as ContentPresenter;
            Assert.IsNotNull(presenter1);
            Assert.IsNotNull(presenter2);
            Assert.IsNotNull(presenter3);
            Assert.AreEqual(Dock.Bottom, DockPanel.GetDock(presenter1));
            Assert.AreEqual(Dock.Bottom, DockPanel.GetDock(presenter2));
            Assert.AreEqual(Dock.Bottom, DockPanel.GetDock(presenter3));

            TextBlock textBlock1 = presenter1.VisualChildren.FirstOrDefault() as TextBlock;
            Assert.IsNotNull(textBlock1);
            Assert.AreEqual("item1", textBlock1.Text);

            itemsControl.Items[0] = "item4";
            Assert.IsFalse(presenter1.VisualChildren.Any());
            Assert.IsNull(textBlock1.Text);

            ContentPresenter presenter4 = dockPanel.Children[0] as ContentPresenter;
            Assert.AreNotEqual(presenter1, presenter4);

            TextBlock textBlock4 = presenter4.VisualChildren.FirstOrDefault() as TextBlock;
            Assert.AreEqual("item4", textBlock4.Text);
        }
    }
}
