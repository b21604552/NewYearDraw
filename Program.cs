using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NewYearDraw
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> allParticipant = new List<Person>();
            string filePath = @"XXXXX\participants.txt";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string line = sw.ReadLine();
            while (line != null)
            {
                var infos = line.Split(' ');
                var newPerson = new Person();
                newPerson.id = Convert.ToInt32(infos[0]);
                newPerson.name = infos[1];
                newPerson.email = infos[2];
                allParticipant.Add(newPerson);
                line = sw.ReadLine();
            }
            sw.Close();
            fs.Close();
            List<Person> drawedPersons = allParticipant;
            List<DrawResult> drawResult = new List<DrawResult>();
            foreach (var participant in allParticipant)
            {
                int randomValue = randomGenerator(drawedPersons.Count(), drawedPersons, participant.id);
                DrawResult temp = new DrawResult();
                temp.from = participant;
                temp.to = drawedPersons.Where(x => x.isDrawed == true).ToList().FirstOrDefault();
                drawResult.Add(temp);
                drawedPersons = drawedPersons.Where( x => x.isDrawed != true).ToList();
            }
            foreach (var draw in drawResult)
            {
                string body = PopulateBody(draw.to, draw.from);
                SendGmail(body, draw.from.email);
            }
        }
        private static string PopulateBody(Person drawed, Person particapant)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("NewYearDraw.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Participant}", particapant.name);
            body = body.Replace("{DrawPerson}", drawed.name);
            body = body.Replace("{DrawPerson}", drawed.name);
            body = body.Replace("{DrawPerson}", drawed.name);

            return body;
        }
        private static void SendGmail(string body, string email)
        {
            var fromAddress = new MailAddress("xxxxxx", "xxxxx");
            var toAddress = new MailAddress(email, "Yılbaşı Çekilişi");
            const string fromPass = "xxxxxxx";
            const string subject = "Yılbaşı Çekilişi";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPass)
            };
            using (var mes = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            })
            {
                smtp.Send(mes);
            }
        }
        public static int randomGenerator(int limit,List<Person> drawedPersons, int id)
        {
            Random rnd = new Random();
            int random = rnd.Next(limit);
            if (drawedPersons[random].id == id)
                return randomGenerator(limit, drawedPersons, id);
            else
            {
                var person = drawedPersons[random];
                person.isDrawed = true;
                return random;
            }
        }

        public class Person
        {
            public int id;
            public string name;
            public string email;
            public bool isDrawed;
        }

        public class DrawResult
        {
            public Person from;
            public Person to;
        }
    }
}
