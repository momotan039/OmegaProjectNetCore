using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace OmegaProject.services
{
   
    public partial class MyTools
    {
        private static string mainStyle = @"<style>
                                    .button{
                                            display: block;
                                            margin-top: 25px;
                                            padding: 20px;
                                            text-align: center;
                                            color: white;
                                            border-radius: 20px;
                                            background: -webkit-linear-gradient(to right, #f83600, #fe8c00);
                                            background: linear-gradient(to right, #f83600, #fe8c00);        
                                            }
                                     body{
                                            font-weight:bold
                                            }
                            </style>";

        static bool SendMail(MimeMessage msg)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect("smtp.gmail.com", 465, true);
                //this will send a mail
                client.Authenticate("manager.omega.academy@gmail.com", "pfeqadqgnpittqpg");
                client.Send(msg);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        public static bool SendConfirmRegistration(int userId, string reciver)
        {
            string link = "http://localhost:4200/ConfirmRegistration/" + userId;
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Admin Omega", "stam@gmail.com"));
            msg.To.Add(MailboxAddress.Parse(reciver));
            msg.Subject = "Confirm Regestriation";
            msg.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = @"<html>
                            "+ mainStyle + @"
                            <body>
                            <h1>Welcome to the <span style='color:orange'>Omega Academy</span> Family</h1>
                            Your account has been created successfully
                            All you have to do Now , is activate it via the following link
                            <a class='button' href='" + link + @"'>Confirm Regestriation</a>
                            </body>
                        </html>"
            };

            return SendMail(msg);
        }

      

       

        public static bool SendResetPassMail(string token, string reciverMail)
        {
            var msg = new MimeMessage();
            string link = "http://localhost:4200/ResetPassword/" + token;
            msg.From.Add(new MailboxAddress("Admin Omega", "stam@gmail.com"));
            msg.To.Add(MailboxAddress.Parse(reciverMail));
            msg.Subject = "Reset Password Request";
            msg.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = @"<html>
                            " + mainStyle + @"
                            <body>
                            All you have to do Now ,  in order to  reset your password is navigate the following link
                            <a class='button' href='" + link + @"'>Confirm Regestriation</a>
                            </body>
                        </html>"
            };

            return SendMail(msg);
        }
    }
}
