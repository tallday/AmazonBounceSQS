namespace SESbounces
{
    class AmazonSqsNotification
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    /// <summary>Represents an Amazon SES bounce notification.</summary>
    class AmazonSesBounceNotification
    {
        public string NotificationType { get; set; }
        public AmazonSesBounce Bounce { get; set; }
        public AmazonSesMail Mail { get; set; }
    }
    /// <summary>Represents meta data for the bounce notification from Amazon SES.</summary>
    class AmazonSesBounce
    {
        public string BounceType { get; set; }
        public string BounceSubType { get; set; }
        public DateTime Timestamp { get; set; }
        public List<AmazonSesBouncedRecipient> BouncedRecipients { get; set; }
    }
    class AmazonSesMail
    {
        public List<HeaderValues> Headers { get; set; }
     
    }
    class HeaderValues
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    /// <summary>Represents the email address of recipients that bounced
    /// when sending from Amazon SES.</summary>
    class AmazonSesBouncedRecipient
    {
        public string EmailAddress { get; set; }
    }

    class AmazonSesComplaintNotification
    {
        public string NotificationType { get; set; }
        public AmazonSesComplaint Complaint { get; set; }
        public AmazonSesMail Mail { get; set; }
    }
    /// <summary>Represents the email address of individual recipients that complained 
    /// to Amazon SES.</summary>
    class AmazonSesComplainedRecipient
    {
        public string EmailAddress { get; set; }
    }
    /// <summary>Represents meta data for the complaint notification from Amazon SES.</summary>
    class AmazonSesComplaint
    {
        public List<AmazonSesComplainedRecipient> ComplainedRecipients { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageId { get; set; }
    }
}