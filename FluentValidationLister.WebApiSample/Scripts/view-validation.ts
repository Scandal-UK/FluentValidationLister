﻿(function (window, document) {
    let getValidation: HTMLButtonElement;
    let resultPanel: HTMLDivElement;

    const displayValidationResponse = function (this: XMLHttpRequest) {
        if (this.readyState === 4) {
            getValidation.disabled = false;

            if (this.status === 200) {

                // Format the response in a code element
                const code = document.createElement("code");
                const validationResponse = JSON.parse(this.responseText);
                code.innerHTML = JSON.stringify(validationResponse, null, 2);

                // Display the code block in the result element
                const output = document.createElement("pre");
                output.appendChild(code);
                resultPanel.appendChild(output);

            } else {
                // Unexpected response status; show the error in the console
                if (console.error) console.error(this.responseText);

                // Display the error as red in the result element
                const error = document.createElement("pre");
                error.classList.add("error");
                error.innerHTML = this.responseText;
                resultPanel.appendChild(error);
            }
        }
    };

    window.addEventListener("load", function () {
        // Define the elements that we're interested in manipulating
        getValidation = document.getElementById("getValidation") as HTMLButtonElement;
        resultPanel = document.getElementById("resultPanel") as HTMLDivElement;

        // Define a button-click event listener
        getValidation.addEventListener("click", function () {
            getValidation.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            // Assign the callback (which displays validation information)
            const xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayValidationResponse;

            // Request the validation information for the endpoint "POST:api/Person"
            xhr.open("POST", "api/Person?validation=true", true);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send("{}"); // (any old body; it's ignored anyway)
        });
    });
})(window, document);
