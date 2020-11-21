using System.Windows;
using WpfNavigationAnimation.Controls;

namespace WpfNavigationAnimation
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFadeButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new FadeOutFadeInPage(), OldPageAnimation.FadeOut, NewPageAnimation.FadeIn);
        }

        private void OnScaleButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new ScaleOutScaleInPage(), OldPageAnimation.ScaleOut, NewPageAnimation.ScaleIn);
        }

        private void OnSlideLeftRightButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new SlideLeftRightPage(), OldPageAnimation.SlideOutToLeft, NewPageAnimation.SlideInFromRight);
        }

        private void OnSlideRightLeftButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindowPageHost.ChangeView(new SlideRightLeftPage(), OldPageAnimation.SlideOutToRight, NewPageAnimation.SlideInFromLeft);
        }
    }
}
