using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using SUT.PrintEngine.Utils;

namespace SUT.PrintEngine.Paginators
{
    public class ItemsPaginator : VisualPaginator
    {
        protected PrintTableDefination PrintTableDefination;
        protected IList<int> ColumnCount;

        public ItemsPaginator(DrawingVisual source, Size printSize, Thickness pageMargins, PrintTableDefination printTableDefination)
            : base(source, printSize, pageMargins, pageMargins)
        {
            PrintTableDefination = printTableDefination;
            ColumnCount = new List<int>();
            CalculateHeaderHeight();
            //CalculateSummaryHeight();
        }

        private void CalculateHeaderHeight()
        {
            var header = XamlReader.Parse(PrintTableDefination.HeaderTemplate) as FrameworkElement;
            UiUtil.UpdateSize(header, PrintablePageWidth);
            HeaderHeight = header.ActualHeight + PageMargins.Top+PrintTableDefination.ColumnHeaderHeight;
        }

        //private void CalculateSummaryHeight()
        //{
        //    var summary = XamlReader.Parse(PrintTableDefination.SummaryTemplate) as FrameworkElement;
        //    UiUtil.UpdateSize(summary, PrintablePageWidth);
        //    SummaryHeight = summary.ActualHeight + PageMargins.Top + PrintTableDefination.ColumnHeaderHeight;
        //}

        protected override int GetVerticalPageCount()
        {
            var pageCountY = 0;
            double totalHeight = 0; // SummaryHeight * PrintTableDefination.Scale;
            double lastTotalHeight = 0;
            for (var i = 0; i < PrintTableDefination.RowHeights.Count; i++)
            {
                lastTotalHeight = totalHeight + PrintTableDefination.RowHeights[i] * PrintTableDefination.Scale;
                if (totalHeight + PrintTableDefination.RowHeights[i] * PrintTableDefination.Scale <= PrintablePageHeight - HeaderHeight)
                {
                    totalHeight += PrintTableDefination.RowHeights[i] * PrintTableDefination.Scale;
                }
                else
                {
                    pageCountY++;
                    AdjustedPageHeights.Add(totalHeight);
                    totalHeight = 0;
                    i--;
                }

            }
            AdjustedPageHeights.Add(lastTotalHeight);
            return pageCountY + 1;
        }

        protected override Rect GetPageBounds(int horizontalPageNumber, int verticalPageNumber, float horizontalOffset, float verticalOffset)
        {
            verticalOffset = 0;
            for (var i = 0; i < horizontalPageNumber; i++)
            {
                horizontalOffset += (float)GetPageWidth(i);
            }
            for (var j = 0; j < verticalPageNumber; j++)
            {
                verticalOffset += (float)AdjustedPageHeights[j];
            }
            var pageBounds = new Rect
                                 {
                                     X = horizontalOffset,
                                     Y = verticalOffset,
                                     Size = new Size(GetPageWidth(horizontalPageNumber)+ 2, AdjustedPageHeights[verticalPageNumber] + 2)
                                 };
            return pageBounds;
        }

        protected virtual double GetPageWidth(int pageNumber)
        {
            return PrintablePageWidth;            
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var page = base.GetPage(pageNumber);
            var headerVisual = new DrawingVisual();
            using (var drawingContext = headerVisual.RenderOpen())
            {
                var rowNumber = pageNumber % HorizontalPageCount;
                var contentWidth = GetPageWidth(rowNumber);

                CreateHeader(rowNumber,drawingContext,pageNumber+1);
                if (PrintTableDefination.HasFooter)
                {
                    var text3 = new FormattedText(PrintTableDefination.FooterText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black);
                    drawingContext.DrawText(text3, new Point(PageMargins.Left, PageSize.Height - PageMargins.Bottom - text3.Height));
                }

                var contentTop = PageMargins.Top + HeaderHeight;
                //if(pageNumber < HorizontalPageCount)
                //{
                //    contentTop += SummaryHeight;
                //}
                var gridLineBrush = Brushes.Gray;
                const double gridLineThickness = 0.5;
                var gridLinePen = new Pen(gridLineBrush, gridLineThickness);

                if (PrintTableDefination.ColumnNames != null)
                {                    
                    drawingContext.DrawRectangle(Brushes.Transparent, gridLinePen, new Rect(PageMargins.Left - 1, contentTop - PrintTableDefination.ColumnHeaderHeight, contentWidth, PrintTableDefination.ColumnHeaderHeight));
                    drawingContext.DrawRectangle(gridLineBrush, gridLinePen, new Rect(PageMargins.Left - 1, contentTop - 2, contentWidth, 2));

                    var cumilativeColumnNumber = 0;
                    var columnLeft = PageMargins.Left - 1;
                    var currentPageColumns = rowNumber == HorizontalPageCount - 1 ? ColumnCount[rowNumber] - 1 : ColumnCount[rowNumber];
                    for (int j = 0; j < rowNumber; j++)
                    {
                        cumilativeColumnNumber += ColumnCount[j];
                    }
                    for (int i = cumilativeColumnNumber; i < cumilativeColumnNumber + currentPageColumns; i++)
                    {
                        var columnWidth = PrintTableDefination.ColumnWidths[i] * PrintTableDefination.Scale;
                        var colName = PrintTableDefination.ColumnNames[i];
                        if (colName == string.Empty)
                        {
                            drawingContext.DrawRectangle(gridLineBrush, gridLinePen, new Rect(columnLeft, contentTop - PrintTableDefination.ColumnHeaderHeight, columnWidth, PrintTableDefination.ColumnHeaderHeight));
                        }
                        else
                        {
                            var columnName = new FormattedText(colName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("SimSun"), PrintTableDefination.ColumnHeaderFontSize, PrintTableDefination.ColumnHeaderBrush)
                                                 {
                                                     MaxTextWidth = columnWidth,
                                                     MaxLineCount = 1,
                                                     Trimming = TextTrimming.CharacterEllipsis
                                                 };
                            drawingContext.DrawText(columnName, new Point(columnLeft + 5, contentTop - PrintTableDefination.ColumnHeaderHeight + 3));
                        }
                        columnLeft += columnWidth;
                        drawingContext.DrawRectangle(gridLineBrush, gridLinePen, new Rect(columnLeft, contentTop - PrintTableDefination.ColumnHeaderHeight, gridLineThickness, PrintTableDefination.ColumnHeaderHeight));

                    }
                    if (rowNumber == HorizontalPageCount - 1)
                    {
                        var columnWidth =
                            PrintTableDefination.ColumnWidths[cumilativeColumnNumber + ColumnCount[rowNumber] - 1];
                        var colName =
                            PrintTableDefination.ColumnNames[cumilativeColumnNumber + ColumnCount[rowNumber] - 1];
                        if (colName == string.Empty)
                        {
                            drawingContext.DrawRectangle(gridLineBrush, gridLinePen, new Rect(columnLeft, contentTop - PrintTableDefination.ColumnHeaderHeight - 2, columnWidth, PrintTableDefination.ColumnHeaderHeight));
                        }
                        else
                        {
                            var columnName = new FormattedText(colName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), PrintTableDefination.ColumnHeaderFontSize, Brushes.Black)
                                                 {
                                                     MaxTextWidth = columnWidth,
                                                     MaxLineCount = 1,
                                                     Trimming = TextTrimming.CharacterEllipsis
                                                 };
                            drawingContext.DrawText(columnName, new Point(columnLeft + 5, contentTop - PrintTableDefination.ColumnHeaderHeight + 3));
                        }
                    }
                }
                //if (ShowPageMarkers)
                //{
                //    var pageNumberText = new FormattedText(string.Format("{0}", pageNumber + 1), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12, gridLineBrush);
                //    drawingContext.DrawText(pageNumberText, new Point(PageMargins.Left + 5, PrintablePageHeight - pageNumberText.Height + 15));
                //}
                drawingContext.PushOpacityMask(Brushes.White);
            }
                      
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(headerVisual.Drawing);
            drawingGroup.Children.Add(((DrawingVisual)page.Visual).Drawing);
            
            var currentDrawingVisual = (DrawingVisual)page.Visual;
            currentDrawingVisual.Transform = new TranslateTransform(PageMargins.Left, PageMargins.Top);
            
            var currentDrawingContext = currentDrawingVisual.RenderOpen();
            currentDrawingContext.DrawDrawing(drawingGroup);
            currentDrawingContext.PushOpacityMask(Brushes.White);
            currentDrawingContext.Close();
            var documentPage = new DocumentPage(currentDrawingVisual, PageSize, FrameRect, FrameRect);
            OnGetPageCompleted(new GetPageCompletedEventArgs(documentPage, pageNumber, null, false, null));
            return documentPage;
        }

        private void CreateHeader(int index, DrawingContext drawingContext, int pageNumber)
        {
            if (!string.IsNullOrEmpty(PrintTableDefination.HeaderTemplate) && index==0)
            {
                var headerTemplate = PrintTableDefination.HeaderTemplate.Replace("@PageNumber", pageNumber.ToString()).Replace("@TotalPageNumber", _verticalPageCount.ToString());
                var header = XamlReader.Parse(headerTemplate) as FrameworkElement;
                header.Width = PrintablePageWidth;
                UiUtil.UpdateSize(header, PrintablePageWidth);
                drawingContext.DrawRectangle(new VisualBrush(header) { Stretch = Stretch.None }, null, new Rect(PageMargins.Left, PageMargins.Top, PrintablePageWidth, header.ActualHeight));

                //if (pageNumber == 1)
                //{                    
                //var summaryTemplate = PrintTableDefination.SummaryTemplate.Replace("@PageNumber", "This is Summary");
                //var summary = XamlReader.Parse(summaryTemplate) as FrameworkElement;
                //summary.Width = PrintablePageWidth;
                //UiUtil.UpdateSize(summary, PrintablePageWidth);
                //drawingContext.DrawRectangle(new VisualBrush(summary) { Stretch = Stretch.None }, null, new Rect(PageMargins.Left, PageMargins.Top + header.ActualHeight, PrintablePageWidth, summary.ActualHeight + header.ActualHeight));
                //}
            }
        }

        protected override void InsertPageMarkers(int pageNumber, DocumentPage documentPage)
        {
        }
    }
}