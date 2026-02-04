namespace chartModel;
public class ChartConfig
    {
        public string Type { get; set; } = "line";
        public ChartData Data { get; set; } = new();
        public object Options {get; set; } = new();
    }
    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
    }
    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<double> Data {get; set; } = new();
        public string BorderColor { get; set; } = "blue";
        public bool Fill { get; set; } = false;
    }
