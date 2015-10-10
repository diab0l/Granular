using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Granular.Extensions;
using System.Windows.Input;

namespace Granular.Host.Render
{
    public class HtmlTextBoxRenderElement : HtmlRenderElement, ITextBoxRenderElement
    {
        private HtmlRenderElement contentElement;
        private HtmlRenderElement ContentElement
        {
            get { return contentElement; }
            set
            {
                if (contentElement == value)
                {
                    return;
                }

                if (contentElement != null)
                {
                    HtmlElement.RemoveChild(contentElement.HtmlElement);
                }

                contentElement = value;

                if (contentElement != null)
                {
                    HtmlElement.AppendChild(contentElement.HtmlElement);
                }
            }
        }

        public event EventHandler TextChanged;
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                {
                    return;
                }

                text = value;

                SetContentElementText();
                GetContentElementSelection();
                TextChanged.Raise(this);
            }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                if (maxLength == value)
                {
                    return;
                }

                maxLength = value;
                SetContentElementMaxLength();
            }
        }

        public event EventHandler CaretIndexChanged;
        private int caretIndex;
        public int CaretIndex
        {
            get { return caretIndex; }
            set
            {
                if (caretIndex == value)
                {
                    return;
                }

                caretIndex = value;
                SetContentElementCaretIndex();
                CaretIndexChanged.Raise(this);
            }
        }

        public event EventHandler SelectionStartChanged;
        private int selectionStart;
        public int SelectionStart
        {
            get { return selectionStart; }
            set
            {
                if (selectionStart == value)
                {
                    return;
                }

                selectionStart = value;
                SetContentElementSelectionStart();
                SelectionStartChanged.Raise(this);
            }
        }

        public event EventHandler SelectionLengthChanged;
        private int selectionLength;
        public int SelectionLength
        {
            get { return selectionLength; }
            set
            {
                if (selectionLength == value)
                {
                    return;
                }

                selectionLength = value;
                SetContentElementSelectionLength();
                SelectionLengthChanged.Raise(this);
            }
        }

        private bool isPassword;
        public bool IsPassword
        {
            get { return isPassword; }
            set
            {
                if (isPassword == value)
                {
                    return;
                }

                isPassword = value;
                SetContentElement();
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly == value)
                {
                    return;
                }

                isReadOnly = value;
                SetContentElementIsReadOnly();
            }
        }

        private bool spellCheck;
        public bool SpellCheck
        {
            get { return spellCheck; }
            set
            {
                if (spellCheck == value)
                {
                    return;
                }

                spellCheck = value;
                SetContentElementSpellCheck();
            }
        }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                Style.SetBounds(bounds, converter);
                ContentElement.Style.SetSize(bounds.Size, converter);
            }
        }

        private Brush foreground;
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (foreground == value)
                {
                    return;
                }

                if (foreground != null)
                {
                    foreground.Changed -= OnForegroundChanged;
                }

                foreground = value;
                ContentElement.Style.SetForeground(Foreground, converter);

                if (foreground != null)
                {
                    foreground.Changed += OnForegroundChanged;
                }
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                if (fontFamily == value)
                {
                    return;
                }

                fontFamily = value;
                ContentElement.Style.SetFontFamily(fontFamily, converter);
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                if (fontSize == value)
                {
                    return;
                }

                fontSize = value;
                ContentElement.Style.SetFontSize(fontSize, converter);
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (fontStyle == value)
                {
                    return;
                }

                fontStyle = value;
                ContentElement.Style.SetFontStyle(fontStyle, converter);
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (fontWeight == value)
                {
                    return;
                }

                fontWeight = value;
                ContentElement.Style.SetFontWeight(fontWeight, converter);
            }
        }

        private FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                if (fontStretch == value)
                {
                    return;
                }

                fontStretch = value;
                ContentElement.Style.SetFontStretch(fontStretch, converter);
            }
        }

        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                if (textAlignment == value)
                {
                    return;
                }

                textAlignment = value;
                ContentElement.Style.SetTextAlignment(textAlignment, converter);
            }
        }

        private TextTrimming textTrimming;
        public TextTrimming TextTrimming
        {
            get { return textTrimming; }
            set
            {
                if (textTrimming == value)
                {
                    return;
                }

                textTrimming = value;
                ContentElement.Style.SetTextTrimming(textTrimming);
            }
        }

        private TextWrapping textWrapping;
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set
            {
                if (textWrapping == value)
                {
                    return;
                }

                textWrapping = value;
                SetContentElementTextWrapping();
            }
        }

        private bool acceptsReturn;
        public bool AcceptsReturn
        {
            get { return acceptsReturn; }
            set
            {
                if (acceptsReturn == value)
                {
                    return;
                }

                acceptsReturn = value;
                SetContentElement();
            }
        }

        private ScrollBarVisibility horizontalScrollBarVisibility;
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return horizontalScrollBarVisibility; }
            set
            {
                if (horizontalScrollBarVisibility == value)
                {
                    return;
                }

                horizontalScrollBarVisibility = value;
                ContentElement.Style.SetHorizontalScrollBarVisibility(horizontalScrollBarVisibility, converter);
            }
        }

        private ScrollBarVisibility verticalScrollBarVisibility;
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return verticalScrollBarVisibility; }
            set
            {
                if (verticalScrollBarVisibility == value)
                {
                    return;
                }

                verticalScrollBarVisibility = value;
                ContentElement.Style.SetVerticalScrollBarVisibility(verticalScrollBarVisibility, converter);
            }
        }

        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                if (isHitTestVisible == value)
                {
                    return;
                }

                isHitTestVisible = value;
                ContentElement.Style.SetIsHitTestVisible(IsHitTestVisible);
            }
        }

        public bool AcceptsTab { get; set; }

        private IRenderQueue renderQueue;
        private IHtmlValueConverter converter;
        private bool isFocused;

        public HtmlTextBoxRenderElement(IRenderQueue renderQueue, IHtmlValueConverter converter) :
            base("div", renderQueue)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            this.bounds = System.Windows.Rect.Empty;
            this.fontFamily = FontFamily.Default;

            Element styleElement = Document.CreateElement("style");
            styleElement.TextContent = "::-ms-clear { width: 0px; height: 0px; }";

            HtmlElement.AppendChild(styleElement);

            SetContentElement();
        }

        private void SetContentElement()
        {
            if (IsPassword || !AcceptsReturn)
            {
                ContentElement = new HtmlRenderElement("input", renderQueue);
                ContentElement.HtmlElement.SetAttribute("type", IsPassword ? "password" : "text");
            }
            else
            {
                ContentElement = new HtmlRenderElement("textarea", renderQueue);
            }

            SetContentElementText();
            SetContentElementMaxLength();
            SetContentElementSelectionStart();
            SetContentElementSelectionLength();
            SetContentElementIsReadOnly();
            SetContentElementSpellCheck();
            SetContentElementTextWrapping();

            ContentElement.Style.SetValue("resize", "none");
            ContentElement.Style.SetValue("margin", "0px");
            ContentElement.Style.SetValue("padding", "0px");
            ContentElement.Style.SetValue("border", "0px solid transparent");
            ContentElement.Style.SetValue("outline", "1px solid transparent");
            ContentElement.Style.SetValue("cursor", "inherit");
            ContentElement.Style.SetBackground(Brushes.Transparent, converter);
            ContentElement.Style.SetLocation(Point.Zero, converter);

            ContentElement.Style.SetSize(Bounds.Size, converter);
            ContentElement.Style.SetForeground(Foreground, converter);
            ContentElement.Style.SetFontFamily(FontFamily, converter);
            ContentElement.Style.SetFontSize(FontSize, converter);
            ContentElement.Style.SetFontStyle(FontStyle, converter);
            ContentElement.Style.SetFontWeight(FontWeight, converter);
            ContentElement.Style.SetFontStretch(FontStretch, converter);
            ContentElement.Style.SetIsHitTestVisible(IsHitTestVisible);
            ContentElement.Style.SetTextAlignment(TextAlignment, converter);
            ContentElement.Style.SetTextTrimming(TextTrimming);
            ContentElement.Style.SetHorizontalScrollBarVisibility(HorizontalScrollBarVisibility, converter);
            ContentElement.Style.SetVerticalScrollBarVisibility(VerticalScrollBarVisibility, converter);

            ContentElement.HtmlElement.OnInput += e => this.Text = ContentElement.HtmlElement.GetValue();
            ContentElement.HtmlElement.OnKeydown += OnContentElementKeyDown;
            ContentElement.HtmlElement.OnSelect += e => GetContentElementSelection();
            ContentElement.HtmlElement.OnKeyup += e => GetContentElementSelection();
            ContentElement.HtmlElement.OnMouseup += e => GetContentElementSelection();
        }

        public void Focus()
        {
            isFocused = true;
            ContentElement.HtmlElement.Focus();
        }

        public void ClearFocus()
        {
            isFocused = false;
            ContentElement.HtmlElement.Blur();
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            ContentElement.Style.SetForeground(Foreground, converter);
        }

        private void OnContentElementKeyDown(Event e)
        {
            if (!IsReadOnly && AcceptsTab && ((KeyboardEvent)e).KeyCode == 9)
            {
                int selectionStart = SelectionStart;

                string contentElementText = ContentElement.HtmlElement.GetValue();
                this.Text = String.Format("{0}\t{1}", contentElementText.Substring(0, SelectionStart), contentElementText.Substring(SelectionStart + SelectionLength));

                ContentElement.HtmlElement.SetSelectionStart(selectionStart + 1);
                ContentElement.HtmlElement.SetSelectionEnd(selectionStart + 1);
                GetContentElementSelection();

                e.PreventDefault();
            }
        }

        private void GetContentElementSelection()
        {
            int selectionStart = ContentElement.HtmlElement.GetSelectionStart();
            int selectionEnd = ContentElement.HtmlElement.GetSelectionEnd();

            if (this.SelectionStart != selectionStart || this.SelectionLength != selectionEnd - selectionStart)
            {
                int changeIndex = this.SelectionStart + this.SelectionLength != selectionEnd ? selectionEnd : selectionStart;

                this.SelectionStart = selectionStart;
                this.SelectionLength = selectionEnd - selectionStart;
                this.CaretIndex = changeIndex;
            }
        }

        private void SetContentElementCaretIndex()
        {
            if (isFocused && CaretIndex != SelectionStart && CaretIndex != SelectionStart + SelectionLength)
            {
                ContentElement.HtmlElement.Focus();
                ContentElement.HtmlElement.SetCaretIndex(CaretIndex);
            }
        }

        private void SetContentElementSelectionStart()
        {
            if (ContentElement.HtmlElement.GetSelectionStart() != SelectionStart)
            {
                ContentElement.HtmlElement.SetSelectionStart(SelectionStart);
            }
        }

        private void SetContentElementSelectionLength()
        {
            if (ContentElement.HtmlElement.GetSelectionEnd() != SelectionStart + SelectionLength)
            {
                ContentElement.HtmlElement.SetSelectionEnd(SelectionStart + SelectionLength);
            }
        }

        private void SetContentElementText()
        {
            if (ContentElement.HtmlElement.GetValue() != Text.DefaultIfNullOrEmpty())
            {
                ContentElement.HtmlElement.SetValue(Text);
            }
        }

        private void SetContentElementMaxLength()
        {
            if (maxLength > 0)
            {
                ContentElement.HtmlElement.SetAttribute("maxLength", maxLength.ToString());
            }
            else
            {
                ContentElement.HtmlElement.RemoveAttribute("maxLength");
            }
        }

        private void SetContentElementIsReadOnly()
        {
            if (IsReadOnly)
            {
                ContentElement.HtmlElement.SetAttribute("readonly", String.Empty);
            }
            else
            {
                ContentElement.HtmlElement.RemoveAttribute("readonly");
            }
        }

        private void SetContentElementSpellCheck()
        {
            ContentElement.HtmlElement.SetAttribute("spellcheck", converter.ToBooleanString(SpellCheck));
        }

        private void SetContentElementTextWrapping()
        {
            ContentElement.HtmlElement.SetAttribute("wrap", converter.ToWrapString(TextWrapping));
        }

        public void ProcessKeyEvent(KeyEventArgs e)
        {
            e.ForceHostHandling = true;
            e.Handled = true;
        }
    }
}
