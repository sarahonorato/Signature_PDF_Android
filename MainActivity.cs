using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using iTextSharp.text.pdf;
using iTextSharp.text;
using SignaturePad;
using System.IO;
using Android.Graphics;
using AssinaGravaPDF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssinaGravaPDF
{
    [Activity(Label = "Assinatura e PDF", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<Produto> ListaImprimir = new List<Produto>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            RequestWindowFeature(WindowFeatures.ActionBar);

            // Definição da ActionBar
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
            ActionBar.SetDisplayShowTitleEnabled(true);

            CarregaPainelAssinatura();
            CarregaPedido();

            Button btnSalvar = FindViewById<Button>(Resource.Id.btnSalvar);
            btnSalvar.Click += async delegate
            {
                ProgressDialog mDialog = new ProgressDialog(this);
                mDialog.SetMessage("Gerando PDF");
                mDialog.Show();
                await Task.Run(() =>
                {
                    SalvaPedidoPDF();
                });
                mDialog.Dismiss();
                ListaImprimir.Clear();
                Finish();
            };
        }

        private void CarregaPainelAssinatura()
        {
            SignaturePadView signature = FindViewById<SignaturePadView>(Resource.Id.signatureView);
            signature.Caption.Text = "Sara A. H.";
            signature.Caption.SetTypeface(Typeface.Serif, TypefaceStyle.BoldItalic);
            signature.Caption.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 16f);
            signature.SignaturePrompt.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            signature.SignaturePrompt.SetTextSize(global::Android.Util.ComplexUnitType.Sp, 16f);
            signature.StrokeColor = Android.Graphics.Color.Black;
            signature.BackgroundColor = Android.Graphics.Color.WhiteSmoke;
            signature.ClearLabel.Text = "Limpar";

            TextView caption = signature.Caption;
            caption.SetPadding(caption.PaddingLeft, 1, caption.PaddingRight, 10);
        }

        private void CarregaPedido()
        {
            try
            {
                Produto produto001 = new Produto();
                produto001.ID_CADMAT = 1;
                produto001.DESCR_MAT_CADMAT = "Batata Frita";
                produto001.PRECO_UNIT = "4,99";
                produto001.QTD_CADMAT = 1;
                produto001.UN_MEDIDA = "Unidade";
                ListaImprimir.Add(produto001);

                Produto produto002 = new Produto();
                produto002.ID_CADMAT = 2;
                produto002.DESCR_MAT_CADMAT = "Suco de laranja";
                produto002.PRECO_UNIT = "2,50";
                produto002.QTD_CADMAT = 1;
                produto002.UN_MEDIDA = "Copo (300ml)";
                ListaImprimir.Add(produto002);

                ListView lstItemPedido = FindViewById<ListView>(Resource.Id.lstItemPedido);
                lstItemPedido.Adapter = new ListaImprimirPedidoAdapter(this, ListaImprimir);

                TextView txtTotal = FindViewById<TextView>(Resource.Id.txtTotal);
                txtTotal.Text = "7,49";
            }
            catch (System.Exception ex)
            {
                Toast toast = Toast.MakeText(this, "Erro: " + ex.Message, ToastLength.Long);
                toast.SetGravity(GravityFlags.CenterVertical | GravityFlags.CenterHorizontal, 0, 0);
                toast.Show();
            }
        }

        private void SalvaPedidoPDF()
        {

            try
            {
                //O PDF sempre será salvo no sdcard
                string caminho = "mnt/sdcard/Download/invoice.pdf";
                System.IO.FileStream fs = new System.IO.FileStream(caminho, System.IO.FileMode.Create);

                //Create an instance of the document class which represents the PDF document itself.
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);

                //Create an instance to the PDF file by creating an instance of the PDF Writer class, using the document and the filestrem in the constructor.
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                //Open the document to enable you to write to the document
                document.Open();

                //Adiciona título
                Paragraph pgTitulo = new Paragraph();
                pgTitulo.Add("Pedido");
                pgTitulo.Alignment = Element.ALIGN_CENTER;
                BaseFont bfTitulo = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                pgTitulo.Font = new Font(bfTitulo, Font.BOLD);
                document.Add(pgTitulo);


                // Add a simple and well known phrase to the document in a flow layout manner
                Paragraph pgPedido = new Paragraph("Pedido: " + FindViewById<TextView>(Resource.Id.txtNumOrdem).Text);
                pgPedido.Add("     ");
                pgPedido.Add("Data: " + FindViewById<TextView>(Resource.Id.txtData).Text);
                pgPedido.Add("     ");
                pgPedido.Add("Hora: " + FindViewById<TextView>(Resource.Id.txtHora).Text);
                pgPedido.Alignment = Element.ALIGN_LEFT;
                pgPedido.SpacingBefore = 20;
                document.Add(pgPedido);

                Paragraph pgNome = new Paragraph("Cliente: " + FindViewById<TextView>(Resource.Id.txtNome).Text);
                pgNome.Alignment = Element.ALIGN_LEFT;
                document.Add(pgNome);               

                Paragraph pgObservacao = new Paragraph("Observação Geral: " + FindViewById<TextView>(Resource.Id.txtObsGeral).Text);
                pgObservacao.Alignment = Element.ALIGN_LEFT;
                document.Add(pgObservacao);

                PdfPTable myTable = new PdfPTable(3);
                myTable.WidthPercentage = 100;
                myTable.HorizontalAlignment = 0;
                myTable.SpacingBefore = 20;
                myTable.SpacingAfter = 10;
                float[] sglTblHdWidths = new float[3];
                sglTblHdWidths[0] = 300f;
                sglTblHdWidths[1] = 60f;
                sglTblHdWidths[2] = 70f;
                myTable.SetWidths(sglTblHdWidths);

                PdfPCell CellOneHdr = new PdfPCell(new Phrase("Produto"));
                CellOneHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(CellOneHdr);
                PdfPCell CellTwoHdr = new PdfPCell(new Phrase("Qtde"));
                CellTwoHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(CellTwoHdr);
                PdfPCell CellFourHdr = new PdfPCell(new Phrase("Preço"));
                CellFourHdr.HorizontalAlignment = Element.ALIGN_CENTER;
                myTable.AddCell(CellFourHdr);

                foreach (Produto produto in ListaImprimir)
                {
                    PdfPCell CellOne = new PdfPCell(new Phrase(produto.DESCR_MAT_CADMAT));
                    CellOne.HorizontalAlignment = Element.ALIGN_LEFT;
                    myTable.AddCell(CellOne);
                    PdfPCell CellTwo = new PdfPCell(new Phrase(produto.QTD_CADMAT.ToString()));
                    CellTwo.HorizontalAlignment = Element.ALIGN_CENTER;
                    myTable.AddCell(CellTwo);
                    PdfPCell CellFour = new PdfPCell(new Phrase(Convert.ToDouble(produto.PRECO_UNIT).ToString("N2")));
                    CellFour.HorizontalAlignment = Element.ALIGN_RIGHT;
                    myTable.AddCell(CellFour);
                }

                document.Add(myTable);

                Paragraph pgTotal = new Paragraph("Total do Pedido: " + FindViewById<TextView>(Resource.Id.txtTotal).Text);
                pgTotal.Alignment = Element.ALIGN_RIGHT;
                pgTotal.SpacingAfter = 30;
                document.Add(pgTotal);

                // Converte a assinatura para byte, e depois para iTextImage
                SignaturePadView signature = FindViewById<SignaturePadView>(Resource.Id.signatureView);
                Android.Graphics.Bitmap imagen = signature.GetImage(Android.Graphics.Color.Black, Android.Graphics.Color.White);
                MemoryStream fstream = new MemoryStream();
                imagen.Compress(Bitmap.CompressFormat.Jpeg, 100, fstream);
                byte[] result = fstream.ToArray();
                fstream.Flush();

                //Adiciona assinatura ao documento
                Image imgAssinatura = Image.GetInstance(result);
                imgAssinatura.ScaleAbsolute(300f, 30f);
                imgAssinatura.Alignment = Element.ALIGN_CENTER;
                imgAssinatura.SpacingBefore = 20;
                document.Add(imgAssinatura);

                Paragraph pgAssinatura = new Paragraph();
                pgAssinatura.Add("Cliente: Sara A. H.");
                pgAssinatura.Alignment = Element.ALIGN_CENTER;
                document.Add(pgAssinatura);

                //Adiciona Copyright
                Paragraph pgFooter = new Paragraph();
                pgFooter.Add("Sara Honorato © 2016 All rights reserved ");
                pgFooter.Alignment = Element.ALIGN_RIGHT;
                pgFooter.SpacingBefore = 20;
                pgFooter.SpacingAfter = 20;
                document.Add(pgFooter);

                // Close the document
                document.Close();

                // Close the writer instance
                writer.Close();

                // Always close open file handles explicitly
                fs.Close();

                // Abre o pdf salvo
                OpenPdf(caminho);
            }
            catch (System.Exception ex)
            {
                Toast toast = Toast.MakeText(this, "Erro: " + ex.Message, ToastLength.Long);
                toast.SetGravity(GravityFlags.CenterVertical | GravityFlags.CenterHorizontal, 0, 0);
                toast.Show();
            }
        }

        private void OpenPdf(string filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + filePath);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "application/pdf");
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            try
            {
                StartActivity(intent);
            }
            catch (System.Exception ex)
            {
                Toast toast = Toast.MakeText(this, "Erro: " + ex.Message, ToastLength.Long);
                toast.SetGravity(GravityFlags.CenterVertical | GravityFlags.CenterHorizontal, 0, 0);
                toast.Show();
            }
        }
    }

    public class ListaImprimirPedidoAdapter : BaseAdapter<Produto>
    {
        List<Produto> ListaPedido;
        Activity context;
        public ListaImprimirPedidoAdapter(Activity context, List<Produto> ListaPedido)
       : base()
        {
            this.context = context;
            this.ListaPedido = ListaPedido;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override int Count
        {
            get { return ListaPedido.Count; }
        }

        public override Produto this[int position]
        {
            get { return ListaPedido[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = ListaPedido[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ListItemPedido, null);
            //=== Busca os itens na view filho para preencher os valores
            view.FindViewById<TextView>(Resource.Id.ViewIDProdutoPedido).Text = item.ID_CADMAT.ToString();
            view.FindViewById<TextView>(Resource.Id.ViewDescricaoProdutoPedido).Text = item.DESCR_MAT_CADMAT;
            view.FindViewById<EditText>(Resource.Id.ViewQtdeProdutoPedido).Text = item.QTD_CADMAT.ToString();
            view.FindViewById<EditText>(Resource.Id.ViewQtdeProdutoPedido).Enabled = false;
            view.FindViewById<TextView>(Resource.Id.ViewPrecoProdutoPedido).Text = Convert.ToDouble(item.PRECO_UNIT).ToString("N2");
            return view;
        }
    }

}

