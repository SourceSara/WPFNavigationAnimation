using System;
using System.Threading.Tasks;
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

        private const double AnimationsDuration = 0.24;
        private const double SlideAnimationMargin = 600;
        private bool _isDirectNavigationDisabled;
        private OldPageAnimation _oldPageAnimation;
        private NewPageAnimation _newPageAnimation;

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

        public Page OldPage { get; protected set; }
        public Page NewPage { get; protected set; }

        #endregion

        #region Event handlers

        private async void OnNavigating(object sender, NavigatingCancelEventArgs args)
        {
            if (_oldPageAnimation == OldPageAnimation.None &&
                _newPageAnimation == NewPageAnimation.None)
                return;

            if (args.Uri is { } && args.Content is null)
                throw new InvalidOperationException("PageHost does not supports Uri.");

            if (args.Content is { } && args.Content.Equals(NewPage))
                return;

            if (Content is { } && _isDirectNavigationDisabled)
            {
                args.Cancel = true;

                OldPage = (Page)Content;
                HandleOldPageAnimaton(OldPage, args);

                await Task.Delay(TimeSpan.FromSeconds(AnimationsDuration / 1.2));
                HandleNavigation(args);
            }
            else
            {
                NewPage = (Page)args.Content;
                HandleNewPageAnimaton(NewPage);
                _isDirectNavigationDisabled = true;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs args)
            => NavigationService.RemoveBackEntry();

        #endregion

        #region Helper methods

        private void HandleOldPageAnimaton(Page page, NavigatingCancelEventArgs args)
        {
            switch (_oldPageAnimation)
            {
                case OldPageAnimation.None:
                    HandleNavigation(args);
                    break;
                case OldPageAnimation.FadeOut:
                    FadeOut(page);
                    break;
                case OldPageAnimation.ScaleToIn:
                    OldPageScaleToIn(page);
                    break;
                case OldPageAnimation.ScaleToOut:
                    OldPageScaleToOut(page);
                    break;
                case OldPageAnimation.SlideOutToRight:
                case OldPageAnimation.SlideOutToLeft:
                case OldPageAnimation.SlideOutToTop:
                case OldPageAnimation.SlideOutToBottom:
                    SlideOutTo(_oldPageAnimation, page);
                    break;
            }
        }

        private void HandleNewPageAnimaton(Page page)
        {
            switch (_newPageAnimation)
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
                case NewPageAnimation.SlideInFromLeft:
                case NewPageAnimation.SlideInFromTop:
                case NewPageAnimation.SlideInFromBottom:
                    SlideInFrom(_newPageAnimation, page);
                    break;
            }
        }

        private void HandleNavigation(NavigatingCancelEventArgs args)
        {
            _isDirectNavigationDisabled = false;

            switch (args.NavigationMode)
            {
                case NavigationMode.New:
                    Navigate(args.Content);
                    break;
            }

            if (Content is null)
                NewPage = (Page)args.Content;

            HandleNewPageAnimaton(NewPage);
            _isDirectNavigationDisabled = true;
        }

        #endregion

        #region Public methods

        public void ChangeView(FrameworkElement view, OldPageAnimation oldPageAnimation, NewPageAnimation newPageAnimation)
        {
            _oldPageAnimation = oldPageAnimation;
            _newPageAnimation = newPageAnimation;

            Navigate(view);
        }

        #endregion

        #region Animations

        #region Generators

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
                BeginTime = beginTime is null ? TimeSpan.FromSeconds(0) : beginTime,
                EasingFunction = new PowerEase
                {
                    Power = 8,
                    EasingMode = EasingMode.EaseInOut
                }
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

        #region New page

        private static void FadeIn(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(AnimationsDuration));
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void NewPageScaleFromIn(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(0.5, 0.5), new Point(1, 1), TimeSpan.FromSeconds(AnimationsDuration));
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(AnimationsDuration / 2));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void NewPageScaleFromOut(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(1.5, 1.5), new Point(1, 1), TimeSpan.FromSeconds(AnimationsDuration));
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(AnimationsDuration / 2));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void SlideInFrom(NewPageAnimation animation, Page page)
        {
            var fromThickness = GetThickness();
            var marginAnimation = GetMarginAnimation(page, fromThickness, new Thickness(0), TimeSpan.FromSeconds(AnimationsDuration));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 0, 1, TimeSpan.FromSeconds(AnimationsDuration / 2));
            var storyboard = new Storyboard();

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();

            Thickness GetThickness()
            {
                return animation switch
                {
                    NewPageAnimation.SlideInFromRight => new Thickness(0, 0, -SlideAnimationMargin, 0),
                    NewPageAnimation.SlideInFromLeft => new Thickness(-SlideAnimationMargin, 0, 0, 0),
                    NewPageAnimation.SlideInFromTop => new Thickness(0, -SlideAnimationMargin, 0, 0),
                    NewPageAnimation.SlideInFromBottom => new Thickness(0, 0, 0, -SlideAnimationMargin),
                    _ => throw new NotSupportedException(),
                };
            }
        }

        #endregion

        #region Old page

        private static void FadeOut(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var marginAnimation = GetMarginAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(AnimationsDuration));
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void OldPageScaleToIn(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(1, 1), new Point(0.8, 0.8), TimeSpan.FromSeconds(AnimationsDuration));
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(AnimationsDuration / 2));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void OldPageScaleToOut(Page page)
        {
            var (xAnimation, yAnimation) = GetScaleAnimation(page, new Point(1, 1), new Point(1.2, 1.2), TimeSpan.FromSeconds(AnimationsDuration));
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(AnimationsDuration / 2));
            var marginAnimation = GetMarginAnimation(page);
            var storyboard = new Storyboard();

            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }

        private static void SlideOutTo(OldPageAnimation animation, Page page)
        {
            var toThickness = GetThickness();
            var marginAnimation = GetMarginAnimation(page, new Thickness(0), toThickness, TimeSpan.FromSeconds(AnimationsDuration));
            var (xAnimation, yAnimation) = GetScaleAnimation(page);
            var opacityAnimation = GetOpacityAnimation(page, 1, 0, TimeSpan.FromSeconds(AnimationsDuration / 2), TimeSpan.FromSeconds(AnimationsDuration / 3));
            var storyboard = new Storyboard();

            storyboard.Children.Add(marginAnimation);
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();

            Thickness GetThickness()
            {
                return animation switch
                {
                    OldPageAnimation.SlideOutToRight => new Thickness(0, 0, -SlideAnimationMargin, 0),
                    OldPageAnimation.SlideOutToLeft => new Thickness(-SlideAnimationMargin, 0, 0, 0),
                    OldPageAnimation.SlideOutToTop => new Thickness(0, -SlideAnimationMargin, 0, 0),
                    OldPageAnimation.SlideOutToBottom => new Thickness(0, 0, 0, -SlideAnimationMargin),
                    _ => throw new NotSupportedException(),
                };
            }
        }

        #endregion

        #endregion
    }
}