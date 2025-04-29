using DocumentFormat.OpenXml.Wordprocessing;

namespace StaffApp.Application.Extensions.Helpers.OpenXML
{
    public static class OpenXMLTableHelper
    {
        public static void SetAllTableCellsFontSize(IEnumerable<Table> tables, int fontSize = 10)
        {
            if (tables == null) return;

            foreach (var table in tables)
            {
                foreach (var row in table.Elements<TableRow>())
                {
                    foreach (var cell in row.Elements<TableCell>())
                    {
                        SetCellFontSize(cell, 10);
                    }
                }
            }
        }

        public static void SetSpecificTableFontSize(Table table, int fontSize = 10)
        {
            foreach (var row in table.Elements<TableRow>())
            {
                foreach (var cell in row.Elements<TableCell>())
                {
                    SetCellFontSize(cell, 10);
                }
            }
        }

        public static void SetSpecificRowFontSize(Table table, int rowIndex, int cellIndex, int fontSize = 10)
        {
            // Get the specified row
            var rows = table.Elements<TableRow>().ToList();
            if (rowIndex >= rows.Count) return;
            TableRow row = rows[rowIndex];

            // Get the specified cell
            var cells = row.Elements<TableCell>().ToList();
            if (cellIndex >= cells.Count) return;
            TableCell cell = cells[cellIndex];

            SetCellFontSize(cell, 10);
        }

        public static void SetCellFontSize(TableCell cell, int fontSize)
        {
            // Word stores font size in half-points, so 10pt = 20 half-points
            string fontSizeInHalfPoints = (fontSize * 2).ToString();

            // Process each paragraph in the cell
            foreach (var paragraph in cell.Elements<Paragraph>())
            {
                // Process each run in the paragraph
                foreach (var run in paragraph.Elements<Run>())
                {
                    // Get or create run properties
                    RunProperties runProps = run.Elements<RunProperties>().FirstOrDefault();
                    if (runProps == null)
                    {
                        runProps = new RunProperties();
                        run.InsertAt(runProps, 0);
                    }

                    // Set or update font size
                    FontSize size = runProps.Elements<FontSize>().FirstOrDefault();
                    if (size != null)
                    {
                        size.Val = fontSizeInHalfPoints;
                    }
                    else
                    {
                        runProps.AppendChild(new FontSize { Val = fontSizeInHalfPoints });
                    }
                }
            }
        }

        public static void SetHorizontalTextAlignment(this TableCell cell, JustificationValues alignment)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            TableCellProperties cellProperties = cell.GetOrCreateTableCellProperties();
            SetParagraphAlignment(cell, alignment);
        }

        /// <summary>
        /// Sets the vertical text alignment for a table cell
        /// </summary>
        /// <param name="cell">The table cell to set alignment for</param>
        /// <param name="alignment">The vertical alignment value</param>
        public static void SetVerticalTextAlignment(this TableCell cell, TableVerticalAlignmentValues alignment)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            TableCellProperties cellProperties = cell.GetOrCreateTableCellProperties();

            // Find or create the vertical alignment property
            TableCellVerticalAlignment? verticalAlignment = cellProperties.Elements<TableCellVerticalAlignment>().FirstOrDefault();
            if (verticalAlignment == null)
            {
                verticalAlignment = new TableCellVerticalAlignment();
                cellProperties.AppendChild(verticalAlignment);
            }

            verticalAlignment.Val = alignment;
        }

        public static void SetTextAlignment(this TableCell cell, JustificationValues horizontalAlignment, TableVerticalAlignmentValues verticalAlignment)
        {
            SetHorizontalTextAlignment(cell, horizontalAlignment);
            SetVerticalTextAlignment(cell, verticalAlignment);
        }

        public static void RemoveBoldFromTableCell(TableCell cell)
        {
            if (cell == null) return;

            foreach (var paragraph in cell.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    var runProperties = run.GetFirstChild<RunProperties>();

                    if (runProperties != null)
                    {
                        var bold = runProperties.GetFirstChild<Bold>();
                        bold = new Bold() { Val = false };
                        //if (bold != null)
                        //{
                        //    bold.Remove(); // remove <w:b>
                        //}

                        // Also check for complex script bold <w:bCs>
                        var boldCs = runProperties.GetFirstChild<BoldComplexScript>();
                        if (boldCs != null)
                        {
                            boldCs = new BoldComplexScript { Val = false };
                        }
                    }
                }
            }
        }

        public static void AddBoldToTableCell(TableCell cell)
        {
            if (cell == null) return;

            foreach (var paragraph in cell.Elements<Paragraph>())
            {
                foreach (var run in paragraph.Elements<Run>())
                {
                    // Get or create RunProperties
                    var runProperties = run.GetFirstChild<RunProperties>();
                    if (runProperties == null)
                    {
                        runProperties = new RunProperties();
                        run.PrependChild(runProperties);
                    }

                    // Add or update <w:b>
                    var bold = runProperties.GetFirstChild<Bold>();
                    if (bold == null)
                    {
                        runProperties.Append(new Bold());
                    }

                    // Add complex script bold <w:bCs> if needed
                    var boldCs = runProperties.GetFirstChild<BoldComplexScript>();
                    if (boldCs == null)
                    {
                        runProperties.Append(new BoldComplexScript());
                    }
                }
            }
        }

        private static TableCellProperties GetOrCreateTableCellProperties(this TableCell cell)
        {
            TableCellProperties? properties = cell.Elements<TableCellProperties>().FirstOrDefault();
            if (properties == null)
            {
                properties = new TableCellProperties();
                cell.PrependChild(properties);
            }
            return properties;
        }

        private static void SetParagraphAlignment(TableCell cell, JustificationValues alignment)
        {
            // Get all paragraphs in the cell
            var paragraphs = cell.Elements<Paragraph>().ToList();

            // If no paragraphs exist, create one
            if (!paragraphs.Any())
            {
                var paragraph = new Paragraph();
                cell.AppendChild(paragraph);
                paragraphs.Add(paragraph);
            }

            // Set alignment for each paragraph
            foreach (var paragraph in paragraphs)
            {
                ParagraphProperties properties = paragraph.GetOrCreateParagraphProperties();
                Justification justification = properties.Elements<Justification>().FirstOrDefault();

                if (justification == null)
                {
                    justification = new Justification();
                    properties.AppendChild(justification);
                }

                justification.Val = alignment;
            }
        }

        private static ParagraphProperties GetOrCreateParagraphProperties(this Paragraph paragraph)
        {
            ParagraphProperties? properties = paragraph.Elements<ParagraphProperties>().FirstOrDefault();
            if (properties == null)
            {
                properties = new ParagraphProperties();
                paragraph.PrependChild(properties);
            }
            return properties;
        }
    }
}
