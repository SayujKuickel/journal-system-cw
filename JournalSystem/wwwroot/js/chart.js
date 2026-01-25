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

function initBarChart(id, label, labels, data) {
  const ctx = document.getElementById(id);

  new Chart(ctx, {
    type: "bar",
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
        },
      ],
    },
    options: {
      plugins: {
        legend: {
          display: false,
        },
        tooltip: {
          enabled: true,
        },
      },
      scales: {
        x: {
          ticks: {
            display: true,
          },
        },
        y: {
          beginAtZero: true,
        },
      },
    },
  });
}
