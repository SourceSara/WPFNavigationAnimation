using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace WpfNavigationAnimation.Controls
{
    public class PageHost : Frame
    {
        #region Fields

        private bool _isDirectNavigationDisabled;
        private PageHostNavigationBehavior _navigationBehavior;

        #endregion

        #region Constructors

        static PageHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageHost),
                new FrameworkPropertyMetadata(typeof(PageHost)));
        }

        public PageHost()
        {
            _isDirectNavigationDisabled = true;

            Navigating += OnNavigating;
            Navigated += OnNavigated;
        }

        #endregion

        #region Properties

        public Page PreviousPage { get; protected set; }
        public Page CurrentPage { get; protected set; }

        #endregion

        #region Event handlers

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Content is { } content && content.Equals(CurrentPage))
                return;

            if (Content is { } && _isDirectNavigationDisabled)
            {
                e.Cancel = true;

                PreviousPage = (Page)Content;
                HandleOldPageAnimaton(PreviousPage, e);
            }
            else
            {
                CurrentPage = (Page)e.Content;
                HandleNewPageAnimaton(CurrentPage);
                _isDirectNavigationDisabled = true;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
            => NavigationService.RemoveBackEntry();

        #endregion

        #region Helper methods

        private void HandleOldPageAnimaton(Page page, NavigatingCancelEventArgs e = null)
        {
            switch (_navigationBehavior.OldPageAnimation)
            {
                case OldPageAnimation.FadeOut:
                    FadeOut(page, e);
                    break;
                case OldPageAnimation.ScaleToIn:
                    OldPageScaleToIn(page, e);
                    break;
                case OldPageAnimation.ScaleToOut:
                    OldPageScaleToOut(page, e);
                    break;
                case OldPageAnimation.SlideOutToRight:
                    SlideOutToRight(page, e);
                    break;
                case OldPageAnimation.SlideOutToLeft:
                    SlideOutToLeft(page, e);
                    break;
            }
        }

        private void HandleNewPageAnimaton(Page page)
        {
            switch (_navigationBehavior.NewPageAnimation)
            {
                case NewPageAnimation.FadeIn:
                    FadeIn(page);
                    break;
                case NewPageAnimation.ScaleFromIn:
                    NewPageScaleFromIn(page);
                    break;
                case NewPageAnimation.ScaleFromOut:
                    NewPageScaleFromOut(page);
                    break;
                case NewPageAnimation.SlideInFromRight:
                    SlideInFromRight(page);
                    break;
                case NewPageAnimation.SlideInFromLeft:
                    SlideInFromLeft(page);
                    break;
            }
        }

        private void HandleNavigation(NavigatingCancelEventArgs e)
        {
            _isDirectNavigationDisabled = false;

            switch (e.NavigationMode)
            {
                case NavigationMode.New:
                    Navigate(e.Uri is null ? e.Content : e.Uri);
                    break;
                case NavigationMode.Back:
                    GoBack();
                    break;
                case NavigationMode.Forward:
                    GoForward();
                    break;
                case NavigationMode.Refresh:
                    Refresh();
                    break;
            }

            if (Content is null)
                CurrentPage = (Page)e.Content;

            HandleNewPageAnimaton(CurrentPage);
            _isDirectNavigationDisabled = true;
        }

        private static DoubleAnimation GetOpacityAnimation(
            Page page, double? from = null, double? to = null,
            TimeSpan? duration = null, TimeSpan? beginTime = null)
        {
            var animation = new DoubleAnimation
            {
                From = from is null ? 1 : from.GetValueOrDefault(),
                To = to is null ? 1 : to.GetValueOrDefault(),
                Duration = new Duration(duration is null ? TimeSpan.FromSeconds(0) : duration.GetValueOrDefault()),
                BeginTime = beginTime is null ? TimeSpan.FromSeconds(0) : beginTime
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(animation, page);

            return animation;
        }

        private static ThicknessAnimation GetMarginAnimation(
            Page page, Thickness? from = null, Thickness? to = null,
            TimeSpan? duration = null, TimeSpan? beginTime = null)
        {
            var animation = new ThicknessAnimation
            {
                From = from is null ? new Thickness(0) : from,
                To = to is null ? new Thickness(0) : to,
                Duration = new Duration(duration is null ? TimeSpan.FromSeconds(0) : duration.GetValueOrDefault()),
                BeginTime = beginTime is null ? TimeSpan.FromSeconds(0) : beginTime
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(MarginProperty));
            Storyboard.SetTarget(animation, page);

            return animation;
        }

        private static (DoubleAnimation, DoubleAnimation) GetScaleAnimation(
           Page page, Point? from = null, Point? to = null,
           TimeSpan? duration = null, TimeSpan? beginTime = null)
        {
            var scale = new ScaleTransform(1, 1);
            var xAnimation = new DoubleAnimation
            {
                From = from is null ? 1 : from.GetValueOrDefault().X,
                To = to is null ? 1 : to.GetValueOrDefault().X,
                Duration = new Duration(duration is null ? TimeSpan.FromSeconds(0) : duration.GetValueOrDefault()),
                BeginTime = beginTime is null ? TimeSpan.FromSeconds(0) : beginTime
            };
            var yAnimation = new DoubleAnimation
            {
                From = from is null ? 1 : from.GetValueOrDefault().Y,
                To = to is null ? 1 : to.GetValueOrDefault().Y,
                Duration = new Duration(duration is null ? TimeSpan.FromSeconds(0) : duration.GetValueOrDefault()),
                BeginTime = beginTime is null ? TimeSpan.FromSeconds(0) : beginTime
            };

            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(xAnimation, page);

            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(yAnimation, page);

            page.RenderTransformOrigin = new Point(0.5, 0.5);
            page.RenderTransform = scale;

            return (xAnimation, yAnimation);
        }

        #endregion

        #region Public methods

        public void ChangeView(Page page, OldPageAnimation oldPageAnimation, NewPageAnimation newPageAnimation)
        {
            _navigationBehavior = new PageHostNavigationBehavior(oldPageAnimation, newPageAnimation);
            Navigate(page);
        }

        #endregion

        #region Animations

        private void FadeIn(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void NewPageScaleFromIn(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(0, 0), new Point(1, 1), TimeSpan.FromSeconds(0.24));
            var opacityAnimation = GetOpacityAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void NewPageScaleFromOut(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(2, 2), new Point(1, 1), TimeSpan.FromSeconds(0.24));
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(0.24));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void SlideInFromRight(Page page)
        {
            var fromThickness = new Thickness(-600, 0, 0, 0);
            var marginAnimation = GetMarginAnimation(page, fromThickness, new Thickness(0), TimeSpan.FromSeconds(0.24));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void SlideInFromLeft(Page page)
        {
            var fromThickness = new Thickness(0, 0, -600, 0);
            var marginAnimation = GetMarginAnimation(page, fromThickness, new Thickness(0), TimeSpan.FromSeconds(0.24));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void FadeOut(Page page, NavigatingCancelEventArgs e = default)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            if (e is { })
                storyboard.Completed += (sender, args) => HandleNavigation(e);

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void OldPageScaleToIn(Page page, NavigatingCancelEventArgs e = default)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(1, 1), new Point(0, 0), TimeSpan.FromSeconds(0.24));
            var opacityAnimation = GetOpacityAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            if (e is { })
                storyboard.Completed += (sender, args) => HandleNavigation(e);

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void OldPageScaleToOut(Page page, NavigatingCancelEventArgs e = default)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(1, 1), new Point(2, 2), TimeSpan.FromSeconds(0.24));
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(0.24));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            if (e is { })
                storyboard.Completed += (sender, args) => HandleNavigation(e);

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void SlideOutToLeft(Page page, NavigatingCancelEventArgs e = default)
        {
            var toThickness = new Thickness(0, 0, -600, 0);
            var marginAnimation = GetMarginAnimation(page, new Thickness(0), toThickness, TimeSpan.FromSeconds(0.24));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            if (e is { })
                storyboard.Completed += (sender, args) => HandleNavigation(e);

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        private void SlideOutToRight(Page page, NavigatingCancelEventArgs e = default)
        {
            var toThickness = new Thickness(-600, 0, 0, 0);
            var marginAnimation = GetMarginAnimation(page, new Thickness(0), toThickness, TimeSpan.FromSeconds(0.24));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(0.24));
            var storyboard = new Storyboard();

            if (e is { })
                storyboard.Completed += (sender, args) => HandleNavigation(e);

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();
        }

        #endregion
    }
}
