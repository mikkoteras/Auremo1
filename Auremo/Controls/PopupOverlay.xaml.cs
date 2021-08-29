using Auremo.DataModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class PopupOverlay : UserControl
    {
        public delegate void Callback(bool okClicked, string answer);

        private static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(PopupOverlay), new FrameworkPropertyMetadata(""));
        private static readonly DependencyProperty AnswerProperty =
            DependencyProperty.Register("Answer", typeof(string), typeof(PopupOverlay), new FrameworkPropertyMetadata(""));

        Callback m_Callback = null;

        public PopupOverlay()
        {
            InitializeComponent();
        }

        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            private set => SetValue(QuestionProperty, value);
        }

        public string Answer
        {
            get => (string)GetValue(AnswerProperty);
            private set => SetValue(AnswerProperty, value);
        }

        public void Ask(string question, Callback callback)
        {
            Run(question, null, callback);
        }

        public void Run(string question, string defaultAnswer, Callback callback)
        {
            Question = question;
            Answer = defaultAnswer ?? "";
            m_Callback = callback;
            World.Instance.InterfaceState.PopupOverlayIsActive = true;
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool turnedVisible && turnedVisible)
            {
                m_Input.Focus();
            }
        }
        
        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Finish(true);
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Finish(false);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Finish(e.Key == Key.Enter);
                e.Handled = true;
            }
        }

        private void Finish(bool ok)
        {
            if (m_Callback != null)
            {
                m_Callback.Invoke(ok, Answer);
            }

            World.Instance.InterfaceState.PopupOverlayIsActive = false;
            Question = "";
            Answer = "";
            m_Callback = null;
        }
    }
}
