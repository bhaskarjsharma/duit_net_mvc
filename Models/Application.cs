namespace duit_net_mvc.Models
{
    public class Application
    {
        public int ApplicationId { set; get; }
        public string User { set; get; }
        public string UserName { set; get; }
        public ICollection<ApplicationAttachment> ApplicationAttachment { set; get; }
    }
}
