namespace RemoveMSEdgeGUI.Models
{
    public class RemovalProgress
    {
        public int Percentage { get; set; }
        public string CurrentStep { get; set; }
        public string Message { get; set; }

        public RemovalProgress(int percentage, string currentStep, string message = null)
        {
            Percentage = percentage;
            CurrentStep = currentStep;
            Message = message;
        }
    }
}
