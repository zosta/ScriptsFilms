using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Windows.Controls;

namespace FilmApp.Model
{
    public class TextBoxAppender : AppenderSkeleton
    {
        private TextBox _textBox;

        public string msg;
        public TextBox AppenderTextBox
        {
            get
            {
                return _textBox;
            }
            set
            {
                _textBox = value;
            }
        }
        public string FormName { get; set; }
        public string TextBoxName { get; set; }


        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            msg +=loggingEvent.RenderedMessage + Environment.NewLine;
                //_textBox.AppendText(loggingEvent.RenderedMessage + Environment.NewLine);
        }
    }
}
