using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace ComponentSample
{
    public class DaliDemo : NUIApplication
    {
        private IExample curExample = null;

        private DaliTableView demo;

        public DaliDemo() : base(new Size2D(1000, 1000), new Position2D(0, 0))
        {
        }

        private void FullGC()
        {
            global::System.GC.Collect();
            global::System.GC.WaitForPendingFinalizers();
            global::System.GC.Collect();
        }

        private void DeleteDaliDemo()
        {
            Window.Instance.Remove(demo.mRootActor);
            demo.mRootActor.Dispose();
            demo = null;

            FullGC();
        }

        private void CreateDaliDemo()
        {
            demo = new DaliTableView((string name) =>
            {
                RunSample(name);
            });

            Assembly assembly = this.GetType().Assembly;

            Type exampleType = assembly.GetType("ComponentSample.IExample");

            foreach (Type type in assembly.GetTypes())
            {
                if (exampleType.IsAssignableFrom(type) && type.Name != "SampleMain" && this.GetType() != type && true == type.IsClass)
                {
                    demo.AddExample(new Example(type.FullName, type.Name));
                }
            }

            demo.SortAlphabetically(true);

            demo.Initialize();

            Window.Instance.GetDefaultLayer().Add(demo.mRootActor);

            Window.Instance.BackgroundColor = Color.White;
        }

        private void RunSample(string name)
        {
            Assembly assembly = typeof(DaliDemo).Assembly;

            IExample example = assembly?.CreateInstance(name) as IExample;

            if (null != example)
            {
                DeleteDaliDemo();

                example.Activate();
            }

            curExample = example;
        }

        private void ExitSample()
        {
            curExample?.Deactivate();
            curExample = null;

            FullGC();

            CreateDaliDemo();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            CreateDaliDemo();

            Window.Instance.KeyEvent += Instance_KeyEvent;
        }

        private void Instance_KeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Up)
            {
                if (e.Key.KeyPressedName == "Escape" || e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "BackSpace")
                {
                    if (null != curExample)
                    {
                        ExitSample();
                    }
                    else
                    {
                        Dispose();
                    }
                }
            }
        }

        public void Deactivate()
        {
            demo = null;
        }
    }
}
