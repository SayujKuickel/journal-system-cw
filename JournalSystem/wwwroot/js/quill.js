let quill;

window.initQuill = (editorId) => {
    quill = new Quill(`#${editorId}`, {
        theme: 'snow',
        modules: {
            toolbar: [
                ['bold', 'italic', 'underline', 'strike'],
                [{ list: "ordered" }, { list: "bullet" }], 
                [{ header: [1, 2, 3, false] }],
                ["clean"]
            ]
        }
    });
};

window.getQuillHtml = () => {
    return quill.root.innerHTML;
}

window.setQuillHtml = (html) => {
    quill.root.innerHTML = html;
}