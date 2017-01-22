using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon;


namespace SESbounces
{
    class Program
    {
        static void Main(string[] args)
        {
            //empty array for final output feed to CSV file
            List<string> lines = new List<string>();    

            var amazonSQSClient = new Amazon.SQS.AmazonSQSClient("{awsAccessKeyId}","{awsSecretAccessKey}", RegionEndpoint.USEast1);

            //header row for CSV
            lines.Add("Email,CentreID,NotificationID,SQSMsgID,DateTime,Type"); 
            
            lines.AddRange(loadBounces(amazonSQSClient));
            lines.AddRange(loadComplaints(amazonSQSClient));

            System.IO.File.WriteAllLines(@"C:\awsPS\output-" + DateTime.Now.ToString("dd-MM-yyyy Hmmss") + ".csv", lines);

        }
        static List<string> loadBounces(AmazonSQSClient amazonSQSClient)
        {

            var lines = new List<string>();

            while (true)
            {
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = "{QueueUrl}";
                receiveMessageRequest.MaxNumberOfMessages = 10;

                ReceiveMessageResponse receiveMessageResponse = amazonSQSClient.ReceiveMessage(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count == 0)
                {
                    break;
                }

                foreach (Message item in receiveMessageResponse.Messages)
                {
                    var notification = Newtonsoft.Json.JsonConvert.DeserializeObject<AmazonSqsNotification>(item.Body);

                    var bounce = Newtonsoft.Json.JsonConvert.DeserializeObject<AmazonSesBounceNotification>(notification.Message);

                    if (bounce.NotificationType == "Bounce")
                    {

                        var centre = bounce.Mail.Headers.Where(a => a.name == "CentreID").FirstOrDefault();
                        string CentreID = (centre != null) ? centre.value : "";

                        var notificationV = bounce.Mail.Headers.Where(a => a.name == "NotificationID").FirstOrDefault();
                        string notificationID = (notificationV != null) ? notificationV.value : "";

                        switch (bounce.Bounce.BounceType)
                        {
                            case "Transient":
                               switch (bounce.Bounce.BounceSubType)
                                {
                                    case "AttachmentRejected":
                                        foreach (var recipient in bounce.Bounce.BouncedRecipients)
                                        {
                                            lines.Add(recipient.EmailAddress + "," + CentreID + "," + notificationID + "," + item.MessageId + "," + bounce.Bounce.Timestamp.ToString() + "," + bounce.NotificationType);
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("review" + bounce);
                                        break;
                                }
                                break;
                            default:
                                // Remove all recipients that generated a permanent bounce 
                                // or an unknown bounce.
                                foreach (var recipient in bounce.Bounce.BouncedRecipients)
                                {

                                    lines.Add(recipient.EmailAddress + "," + CentreID + "," + notificationID + "," + item.MessageId + "," + bounce.Bounce.Timestamp.ToString() + "," + bounce.NotificationType);

                                }
                                break;
                        }


                    }

                }



            }
            return lines;
        }
        static private List<string> loadComplaints(AmazonSQSClient amazonSQSClient)
        {

            var lines = new List<string>();
            while (true)
            {
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = "{QueueUrl}";
                receiveMessageRequest.MaxNumberOfMessages = 10;

                ReceiveMessageResponse receiveMessageResponse = amazonSQSClient.ReceiveMessage(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count == 0)
                {
                    break;
                }

                foreach (Message message in receiveMessageResponse.Messages)
                {
                   
                    // First, convert the Amazon SNS message into a JSON object.
                    var notification = Newtonsoft.Json.JsonConvert.DeserializeObject<AmazonSqsNotification>(message.Body);

                    // Now access the Amazon SES complaint notification.
                    var complaint = Newtonsoft.Json.JsonConvert.DeserializeObject<AmazonSesComplaintNotification>(notification.Message);

                    if (complaint.NotificationType == "Complaint")
                    {
                    
                        foreach (var recipient in complaint.Complaint.ComplainedRecipients)
                        {
                            var centre = complaint.Mail.Headers.Where(a => a.name == "CentreID").FirstOrDefault();
                            string CentreID = (centre != null) ? centre.value : "";

                            var notificationV = complaint.Mail.Headers.Where(a => a.name == "NotificationID").FirstOrDefault();
                            string notificationID = (notificationV != null) ? notificationV.value : "";
                           
                            lines.Add(recipient.EmailAddress + "," + CentreID + "," + notificationID + "," + message.MessageId + "," + complaint.Complaint.Timestamp.ToString() + ",Complaint");

                        }
                    }
                }
            }
            return lines;
        }


    }



    
}
