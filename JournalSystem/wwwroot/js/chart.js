function initPieChart(id, label, labels, data) {
  const ctx = document.getElementById(id);

  new Chart(ctx, {
    type: "pie",
    data: {
      labels: labels,
      datasets: [
        {
          label: label,
          data: data,
          backgroundColor: [
            "#481638",
            "#652551",
            "#84366B",
            "#A44A87",
            "#C460A4",
          ],
          borderWidth: 0,
          hoverOffset: 4,
        },
      ],
    },
    options: {
      plugins: {
        legend: {
          display: true,
          position: "bottom",
        },
        tooltip: {
          enabled: true,
        },
      },
    },
  });
}

function generateGradientColors(labelsCount) {
  const colors = [];
  const start = { r: 72, g: 22, b: 56 };
  const end = { r: 196, g: 96, b: 164 };

  for (let i = 0; i < labelsCount; i++) {
    const r = Math.round(start.r + ((end.r - start.r) * i) / (labelsCount - 1));
    const g = Math.round(start.g + ((end.g - start.g) * i) / (labelsCount - 1));
    const b = Math.round(start.b + ((end.b - start.b) * i) / (labelsCount - 1));
    colors.push(`rgb(${r},${g},${b})`);
  }

  return colors;
}

function initBarChart(id, label, labels, data) {
  const ctx = document.getElementById(id);

  const barColors = generateGradientColors(labels.length);

  new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: label,
          data: data,
          backgroundColor: barColors,
        },
      ],
    },
    options: {
      plugins: {
        legend: { display: false },
        tooltip: { enabled: true },
      },
      scales: {
        x: { ticks: { display: true } },
        y: { beginAtZero: true },
      },
    },
  });
}

window.initPieChart = initPieChart;
window.initBarChart = initBarChart;
