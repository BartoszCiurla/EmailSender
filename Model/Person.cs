namespace Model
{
    public class Person:EmailAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString()
        {
            return $"{FirstName}, {LastName}, {Email}";
        }
    }
}
