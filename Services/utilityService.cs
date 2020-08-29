using InstantTutors.DBManage;
using InstantTutors.Helpers;
using InstantTutors.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace InstantTutors.Services
{
    public static class utilityService
    {
        public static string GetEmailAddress(string id)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT top 1 Email FROM [dbo].[AspNetUsers] where id= '" + id + "'";
            dt = dbHandlerObj.GetData(query);
            return dt.Rows[0][0].ToString();
        }

        public static string ActivateDeactivateUsers(string id)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT HasDisabled FROM [dbo].[AspNetUsers] where  id='" + id + "'";
            dt = dbHandlerObj.GetData(query);
            if (dt.Rows[0][0] != null && Convert.ToBoolean(dt.Rows[0][0]))
            {
                query = "update [dbo].[AspNetUsers] set HasDisabled=0 where id='" + id + "'";
                dbHandlerObj.TranscatData(query);
            }
            else
            {
                query = "update [dbo].[AspNetUsers] set HasDisabled=1 where id='" + id + "'";
                dbHandlerObj.TranscatData(query);
            }
            return "success";
        }
        public static List<Messages> GetMessagesTutor(string toEmail)
        {
            DBHandler dbHandlerObj = new DBHandler();
            List<Messages> _messages = new List<Messages>();
            Messages _message;
            DataTable dt = new DataTable();
            string query = "SELECT * FROM [dbo].[MessageCenter] where  fromEmail='" + toEmail + "' or toEmail='" + toEmail + "' order by CreatedDate desc";
            dt = dbHandlerObj.GetData(query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                _message = new Messages();
                _message.MessageId = dt.Rows[i]["MessageId"].ToString();
                _message.MessageTo = dt.Rows[i]["MessageTo"].ToString();
                _message.MessageFrom = dt.Rows[i]["MessageFrom"].ToString();
                _message.MessageContent = dt.Rows[i]["MessageContent"].ToString();
                _message.CreatedDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"]);
                _messages.Add(_message);
            }

            return _messages;
        }
        public async static Task<bool> SendMessageTutor(string mailTo, string mailFrom, string messageBody, string subject)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT top 1 Email,FirstName +' '+ LastName as fullName FROM [dbo].[AspNetUsers] where id= '" + mailTo + "'";
            dt = dbHandlerObj.GetData(query);
            string mailToFullName = dt.Rows[0][1].ToString();
            mailTo = dt.Rows[0][0].ToString();

            query = "SELECT top 1 Email,FirstName +' '+ LastName as fullName FROM [dbo].[AspNetUsers] where id= '" + mailFrom + "'";
            dt = dbHandlerObj.GetData(query);
            string mailFromFullName = dt.Rows[0][1].ToString();
            mailFrom = dt.Rows[0][0].ToString();
            query = "INSERT INTO[dbo].[MessageCenter]";
            query += " ([MessageId]";
            query += ",[MessageTo]";
            query += ",[MessageFrom]";
            query += ",[ToEmail]";
            query += ",[FromEmail]";
            query += ",[MessageContent]";
            query += ",[Createdby])";
            query += " VALUES";
            query += " ('" + Guid.NewGuid() + "'";
            query += " ,'" + mailFromFullName  + "'";
            query += " ,'" + mailToFullName + "'";
            query += " ,'" + mailFrom + "'";
            query += " ,'" +  mailTo + "'";
            query += " ,'" + messageBody + "'";
            query += " ,'" + mailTo + "')";

            dbHandlerObj.TranscatData(query);
            await EmailSender.SendEmailAsync(subject, messageBody, mailTo);
            return true;
        }

        public static List<Messages> GetMessagesStudent(string toEmail)
        {
            DBHandler dbHandlerObj = new DBHandler();
            List<Messages> _messages = new List<Messages>();
            Messages _message;
            DataTable dt = new DataTable();
            string query = "SELECT * FROM [dbo].[MessageCenter] where  fromEmail='" + toEmail + "' or toEmail='" + toEmail + "' order by CreatedDate desc";
            dt = dbHandlerObj.GetData(query);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                _message = new Messages();
                _message.MessageId = dt.Rows[i]["MessageId"].ToString();
                _message.MessageTo = dt.Rows[i]["MessageTo"].ToString();
                _message.MessageFrom = dt.Rows[i]["MessageFrom"].ToString();
                _message.MessageContent = dt.Rows[i]["MessageContent"].ToString();
                _message.CreatedDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"]);
                _messages.Add(_message);
            }

            return _messages;
        }
        public async static Task<bool> SendMessageStudent(string mailTo, string mailFrom, string messageBody, string subject)
        {
            DBHandler dbHandlerObj = new DBHandler();
            DataTable dt = new DataTable();
            string query = "SELECT top 1 Email,FirstName +' '+ LastName as fullName FROM [dbo].[AspNetUsers] where id= '" + mailTo + "'";
            dt = dbHandlerObj.GetData(query);
            string mailToFullName = dt.Rows[0][1].ToString();
            mailTo = dt.Rows[0][0].ToString();

            query = "SELECT top 1 Email,FirstName +' '+ LastName as fullName FROM [dbo].[AspNetUsers] where id= '" + mailFrom + "'";
            dt = dbHandlerObj.GetData(query);
            string mailFromFullName = dt.Rows[0][1].ToString();
            mailFrom = dt.Rows[0][0].ToString();
            query = "INSERT INTO[dbo].[MessageCenter]";
            query += " ([MessageId]";
            query += ",[MessageTo]";
            query += ",[MessageFrom]";
            query += ",[ToEmail]";
            query += ",[FromEmail]";
            query += ",[MessageContent]";
            query += ",[Createdby])";
            query += " VALUES";
            query += " ('" + Guid.NewGuid() + "'";
            query += " ,'" + mailFromFullName + "'";
            query += " ,'" + mailToFullName + "'";
            query += " ,'" + mailFrom + "'";
            query += " ,'" + mailTo + "'";
            query += " ,'" + messageBody + "'";
            query += " ,'" + mailTo + "')";

            dbHandlerObj.TranscatData(query);
            await EmailSender.SendEmailAsync(subject, messageBody, mailTo);
            return true;
        }

    }
}