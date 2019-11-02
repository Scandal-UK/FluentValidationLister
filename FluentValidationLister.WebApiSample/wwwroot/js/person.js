(function (window, document) {
    var getValidation, resultPanel;

    var displayValidationResponse = function () {
        if (this.readyState === 4) {
            getValidation.disabled = false;

            if (this.status === 200) {
                // Display the JSON response in the result element
                var output = document.createElement("pre");
                output.innerHTML = JSON.stringify(JSON.parse(this.responseText), null, 2);
                resultPanel.appendChild(output);

            } else {
                // Unexpected response status - display error in red text!
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
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send(JSON.stringify({ foo: "bar" })); // (any old body; it's ignored anyway)
        });
    });
})(window, document);
