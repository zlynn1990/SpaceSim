using System.Collections.Generic;
using System.Drawing;

namespace SpaceSim.Drawing
{
    class TextDisplay
    {
        private Font _font;
        private SolidBrush _brush;

        private List<TextBlock> _textBlocks;

        public TextDisplay()
        {
            _font = new Font("Verdana Bold", 14);
            _brush = new SolidBrush(Color.White);

            _textBlocks = new List<TextBlock>();
        }

        public void AddTextBlock(StringAlignment alignment, string text)
        {
            AddTextBlock(new TextBlock { Alignment = alignment, Content = new List<string> { text } });
        }

        public void AddTextBlock(StringAlignment alignment, List<string> content)
        {
            AddTextBlock(new TextBlock { Alignment = alignment, Content = content });
        }

        public void AddTextBlock(TextBlock block)
        {
            _textBlocks.Add(block);
        }

        public void Clear()
        {
            _textBlocks.Clear();
        }

        public void Draw(Graphics graphics)
        {
            var blockOffsets = new Dictionary<StringAlignment, int>();

            foreach (TextBlock textBlock in _textBlocks)
            {
                if (!blockOffsets.ContainsKey(textBlock.Alignment))
                {
                    blockOffsets.Add(textBlock.Alignment, 5);
                }

                var format = new StringFormat
                {
                    LineAlignment = StringAlignment.Near,
                    Alignment = textBlock.Alignment
                };

                foreach (string line in textBlock.Content)
                {
                    switch (textBlock.Alignment)
                    {
                        case StringAlignment.Near:
                            graphics.DrawString(line, _font, _brush, 5, blockOffsets[textBlock.Alignment]);
                            break;
                        case StringAlignment.Center:
                            graphics.DrawString(line, _font, _brush, RenderUtils.ScreenWidth * 0.5f, blockOffsets[textBlock.Alignment], format);
                            break;
                        case StringAlignment.Far:
                            graphics.DrawString(line, _font, _brush, RenderUtils.ScreenWidth - 5, blockOffsets[textBlock.Alignment], format);
                            break;
                    }

                    blockOffsets[textBlock.Alignment] += 30;
                }

                blockOffsets[textBlock.Alignment] += 20;
            }
        }
    }

    class TextBlock
    {
        public StringAlignment Alignment { get; set; }

        public List<string> Content { get; set; }
    }
}
