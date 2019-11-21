(function (window, document) {
    let contentTypeSelector;
    let getValidation;
    let resultPanel;
    const displayValidationResponse = function () {
        if (this.readyState === 4) {
            getValidation.disabled = false;
            if (this.status === 200) {
                const code = document.createElement("code");
                if (this.getResponseHeader("content-type") === "text/xml; charset=utf-8") {
                    code.classList.add("language-xml");
                    code.appendChild(document.createTextNode(this.responseText));
                }
                else {
                    var validationResponse = JSON.parse(this.responseText);
                    code.classList.add("language-json");
                    code.innerHTML = JSON.stringify(validationResponse, null, 2);
                }
                const output = document.createElement("pre");
                output.appendChild(code);
                resultPanel.appendChild(output);
            }
            else {
                if (console.error)
                    console.error(this.responseText);
                const error = document.createElement("pre");
                error.classList.add("error");
                error.innerHTML = this.responseText;
                resultPanel.appendChild(error);
            }
        }
    };
    window.addEventListener("load", function () {
        contentTypeSelector = document.getElementById("contentTypeSelector");
        getValidation = document.getElementById("getValidation");
        resultPanel = document.getElementById("resultPanel");
        getValidation.addEventListener("click", function () {
            getValidation.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }
            const xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayValidationResponse;
            xhr.open("POST", "api/Person?validation=true", true);
            if (contentTypeSelector.value === "JSON") {
                xhr.setRequestHeader("Content-Type", "application/json");
                xhr.send("{}");
            }
            else {
                xhr.setRequestHeader("Content-Type", "text/xml");
                xhr.setRequestHeader("Accept", "text/xml");
                xhr.send("<Person>1</Person>");
            }
        });
    });
})(window, document);
//# sourceMappingURL=view-validation.js.map