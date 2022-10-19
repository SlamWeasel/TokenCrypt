using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace Token_Crypt
{
    class Program
    {
        static void Main(string[] args)
        {
            for (; ; Thread.Sleep(60000))
            {
                if (DateTime.UtcNow.AddHours(1).Hour == 1 && DateTime.UtcNow.Minute == 0)
                {
                    string fileDest = Environment.CurrentDirectory + @"\\CrypiticOutput.txt";

                    string OUT;
                    string username = "usr", password = "pwd";

                    OUT = MD5(username.ToLower() +
                                ":" +
                                MD5(password).Substring(0, 19).ToLower() +
                                ":" +
                                DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00")
                             ).Substring(0, 32).ToLower();

                    try { File.WriteAllText(fileDest, OUT); }
                    catch (Exception e) { File.WriteAllText(fileDest, "Ein Problem ist aufgetreten;\n\n" + e); }



                    string url = @"https://custom.er/api/" + username + @"/position/positions?token=" + OUT;

                    HttpWebRequest request;
                    HttpWebResponse response;
                    try
                    {
                        request = (HttpWebRequest)WebRequest.Create(url);
                        //request.Proxy.Credentials = new NetworkCredential("usr", "pwd", "dmn");
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch (WebException w)
                    {
                        SmtpClient smtp = new SmtpClient("255.255.255.255")
                        {
                            Port = 25,
                            //Credentials = new NetworkCredential("usr", "pwd"),
                            //EnableSsl = true
                        };

                        MailMessage mail = new MailMessage
                        {
                            From = new MailAddress("autouser@fixemer.com"),
                            Subject = "Fehler bei der Anfrage an autosatnet WAFPOL",
                            Body =
                            @"   Fehlercode " + (int)((HttpWebResponse)w.Response).StatusCode + " " + ((HttpWebResponse)w.Response).StatusCode +
                            "\n\nAnfrage an " + url + " fehlgeschalgen\n\n" +
                            "..."
                        }; mail.To.Add("user1@fixemer.com"); mail.To.Add("user2@fixemer.com"); mail.To.Add("user3@fixemer.com");

                        smtp.Send(mail);
                    }
                    Console.WriteLine("\n" + DateTime.Now + " Created Token!\n");
                    Thread.Sleep(60000);
                }

                Console.WriteLine(DateTime.Now + " Checking time...");
            }
        }

        public static string MD5(string s)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }
        }
    }
}
