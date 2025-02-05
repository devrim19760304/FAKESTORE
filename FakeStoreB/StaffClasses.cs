public class Person
{
    public int Id { get; set; }=0;
    public string FirstName { get; set; }="";
    public string LastName { get; set; }="";
    public string Email { get; set; }="";
    public string Phone { get; set; }="";
    public string Location { get; set; }="";
    public string PhotoUrl { get; set; }="";

    public Person() 
    {
        
    }
}

public static class StaffManager
{
    private static List<Person> staff = new List<Person>();
    private static bool isFull = false;

    public static bool IsFull => isFull;

    public static List<Person> GetStaff()
    {
        return staff;
    }

    public static void AddStaff(List<Person> newStaff)
    {
        if (isFull) return;

        staff.AddRange(newStaff);
        isFull = true;
    }
}
