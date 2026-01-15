namespace mvc3.viewmodels
{
    public class createdusermodel
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public IEnumerable<string>? roles { get; set; }
    }
}
