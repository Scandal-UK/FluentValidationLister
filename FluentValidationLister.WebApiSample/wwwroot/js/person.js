(function (window, document) {
    var getValidation, resultPanel;

    var displayValidationResponse = function () {
        if (this.readyState === 4) {
            getValidation.disabled = false;

            if (this.status === 200) {
                // Show the response in the console
                var validationResponse = JSON.parse(this.responseText);
                console.log(validationResponse);

                // Format the response in a code element
                var code = document.createElement("code");
                code.classList.add("language-json");
                code.innerHTML = JSON.stringify(validationResponse, null, 2);

                // Display the code block in the result element
                var output = document.createElement("pre");
                output.appendChild(code);
                resultPanel.appendChild(output);

            } else {
                // Unexpected response status; show the error in the console
                console.error(this.responseText);

                // Display the error as red in the result element
                var error = document.createElement("span");
                error.classList.add("error");
                error.innerHTML = this.responseText;
                resultPanel.appendChild(error);
            }
        }
    };

    window.addEventListener("load", function () {
        // Define the button and the result elements
        getValidation = document.getElementById("get-validation");
        resultPanel = document.getElementById("result-panel");

        // Define a button-click event listener
        getValidation.addEventListener("click", function () {
            getValidation.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            // Assign the callback (which displays validation information)
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayValidationResponse;

            // Request the validation information for the endpoint "POST:api/Person"
            xhr.open("POST", "api/Person?validation=true", true);
            xhr.setRequestHeader("Content-Type", "text/xml");
            xhr.setRequestHeader("Accepts", "text/xml");
            xhr.send("<Person>1</Person>");
            //xhr.send(JSON.stringify({ person: 1 })); // (any old body; it's ignored anyway)
        });
    });
})(window, document);
