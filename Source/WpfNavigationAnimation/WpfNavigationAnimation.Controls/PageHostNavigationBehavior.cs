namespace WpfNavigationAnimation.Controls
{
    public class PageHostNavigationBehavior
    {
        public PageHostNavigationBehavior(
            OldPageAnimation oldPageAnimation,
            NewPageAnimation newPageAnimation)
        {
            OldPageAnimation = oldPageAnimation;
            NewPageAnimation = newPageAnimation;
        }

        public OldPageAnimation OldPageAnimation { get; set; }
        public NewPageAnimation NewPageAnimation { get; set; }
    }
}
