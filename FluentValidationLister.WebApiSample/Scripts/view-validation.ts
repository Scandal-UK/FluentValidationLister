(function (window, document) {
    let getValidation: HTMLButtonElement;
    let getTypes: HTMLButtonElement;
    let resultPanel: HTMLDivElement;

    const displayValidationResponse = function (this: XMLHttpRequest) {
        if (this.readyState === 4) {
            toggleButtons(true);

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
        if (console?.error) console.error(responseText);

        const error = document.createElement("pre");
        error.classList.add("error");
        error.innerHTML = responseText;
        resultPanel.appendChild(error);
    };

    const getValidationDetails = function (controller: string) {
        toggleButtons(false);
        if (resultPanel.hasChildNodes()) resultPanel.removeChild(resultPanel.childNodes[0]);

        const xhr = new XMLHttpRequest();
        xhr.onreadystatechange = displayValidationResponse;
        xhr.open("POST", "api/" + controller + "?validation=true", true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send("{}"); // (any old body; it's ignored anyway)
    };

    const toggleButtons = function (enabled: boolean) {
        getValidation.disabled = !enabled;
        getTypes.disabled = !enabled;
    };

    window.addEventListener("load", function () {
        getValidation = document.getElementById("getValidation") as HTMLButtonElement;
        getTypes = document.getElementById("getTypes") as HTMLButtonElement;
        resultPanel = document.getElementById("resultPanel") as HTMLDivElement;

        getValidation.addEventListener("click", () => getValidationDetails("Person"));
        getTypes.addEventListener("click", () => getValidationDetails("Optional"));
    });
})(window, document);
