async function loadCss(href) {
  return new Promise((resolve) => {
    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = href;
    link.onload = resolve;
    document.head.appendChild(link);
  });
}

window.exportJournalsToPdf = async function (journals) {
  const { jsPDF } = window.jspdf;
  if (!jsPDF) {
    console.error("jsPDF not loaded");
    return;
  }

  const container = document.createElement("div");
  container.style.width = "190mm";
  container.style.padding = "0";

  journals.forEach((j, index) => {
    const entry = document.createElement("div");
    entry.style.marginBottom = "2rem";
    if (index < journals.length - 1) {
      entry.className = "page";
    }

    entry.innerHTML = `
    <div style="color: #000">
        <h2 style="font-size: 2.25rem; font-weight: 700">${j.title}</h2>
        <p>${j.date}</p>
        <p><strong>Category:</strong> ${j.category}</p>
        <p><strong>Mood:</strong> ${j.primaryMood}</p>
        <p><strong>Other moods:</strong> ${j.secondaryMoods.join(", ")}</p>
        <p><strong>Tags:</strong> ${j.tags.join(", ")}</p>
        <div class="rich_text_container">${j.richTextHtml}</div>
        <hr style="margin-top: 1rem;">
      </div>
    `;

    container.appendChild(entry);
  });

  document.body.appendChild(container);

  const doc = new jsPDF({
    orientation: "p",
    unit: "mm",
    format: "a4",
  });

  await doc.html(container, {
    x: 10,
    y: 10,
    width: 190,
    html2canvas: {
      scale: 0.264,
      allowTaint: true,
      useCORS: true,
    },
    callback: (doc) => {
      document.body.removeChild(container);
      doc.save("journals.pdf");
    },
  });
};
