public class Dialog
{
    public string Text { get; set; }
    public string Prompt { get; set; }
    public int Timer {get; set;}

    public Dialog(string text, string prompt, int timer)
    {
        Text = text;
        Prompt = prompt;
        Timer = timer;
    }
}