using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.UIComponents;
using Tizen.NUI;

namespace ComponentSample
{
  public class Example
  {
    // Constructors

    /**
     * @param[in] name unique name of example
     * @param[in] title The caption for the example to appear on a tile button.
     */
    public Example(string name, string title)
    {
      this.name = name;
      this.title = title;
    }

    public Example()
    {
    }

    private string name;
    public string Name
    {
      get
      {
        return name;
      }
    }

    private string title;
    public string Title
    {
      get
      {
        return title;
      }
    }

    static public int CompareByTitle(Example lhs, Example rhs)
    {
      return String.Compare(lhs.title, lhs.title);
    }
  };

  public class DaliTableView
  {
    static private uint mCurPage = 0;
    const int EXAMPLES_PER_ROW = 3;
    const int ROWS_PER_PAGE = 3;
    Vector3 TABLE_RELATIVE_SIZE = new Vector3(0.95f, 0.9f, 0.8f);
    const int EXAMPLES_PER_PAGE = EXAMPLES_PER_ROW * ROWS_PER_PAGE;
  
    public void AddExample(Example example)
    {
      mExampleList.Add(example);
    }

    public void SortAlphabetically(bool sortAlphabetically)
    {
      mSortAlphabetically = sortAlphabetically;
    }

    public delegate void ExampleClicked(string name);

    private ExampleClicked onClicked;

    public DaliTableView(ExampleClicked onClicked)
    {
      this.onClicked = onClicked;
    }

    public void Initialize()
    {
      Window.Instance.KeyEvent += OnKeyEvent;

      Size2D stageSize = Window.Instance.WindowSize;

      // Background
      mRootActor = new View() {
        BackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1),
        WidthResizePolicy = ResizePolicyType.FillToParent,
        HeightResizePolicy = ResizePolicyType.FillToParent,
      };
      Window.Instance.Add(mRootActor);

      // Scrollview occupying the majority of the screen
      mScrollView = new ScrollView();
      mScrollView.PositionUsesPivotPoint = true;
      mScrollView.PivotPoint = PivotPoint.BottomCenter;
      mScrollView.ParentOrigin = new Vector3(0.5f, 1.0f - 0.05f, 0.5f);
      mScrollView.WidthResizePolicy = ResizePolicyType.FillToParent;
      mScrollView.HeightResizePolicy = ResizePolicyType.SizeRelativeToParent;
      mScrollView.SetSizeModeFactor(new Vector3(0.0f, 0.6f, 0.0f));

      ushort buttonsPageMargin = (ushort)((1.0f - TABLE_RELATIVE_SIZE.X) * 0.5f * stageSize.Width);
      mScrollView.SetPadding(new PaddingType(buttonsPageMargin, buttonsPageMargin, 0, 0));

      mScrollView.ScrollCompleted += OnScrollComplete;
      mScrollView.ScrollStarted += OnScrollStart;
      mPageWidth = stageSize.Width * TABLE_RELATIVE_SIZE.X * 0.5f;
      mRootActor.Add(mScrollView);

      // Add scroll view effect and setup constraints on pages
      ApplyScrollViewEffect();

      // Add pages and tiles
      Populate();

      if (mCurPage != mScrollView.GetCurrentPage())
      {
        mScrollView.ScrollTo(mCurPage, 0.0f);
      }

      // Remove constraints for inner cube effect
      ApplyCubeEffectToPages();
    }

    private void OnKeyEvent(object sender, Window.KeyEventArgs e)
    {
      if (e.Key.State == Key.StateType.Down)
      {

      }
    }

    private void OnScrollStart(object source, Scrollable.StartedEventArgs e)
    {
      mScrolling = true;
      //PlayAnimation();
    }

    private void OnScrollComplete(object source, Scrollable.CompletedEventArgs e)
    {
      // move focus to 1st item of new page
      mScrolling = false;
      mCurPage = mScrollView.GetCurrentPage();
    }

    private View CreateTile(string name, string title, Vector3 sizeMultiplier, Vector2 position)
    {
      var tile = new TextLabel() {
        Name = name,
        BackgroundColor = new Color(0.7f, 0.7f, 0.7f, 1),
        WidthResizePolicy = ResizePolicyType.SizeRelativeToParent,
        HeightResizePolicy = ResizePolicyType.SizeRelativeToParent,
        PositionUsesPivotPoint = true,
        ParentOrigin = ParentOrigin.Center,
        Focusable = true,
        Text = title,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
      };

      tile.SetSizeModeFactor(sizeMultiplier);
      tile.TouchEvent += OnTilePressed;

      return tile;
    }

    private bool OnTilePressed(object source, View.TouchEventArgs e)
    {
      var actor = source as View;
      var pointState = e.Touch.GetState(0);

      if (PointStateType.Down == pointState)
      {
        mPressedActor = actor;
      }
      else if ((PointStateType.Up == pointState) && (mPressedActor == actor))
      {
        onClicked(mPressedActor?.Name);
      }

      return true;
    }

    private List<View> tiles = new List<View>();

    private void Populate()
    {
      Vector2 stageSize = Window.Instance.WindowSize;

      mTotalPages = (uint)((mExampleList.Count() + EXAMPLES_PER_PAGE - 1) / EXAMPLES_PER_PAGE);

      // Populate ScrollView.
      if (mExampleList.Count() > 0)
      {
        if (mSortAlphabetically)
        {
          mExampleList.Sort(Example.CompareByTitle);
        }

        int pageCount = mExampleList.Count / (ROWS_PER_PAGE * EXAMPLES_PER_ROW) + ((0 == mExampleList.Count % (ROWS_PER_PAGE * EXAMPLES_PER_ROW)) ? 0 : 1);
        mPages = new View[pageCount];

        int pageIndex = 0;

        uint exampleCount = 0;

        for (int t = 0; t < mTotalPages; t++)
        {
          // Create Table
          TableView page = new TableView(ROWS_PER_PAGE, EXAMPLES_PER_ROW);
          page.PositionUsesPivotPoint = true;
          page.PivotPoint = PivotPoint.Center;
          page.ParentOrigin = ParentOrigin.Center;
          page.WidthResizePolicy = page.HeightResizePolicy = ResizePolicyType.FillToParent;
          mScrollView.Add(page);

          // Calculate the number of images going across (columns) within a page, according to the screen resolution and dpi.
          const float margin = 2.0f;
          const float tileParentMultiplier = 1.0f / EXAMPLES_PER_ROW;

          for (uint row = 0; row < ROWS_PER_PAGE; row++)
          {
            for (uint column = 0; column < EXAMPLES_PER_ROW; column++)
            {
              Example example = mExampleList.ElementAt((int)exampleCount++);

              // Calculate the tiles relative position on the page (between 0 & 1 in each dimension).
              Vector2 position = new Vector2((float)(column) / (EXAMPLES_PER_ROW - 1.0f), (float)(row) / (EXAMPLES_PER_ROW - 1.0f));
              View tile = CreateTile(example.Name, example.Title, new Vector3(tileParentMultiplier, tileParentMultiplier, 1.0f), position);

              tile.SetPadding(new PaddingType((int)margin, (int)margin, (int)margin, (int)margin));
              page.AddChild(tile, new TableView.CellPosition(row, column));
              //page.Add(tile);

              tiles.Add(tile);

              if (exampleCount == mExampleList.Count)
              {
                break;
              }
            }

            if (exampleCount == mExampleList.Count)
            {
              break;
            }
          }

          mPages[pageIndex++] = page;

          if (exampleCount == mExampleList.Count)
          {
            break;
          }
        }
      }

      // Update Ruler info.
      mScrollRulerX = new RulerPtr(new FixedRuler(mPageWidth));
      mScrollRulerY = new RulerPtr(new DefaultRuler());
      mScrollRulerX.SetDomain(new RulerDomain(0.0f, (mTotalPages + 1) * stageSize.Width * TABLE_RELATIVE_SIZE.X * 0.5f, true));
      mScrollRulerY.Disable();
      mScrollView.SetRulerX(mScrollRulerX);
      mScrollView.SetRulerY(mScrollRulerY);
    }

    private void ApplyScrollViewEffect()
    {
      // Remove old effect if exists.

      if (mScrollViewEffect)
      {
        mScrollView.RemoveEffect(mScrollViewEffect);
      }

      // Just one effect for now
      SetupInnerPageCubeEffect();

      mScrollView.ApplyEffect(mScrollViewEffect);
    }

    private void SetupInnerPageCubeEffect()
    {
      Vector2 stageSize = Window.Instance.WindowSize;

      Path path = new Path();
      PropertyArray points = new PropertyArray();
      points.PushBack(new PropertyValue(new Vector3(stageSize.X * 0.5f, 0.0f, stageSize.X * 0.5f)));
      points.PushBack(new PropertyValue(new Vector3(0.0f, 0.0f, 0.0f)));
      points.PushBack(new PropertyValue(new Vector3(-stageSize.X * 0.5f, 0.0f, stageSize.X * 0.5f)));
      path.Points = points;

      PropertyArray controlPoints = new PropertyArray();
      controlPoints.PushBack(new PropertyValue(new Vector3(stageSize.X * 0.5f, 0.0f, stageSize.X * 0.3f)));
      controlPoints.PushBack(new PropertyValue(new Vector3(stageSize.X * 0.3f, 0.0f, 0.0f)));
      controlPoints.PushBack(new PropertyValue(new Vector3(-stageSize.X * 0.3f, 0.0f, 0.0f)));
      controlPoints.PushBack(new PropertyValue(new Vector3(-stageSize.X * 0.5f, 0.0f, stageSize.X * 0.3f)));
      path.ControlPoints = controlPoints;

      mScrollViewEffect = new ScrollViewPagePathEffect(path,
                               new Vector3(-1.0f, 0.0f, 0.0f),
                               ScrollView.Property.SCROLL_FINAL_X,
                               new Vector3(stageSize.X * TABLE_RELATIVE_SIZE.X, stageSize.Y * TABLE_RELATIVE_SIZE.Y, 0.0f), mTotalPages);
    }

    void ApplyCubeEffectToPages()
    {
      uint pageCount = 0;

      for (int i = 0; i < mPages.Count(); i++)
      {
        View page = mPages[i];
        mScrollViewEffect.ApplyToPage(page, pageCount++);
      }
    }

    internal View mRootActor;        ///< All content (excluding background is anchored to this Actor)
    private ScrollView mScrollView;         ///< ScrollView container (for all Examples)
    private ScrollViewPagePathEffect mScrollViewEffect;     ///< Effect to be applied to the scroll view
    private RulerPtr mScrollRulerX;       ///< ScrollView X (horizontal) ruler
    private RulerPtr mScrollRulerY;       ///< ScrollView Y (vertical) ruler
    private View mPressedActor;       ///< The currently pressed actor.

    private View[] mPages;          ///< List of pages.
    private List<Example> mExampleList = new List<Example>();        ///< List of examples.

    private float mPageWidth;        ///< The width of a page within the scroll-view, used to calculate the domain
    private uint mTotalPages;         ///< Total pages within scrollview.

    private bool mScrolling;        ///< Flag indicating whether view is currently being scrolled
    private bool mSortAlphabetically;   ///< Sort examples alphabetically.
  }
}
