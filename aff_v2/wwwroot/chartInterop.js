window.chartInterop = {

    charts: {},

    createChart: function (canvasId, config) {
        const ctx = document.getElementById(canvasId);
        
        if (!ctx) return;
        if (this.charts[canvasId]){
            this.charts[canvasId].destroy();
        }
        
        this.charts[canvasId] = new Chart(ctx, config);
    }
};