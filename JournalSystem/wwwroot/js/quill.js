let quill;

window.initQuill = (editorId) => {
  if (quill) return;

  const el = document.getElementById(editorId);
  if (!el) return;

  quill = new Quill(el, {
    theme: "snow",
    modules: {
      toolbar: [
        ["bold", "italic", "underline", "strike", "link"],
        [{ list: "ordered" }, { list: "bullet" }],
        [{ header: [2, 3, 4, false] }],
        ["clean"],
      ],
    },
  });
};

window.getQuillHtml = () => {
  return quill ? quill.root.innerHTML : "";
};

window.setQuillHtml = (html) => {
  if (!quill) return;
  quill.root.innerHTML = html;
};
