using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AuthNDecryptService
{
    public class Response
    {
        public Response()
        {

        }
        public void Generate (string filename, string decryptedEmail, string decryptedContent, string key)
        {
            Document nouveauDocument = new Document();
            nouveauDocument.AddAuthor("Generator");
            nouveauDocument.AddCreator("Generator");
            nouveauDocument.AddTitle(filename);
            nouveauDocument.AddCreationDate();

            try
            {
                PdfWriter.GetInstance(nouveauDocument, new FileStream("fichier.pdf", FileMode.Create));
                nouveauDocument.Open();
                nouveauDocument.Add(new Paragraph(filename));
                nouveauDocument.Add(new Paragraph(key));
                nouveauDocument.Add(new Paragraph(decryptedEmail));
                nouveauDocument.Add(new Paragraph(decryptedContent));
            }
            catch (DocumentException de)
            {
                Console.WriteLine("error " +de.Message);
            }
            catch (System.IO.IOException ioe)
            {
                Console.WriteLine("error " +ioe.Message);
            }
            nouveauDocument.Close();

        }

        public void SendEmail(string emailAddress)
        {
            MailMessage mail = new MailMessage("projrecherche@gmail.com", emailAddress);
            SmtpClient client = new SmtpClient()
            {
                Timeout = 20000
            };
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("projrecherche@gmail.com", "recherche123");
            client.Host = "smtp.gmail.com";
            mail.IsBodyHtml = true;
            mail.Subject = "File decrypted";
            mail.Body = "Vous trouverez ci-joint le fichier décrypté ";
            mail.Attachments.Add(new Attachment(@"fichier.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            client.Send(mail);
        }
    }
}