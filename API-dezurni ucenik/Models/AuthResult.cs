namespace API_dezurni_ucenik.Models
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool ResultStatus { get; set; }
        public List<string> Errors { get; set; }
    }


}
