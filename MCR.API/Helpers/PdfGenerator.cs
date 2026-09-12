using iTextSharp.text;
using iTextSharp.text.pdf;
using MCR.API.CotacoesAgricola.Domain.DTO;
using MCR.API.Shared.Extensions;

namespace MCR.API.Helpers
{
    public class HeaderFooterPageEvent : PdfPageEventHelper
    {
        private Font footerFont;

        public HeaderFooterPageEvent(Font footerFont)
        {
            this.footerFont = footerFont;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.DefaultCell.Border = Rectangle.NO_BORDER;
            footerTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            PdfPCell footerCell = new PdfPCell(new Phrase("Powered by MCR", footerFont));
            footerCell.Border = Rectangle.NO_BORDER;
            footerCell.HorizontalAlignment = Element.ALIGN_LEFT;
            footerCell.PaddingBottom = 10f;
            footerTable.AddCell(footerCell);

            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin + 10, writer.DirectContent);
        }
    }

    public static class PdfGenerator
    {
        public static byte[] GerarPDF(CotacaoAgricolaPdfDTO model)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document documento = new Document(PageSize.A4, 20f, 20f, 20f, 30f);
                PdfWriter writer = PdfWriter.GetInstance(documento, ms);

                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);
                writer.PageEvent = new HeaderFooterPageEvent(smallFont);

                documento.Open();

                Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Font normalBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);
                Font smallBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);
                Font spacingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 5);

                Paragraph titulo = new Paragraph("COTAÇÃO SEGURO AGRÍCOLA", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                documento.Add(titulo);
                documento.Add(new Paragraph(" ", normalFont));

                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 9f, 3f });

                PdfPCell emptyCell = new PdfPCell(new Phrase(" ", normalFont));
                emptyCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(emptyCell);

                var cotacaoPhrase = new Phrase();
                cotacaoPhrase.Add(new Chunk($"COTAÇÃO: {model.NumeroCotacao}\n", normalBoldFont));
                cotacaoPhrase.Add(new Chunk(model.DataCotacao.ToString("dd/MM/yyyy HH:mm"), smallFont));

                PdfPCell cotacaoCell = new PdfPCell(cotacaoPhrase);
                cotacaoCell.HorizontalAlignment = Element.ALIGN_CENTER;
                cotacaoCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cotacaoCell.Border = Rectangle.BOX;
                cotacaoCell.PaddingTop = 5f;
                cotacaoCell.PaddingBottom = 5f;
                headerTable.AddCell(cotacaoCell);

                documento.Add(headerTable);
                documento.Add(new Paragraph(" ", spacingFont));

                AddSecaoAtendimento(documento, model, subtituloFont, normalFont, normalBoldFont);
                documento.Add(new Paragraph(" ", spacingFont));

                AddSecaoRisco(documento, model, subtituloFont, normalFont, normalBoldFont);
                documento.Add(new Paragraph(" ", spacingFont));

                AddSecaoSeguro(documento, model, subtituloFont, normalFont, normalBoldFont);
                documento.Add(new Paragraph(" ", spacingFont));

                AddTabelaCoberturas(documento, model, subtituloFont, normalFont, normalBoldFont, smallFont, smallBoldFont);
                documento.Add(new Paragraph(" ", spacingFont));

                AddTabelaPremios(documento, model, subtituloFont, normalFont, normalBoldFont, smallFont, smallBoldFont);
                documento.Add(new Paragraph(" ", spacingFont));

                AddSecaoInformacoesAdicionais(documento, subtituloFont, normalFont);

                documento.Close();
                return ms.ToArray();
            }
        }

        private static void AddSecaoAtendimento(Document doc, CotacaoAgricolaPdfDTO model, Font subtituloFont, Font normalFont, Font normalBoldFont)
        {
            Paragraph titulo = new Paragraph("DADOS DO ATENDIMENTO", subtituloFont);
            titulo.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.Black, Element.ALIGN_LEFT, -2f)));
            doc.Add(titulo);

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 4f, 1.5f, 3f });
            table.SpacingBefore = 4f;

            AddCellPair(table, "CORRETORA", model.Corretora, normalBoldFont, normalFont);
            AddCellPair(table, "CONSULTOR", model.Consultor, normalBoldFont, normalFont);
            AddCellPair(table, "CANAL", model.Canal, normalBoldFont, normalFont);
            AddCellPair(table, "CONTATO", model.Contato?.FormatCelular(), normalBoldFont, normalFont);
            AddCellPair(table, "P.A.", model.PA, normalBoldFont, normalFont);
            AddCellPair(table, "E-MAIL", model.Email, normalBoldFont, normalFont);

            doc.Add(table);
        }

        private static void AddSecaoRisco(Document doc, CotacaoAgricolaPdfDTO model, Font subtituloFont, Font normalFont, Font normalBoldFont)
        {
            Paragraph titulo = new Paragraph("DADOS DO RISCO", subtituloFont);
            titulo.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.Black, Element.ALIGN_LEFT, -2f)));
            doc.Add(titulo);

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 3f, 2f, 1.5f });
            table.SpacingBefore = 4f;

            AddCellPair(table, "CULTURA", model.Cultura, normalBoldFont, normalFont);
            AddCellPair(table, "TIPO SOLO", model.TipoSolo, normalBoldFont, normalFont);
            AddCellPair(table, "SAFRA", model.Safra, normalBoldFont, normalFont);
            AddCellPair(table, "CLASSIFICAÇÃO SOLO", model.ClassificacaoSolo, normalBoldFont, normalFont);
            AddCellPair(table, "UF", model.UF, normalBoldFont, normalFont);
            AddCellPair(table, "PLANTIO CONSORCIADO", model.PlantioConsorciado, normalBoldFont, normalFont);
            AddCellPair(table, "MUNICÍPIO", model.Municipio, normalBoldFont, normalFont);
            AddCellPair(table, "PLANTIO CONVENCIONAL", model.PlantioDireto, normalBoldFont, normalFont);
            AddCellPair(table, "CPF", model.CPF, normalBoldFont, normalFont);
            AddCellPair(table, "PLANTIO PÓS CANA", model.PlantioPosCanal, normalBoldFont, normalFont);
            AddCellPair(table, "CLIENTE", model.NomeCliente, normalBoldFont, normalFont);
            AddCellPair(table, "LAVOURA IRRIGADA", model.LavouraIrrigada, normalBoldFont, normalFont);
            AddCellPair(table, "ÁREA TOTAL", model.AreaTotal, normalBoldFont, normalFont);
            AddCellPair(table, "", " ", normalBoldFont, normalFont);

            doc.Add(table);
        }

        private static void AddSecaoSeguro(Document doc, CotacaoAgricolaPdfDTO model, Font subtituloFont, Font normalFont, Font normalBoldFont)
        {
            Paragraph titulo = new Paragraph("DADOS DO SEGURO", subtituloFont);
            titulo.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.Black, Element.ALIGN_LEFT, -2f)));
            doc.Add(titulo);

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 3f, 2f, 3f });
            table.SpacingBefore = 4f;

            AddCellPair(table, "MODALIDADE", model.Modalidade, normalBoldFont, normalFont);

            if (model.IsModalidadeProdutividade)
                AddCellPair(table, "PREÇO SACA", model.PrecoSaca, normalBoldFont, normalFont);
            else
                AddCellPair(table, "R$ CUSTEIO/HA", model.CusteioHa, normalBoldFont, normalFont);

            doc.Add(table);
        }

        private static void AddTabelaCoberturas(Document doc, CotacaoAgricolaPdfDTO model, Font subtituloFont, Font normalFont, Font normalBoldFont, Font smallFont, Font smallBoldFont)
        {
            Paragraph titulo = new Paragraph("COBERTURAS", subtituloFont);
            doc.Add(titulo);

            var coberturas = model.Coberturas.OrderBy(c => c.OpcaoSeguradora).ToList();
            int numeroOpcoes = coberturas.Count;

            float[] widths = new float[numeroOpcoes + 1];
            widths[0] = 2.5f;
            for (int i = 1; i <= numeroOpcoes; i++)
                widths[i] = 1.5f;

            PdfPTable table = new PdfPTable(numeroOpcoes + 1);
            table.WidthPercentage = 100;
            table.SetWidths(widths);
            table.SpacingBefore = 4f;

            PdfPCell headerCell = new PdfPCell(new Phrase("COBERTURAS", normalBoldFont));
            headerCell.BackgroundColor = new BaseColor(248, 249, 250);
            headerCell.HorizontalAlignment = Element.ALIGN_LEFT;
            headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            headerCell.Padding = 5f;
            table.AddCell(headerCell);

            for (int i = 0; i < numeroOpcoes; i++)
            {
                headerCell = new PdfPCell(new Phrase($"OPÇÃO {(i + 1)}", normalBoldFont));
                headerCell.BackgroundColor = new BaseColor(248, 249, 250);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                headerCell.Padding = 5f;
                table.AddCell(headerCell);
            }

            AddCoberturaRow(table, "TIPO DA OFERTA", c => c.TipoOferta ?? "Municipal", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "SEGURADORA", c => c.Seguradora, coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "NOME PRODUTO", c => c.NomeProduto, coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "REGULAÇÃO DO SINISTRO", c => c.RegulacaoSinistro, coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "PRODUTIVIDADE ESPERADA KG/HA", c => c.ProdutividadeEsperada, coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "NÍVEL DE COBERTURA", c => $"{c.NivelCobertura}%", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "PRODUTIVIDADE SEGURADA KG/HA", c => c.ProdutividadeSegurada.ToString() ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "LMI DE PRODUÇÃO/HECTARE", c => c.LMIProducaoHectare.FormatarMoeda() ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "LMI DE REPLANTIO/HECTARE", c => c.LMIReplantioHectare.FormatarMoeda() ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "LMI DE PRODUÇÃO TOTAL", c => c.LMIProducaoTotal.FormatarMoeda() ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "LMI DE REPLANTIO TOTAL", c => c.LMIReplantioTotal.FormatarMoeda() ?? "-", coberturas, normalBoldFont, normalFont);

            doc.Add(table);
        }

        private static void AddTabelaPremios(Document doc, CotacaoAgricolaPdfDTO model, Font subtituloFont, Font normalFont, Font normalBoldFont, Font smallFont, Font smallBoldFont)
        {
            Paragraph titulo = new Paragraph("PRÊMIO/CUSTO DO SEGURO", subtituloFont);
            doc.Add(titulo);

            var coberturas = model.Coberturas.OrderBy(c => c.OpcaoSeguradora).ToList();
            int numeroOpcoes = coberturas.Count;

            float[] widths = new float[numeroOpcoes + 1];
            widths[0] = 2.5f;
            for (int i = 1; i <= numeroOpcoes; i++)
                widths[i] = 1.5f;

            PdfPTable table = new PdfPTable(numeroOpcoes + 1);
            table.WidthPercentage = 100;
            table.SetWidths(widths);
            table.SpacingBefore = 4f;

            PdfPCell headerCell = new PdfPCell(new Phrase("PRÊMIO/CUSTO DO SEGURO", normalBoldFont));
            headerCell.BackgroundColor = new BaseColor(248, 249, 250);
            headerCell.HorizontalAlignment = Element.ALIGN_LEFT;
            headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            headerCell.Padding = 5f;
            table.AddCell(headerCell);

            for (int i = 0; i < numeroOpcoes; i++)
            {
                headerCell = new PdfPCell(new Phrase($"OPÇÃO {coberturas[i].OpcaoSeguradora}", normalBoldFont));
                headerCell.BackgroundColor = new BaseColor(248, 249, 250);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                headerCell.Padding = 5f;
                table.AddCell(headerCell);
            }

            AddCoberturaRow(table, "PRÊMIO TOTAL", c => c.PremioTotal ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "SUBVENÇÃO FEDERAL", c => c.SubvencaoFederal ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "SUBVENÇÃO ESTADUAL", c => c.SubvencaoEstadual ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "PARCELA SEGURADO", c => c.ParcelaSegurado ?? "-", coberturas, normalBoldFont, normalFont);
            AddCoberturaRow(table, "CUSTO EM R$/HECTARE", c => {
                var custoHaStr = c.CustoHectare ?? "0";
                var custoHa = decimal.TryParse(System.Text.RegularExpressions.Regex.Replace(custoHaStr, @"[^\d,.-]", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ch) ? ch : 0m;
                var custoAlq = custoHa * 2.42m;
                return $"{custoHa:C2}/ha | {custoAlq:C2}/alq";
            }, coberturas, normalBoldFont, normalFont);

            // Custo em Sacas
            AddCoberturaRow(table, "CUSTO EM SACAS", c => {
                var custoHaStr = c.CustoHectare ?? "0";
                var custoHa = decimal.TryParse(System.Text.RegularExpressions.Regex.Replace(custoHaStr, @"[^\d,.-]", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ch) ? ch : 0m;
                var precoSacaStr = model.PrecoSaca ?? "0";
                var precoSaca = decimal.TryParse(System.Text.RegularExpressions.Regex.Replace(precoSacaStr, @"[^\d,.-]", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ps) ? ps : 1m;
                var sacasHa = precoSaca > 0 ? custoHa / precoSaca : 0;
                var sacasAlq = precoSaca > 0 ? (custoHa * 2.42m) / precoSaca : 0;
                return $"{sacasHa:N2} sacas/ha | {sacasAlq:N2} sacas/alq";
            }, coberturas, normalBoldFont, normalFont);

            doc.Add(table);
        }

        private static void AddSecaoInformacoesAdicionais(Document doc, Font subtituloFont, Font normalFont)
        {
            Paragraph titulo = new Paragraph("INFORMAÇÕES ADICIONAIS", subtituloFont);
            titulo.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.5f, 100f, BaseColor.Black, Element.ALIGN_LEFT, -2f)));
            doc.Add(titulo);
            doc.Add(new Paragraph(" ", normalFont));

            Paragraph info1 = new Paragraph("• Essa Pré-cotação está sujeita a sofrer alterações de acordo com as políticas de subscrição das seguradoras", normalFont);
            doc.Add(info1);

            Paragraph info2 = new Paragraph("• O aceite da proposta dependerá da oferta de capacidade e da análise técnica da seguradora para o referido risco", normalFont);
            doc.Add(info2);
        }

        private static void AddCellPair(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.PaddingBottom = 2.5f;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value ?? "-", valueFont));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.PaddingBottom = 2.5f;
            table.AddCell(valueCell);
        }

        private static void AddHeaderCell(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 4f;
            table.AddCell(cell);
        }

        private static void AddCoberturaRow(PdfPTable table, string label, Func<CoberturaPdfDTO, string> valueSelector,
            List<CoberturaPdfDTO> coberturas, Font labelFont, Font valueFont)
        {
            AddHeaderCell(table, label, labelFont);

            foreach (var cobertura in coberturas)
            {
                PdfPCell cell = new PdfPCell(new Phrase(valueSelector(cobertura) ?? "-", valueFont));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 2.5f;
                table.AddCell(cell);
            }
        }
    }
}
