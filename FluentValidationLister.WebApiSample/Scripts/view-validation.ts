(function (window, document) {
    let getValidation: HTMLButtonElement = document.getElementById("getValidation") as HTMLButtonElement;
    let resultPanel: HTMLDivElement = document.getElementById("resultPanel") as HTMLDivElement;

    const displayValidationResponse = function (this: XMLHttpRequest) {
        if (this.readyState === 4) {
            getValidation.disabled = false;
            if (this.status === 200) showResponse(this.responseText);
            else showError(this.responseText);
        }
    };

    const showResponse = function (responseText: string) {
        const code = document.createElement("code");
        const validationResponse = JSON.parse(responseText);
        code.innerHTML = JSON.stringify(validationResponse, null, 2);

        const output = document.createElement("pre");
        output.appendChild(code);
        resultPanel.appendChild(output);
    };

    const showError = function (responseText: string) {
        if (console.error) console.error(responseText);

        const error = document.createElement("pre");
        error.classList.add("error");
        error.innerHTML = responseText;
        resultPanel.appendChild(error);
    };

    window.addEventListener("load", function () {
        getValidation.addEventListener("click", function () {
            getValidation.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            const xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayValidationResponse;
            xhr.open("POST", "api/Person?validation=true", true);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send("{}"); // (any old body; it's ignored anyway)
        });
    });
})(window, document);
