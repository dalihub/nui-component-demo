using System.Collections.Generic;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI;

namespace ComponentSample
{
  public class TextLabelSample : IExample
  {
    private View mRoot;

    public void Activate()
    {
        Window window = Window.Instance;

        mRoot = new View()
        {
            Size2D = new Size2D(1920, 1080),
        };
        window.Add(mRoot);

        TextLabel label = new TextLabel() {
          Text = "Text 1",
        };

        label.Style.Text = "Text 2";

        mRoot.Add(label);
    }

    public void Deactivate()
    {
        if (mRoot != null)
        {
            Window.Instance.Remove(mRoot);
            mRoot.Dispose();
        }
    }
  }
}