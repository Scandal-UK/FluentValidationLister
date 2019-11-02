(function (window, document) {
    var getValidation, resultPanel;

    var displayValidationResponse = function () {
        if (this.readyState === 4) {
            getValidation.disabled = false;

            if (this.status === 200) {
                // Display JSON response
                var output = document.createElement("pre");
                output.innerHTML = JSON.stringify(JSON.parse(this.responseText), null, 2);
                resultPanel.appendChild(output);

            } else {
                // Display error in red text!
                var error = document.createElement("span");
                error.classList.add("error");
                error.innerHTML = this.responseText;
                resultPanel.appendChild(error);
            }
        }
    };

    window.addEventListener("load", function () {
        getValidation = document.getElementById("get-validation");
        resultPanel = document.getElementById("result-panel");

        getValidation.addEventListener("click", function () {
            getValidation.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            // Request validation information for "POST:api/Person"
            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayValidationResponse;
            xhr.open("POST", "api/Person?validation=true", true);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send(JSON.stringify({ foo: "bar" })); // any old body; it's ignored
        });
    });
})(window, document);
