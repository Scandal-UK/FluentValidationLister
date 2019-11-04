(function (window, document, vkbeautify) {
    var getValidation, resultPanel;

    var displayValidationResponse = function () {
        if (this.readyState === 4) {
            getValidation.disabled = false;

            if (this.status === 200) {

                // Format the response in a code element
                var code = document.createElement("code");
                if (this.getResponseHeader('content-type') === "text/xml; charset=utf-8")
                {
                    code.classList.add("language-xml");
                    code.appendChild(document.createTextNode(vkbeautify.xml(this.responseText)));
                }
                else
                {
                    var validationResponse = JSON.parse(this.responseText);
                    code.classList.add("language-json"); // prismjs?
                    code.innerHTML = JSON.stringify(validationResponse, null, 2);
                }

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

            //// Request the validation information for the endpoint "POST:api/Person"
            //xhr.open("POST", "api/Person?validation=true", true);
            //xhr.setRequestHeader("Content-Type", "application/json");
            //xhr.send(JSON.stringify({ person: 1 })); // (any old body; it's ignored anyway)

            //// Get a Person serialised as XML
            //xhr.open("GET", "api/Person/1", true);
            //xhr.setRequestHeader("Accept", "text/xml");
            //xhr.send();

            xhr.open("POST", "api/Person?validation=true", true);
            xhr.setRequestHeader("Content-Type", "text/xml");
            xhr.setRequestHeader("Accept", "text/xml");
            xhr.send("<Person>1</Person>");
        });
    });
})(window, document, vkbeautify);

