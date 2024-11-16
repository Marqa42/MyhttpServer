using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Services
{
    internal class MailService : IMailService

    {
    public async Task SendAsync(string email, string message, string path)
    {
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        using (smtp)
        {
            smtp.Credentials = new NetworkCredential("marat.gaifullin2015@gmail.com", "tdyo ylwl axgh bdom");
            smtp.EnableSsl = true;

            using (MailMessage m = new MailMessage())
            {
                m.From = new MailAddress("marat.gaifullin2015@gmail.com");
                m.To.Add(email);
                m.Subject = "Тест";
                string attachPath =
                    "C:\\Users\\acer\\Desktop\\Практики\\Новая папка\\MyhttpServer\\MyhttpServer\\MyhttpServer.zip";
                switch (path)
                {
                    case "/Login.html":
                        m.Body = $"Добро пожаловать в систему, Ваша почта: {email} , ваш пароль: {message}";
                        break;

                    case "/lol.html":
                        m.Body =
                            $"Ха-ха вы попались, вашу почту {email}, ваш пароль:{message} теперь знаю Я, Низамов Алмаз";
                        break;

                    case "/homework.html":
                        m.Body = "Мое ДЗ Гайфуллин Марат Дамирович";
                        m.Attachments.Add(new Attachment(attachPath));
                        break;
                }

                try
                {
                    smtp.Send(m);
                    Console.WriteLine("Сообщение отправлено");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка отправки сообщения: {e.Message}");
                }
            }
        }
    }
    }
    
}
