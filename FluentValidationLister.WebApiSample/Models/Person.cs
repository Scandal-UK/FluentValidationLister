namespace FluentValidationLister.WebApiSample.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }
}
