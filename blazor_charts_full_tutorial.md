# Creating Graphs in Blazor – Full Step‑by‑Step Tutorial

This tutorial walks you from **zero → working charts in Blazor**, using **Chart.js** with **Blazor WebAssembly or Blazor Server**.

You’ll end with:
- A reusable chart component
- Line & Bar charts
- Dynamic data updates
- Clean separation of C# and JavaScript

---

## 1. Prerequisites

You should have:
- .NET 8 or .NET 9 installed
- Basic Blazor knowledge (Razor components, `@code` blocks)

Create a project if you don’t have one:

```bash
dotnet new blazorwasm -n BlazorCharts
dotnet run
```

(Works the same for **Blazor Server**.)

---

## 2. Add Chart.js to Blazor

Open:

```
wwwroot/index.html   (WASM)
Pages/_Host.cshtml  (Server)
```

Add **Chart.js CDN**:

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="chartInterop.js"></script>
```

---

## 3. Create JavaScript Interop File

Create:

```
wwwroot/chartInterop.js
```

```javascript
window.chartInterop = {
    charts: {},

    createChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
        }

        this.charts[canvasId] = new Chart(ctx, config);
    }
};
```

This allows Blazor → JavaScript communication.

---

## 4. Create a Reusable Chart Component

Create:

```
Components/Chart.razor
```

```razor
@inject IJSRuntime JS

<canvas id="@CanvasId" width="400" height="200"></canvas>

@code {
    [Parameter] public string CanvasId { get; set; } = Guid.NewGuid().ToString();
    [Parameter] public ChartConfig Config { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("chartInterop.createChart", CanvasId, Config);
        }
    }
}
```

---

## 5. Create Chart Configuration Models

Create:

```
Models/ChartConfig.cs
```

```csharp
public class ChartConfig
{
    public string Type { get; set; } = "line";
    public ChartData Data { get; set; } = new();
    public object Options { get; set; } = new();
}

public class ChartData
{
    public List<string> Labels { get; set; } = new();
    public List<ChartDataset> Datasets { get; set; } = new();
}

public class ChartDataset
{
    public string Label { get; set; } = string.Empty;
    public List<double> Data { get; set; } = new();
    public string BorderColor { get; set; } = "blue";
    public bool Fill { get; set; } = false;
}
```

---

## 6. Use the Chart Component

Edit:

```
Pages/Index.razor
```

```razor
@page "/"

<h3>Sales Over Time</h3>

<Chart Config="lineChartConfig" />

@code {
    private ChartConfig lineChartConfig = new()
    {
        Type = "line",
        Data = new()
        {
            Labels = new() { "Jan", "Feb", "Mar", "Apr" },
            Datasets = new()
            {
                new ChartDataset
                {
                    Label = "Revenue",
                    Data = new() { 1200, 1900, 3000, 5000 },
                    BorderColor = "green"
                }
            }
        }
    };
}
```

Run the app → **You now have a graph 🎉**

---

## 7. Bar Chart Example

```razor
<Chart Config="barChartConfig" />

@code {
    ChartConfig barChartConfig = new()
    {
        Type = "bar",
        Data = new()
        {
            Labels = new() { "Red", "Blue", "Yellow" },
            Datasets = new()
            {
                new ChartDataset
                {
                    Label = "Votes",
                    Data = new() { 12, 19, 3 },
                    BorderColor = "orange"
                }
            }
        }
    };
}
```

---

## 8. Updating Chart Data Dynamically

To refresh a chart, **change the data and re‑render**:

```razor
<button @onclick="UpdateData">Update</button>

@code {
    void UpdateData()
    {
        lineChartConfig.Data.Datasets[0].Data =
            new() { 2000, 2500, 4000, 7000 };

        StateHasChanged();
    }
}
```

(Advanced: add a JS `updateChart` method for smooth animation.)

---

## 9. Common Errors & Fixes

**Chart doesn’t render?**
- Check `chartInterop.js` is loaded
- Verify `<canvas id>` exists

**Nothing happens on update?**
- Chart.js needs `.destroy()` before re‑creating

**JS error in console?**
- Ensure models serialize correctly (no circular refs)

---

## 10. Chart.js vs MudBlazor — Side-by-Side Example

Below is a **single page** that renders **Chart.js (JS interop)** and **MudBlazor Charts** together so you can directly compare behavior, performance, and developer experience.

---

### 10.1 Install MudBlazor

Add MudBlazor packages:

```bash
dotnet add package MudBlazor
```

Register MudBlazor in `Program.cs`:

```csharp
builder.Services.AddMudServices();
```

Wrap your app in `MainLayout.razor`:

```razor
<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

@Body
```

---

### 10.2 MudBlazor Line Chart Example

Add this to **the same page** where Chart.js exists (ex: `Index.razor`).

```razor
@using MudBlazor

<MudPaper Class="pa-4 mt-6">
    <MudText Typo="Typo.h6">MudBlazor – Line Chart</MudText>

    <MudChart ChartType="ChartType.Line"
              XAxisLabels="mudLabels"
              ChartSeries="mudSeries"
              Width="100%"
              Height="300px" />
</MudPaper>

@code {
    string[] mudLabels = { "Jan", "Feb", "Mar", "Apr" };

    List<ChartSeries> mudSeries = new()
    {
        new ChartSeries
        {
            Name = "Revenue",
            Data = new double[] { 1200, 1900, 3000, 5000 }
        }
    };
}
```

---

### 10.3 Same Data — Chart.js Version

This uses the **Chart.js component** you already built earlier:

```razor
<MudPaper Class="pa-4 mt-6">
    <MudText Typo="Typo.h6">Chart.js – Line Chart</MudText>

    <Chart Config="lineChartConfig" />
</MudPaper>
```

Now both charts render **side-by-side on the same page**.

---

## 11. Real-Time Trading & Monitoring — Which Should You Use?

This matters a lot for **trading, monitoring, observability, or telemetry dashboards**.

### MudBlazor Charts (Reality Check)

Pros:
- Very easy to use
- Clean UI
- Blazor-native

Hard limitations:
- ❌ No streaming updates
- ❌ Re-renders whole component
- ❌ Poor performance with high-frequency updates
- ❌ Limited control over animations

MudBlazor charts are **NOT designed for real-time data feeds**.

---

### Chart.js (Reality Check)

Pros:
- ✅ Partial dataset updates
- ✅ Smooth animations
- ✅ Handles high-frequency updates
- ✅ Works perfectly with SignalR / WebSockets
- ✅ Fine-grained control (ticks, axes, time series)

This is how **trading platforms, monitoring tools, and analytics dashboards** are actually built.

---

## 12. Final Recommendation (No Sugarcoating)

### Use **Chart.js** if:
- You are doing **real-time trading**
- You are monitoring prices, latency, or volume
- You need sub-second updates
- Charts are a **core feature**

### Use **MudBlazor Charts** if:
- Charts are secondary UI elements
- Updates are infrequent
- You want fast development

---

### Best Architecture for Trading Apps

**Production-grade setup:**

```
SignalR / WebSockets
        ↓
Blazor Component
        ↓
Chart.js (JS interop)
```

This is the same architecture used in:
- Trading terminals
- Observability dashboards
- Financial monitoring tools

---

## 13. Where to Go Next

- Real-time price feed demo (SignalR)
- Candlestick / OHLC charts
- Time-series scaling & zoom
- WebAssembly performance tuning
- Azure SignalR Service integration

If you want, I can build the **real-time trading chart next**, end-to-end.

- Pie / Doughnut charts
- Real‑time charts (SignalR)
- Dashboard layouts
- Wrapping charts into a NuGet‑style component

Libraries worth exploring:
- **Blazorise Charts**
- **Syncfusion Blazor Charts**
- **MudBlazor Charts**

---

## Final Result

You now have:
- A clean Blazor chart component
- JS interop done the *right* way
- Reusable chart configs

If you want, I can:
- Convert this into **PDF / Word**
- Add **real‑time SignalR graphs**
- Build a **full dashboard layout**
- Make this **Blazor Server‑optimized**

Just tell me 👍

