public class Dialog
{
    public string Text { get; set; }
    public string Prompt { get; set; }

    public Dialog(string text, string prompt)
    {
        Text = text;
        Prompt = prompt;
    }
}