(function (window, document, $) {
    var populateForm, personForm, resultPanel;

    var displayResult = function () {
        if (this.readyState === 4) {
            personForm.disabled = false;

            var validationResponse = JSON.parse(this.responseText);

            var code = document.createElement("code");
            code.classList.add("language-json"); // prismjs?
            code.innerHTML = JSON.stringify(validationResponse, null, 2);

            // Display the code block in the result element
            var output = document.createElement("pre");
            output.appendChild(code);
            resultPanel.appendChild(output);
        }
    };

    window.addEventListener("load", function () {
        populateForm = document.getElementById("populate-form");
        personForm = document.getElementById("person-form");
        resultPanel = document.getElementById("result-panel");

        personForm.addEventListener("submit", function (e) {
            e.preventDefault();
            personForm.disabled = true;
            if (resultPanel.hasChildNodes()) {
                resultPanel.removeChild(resultPanel.childNodes[0]);
            }

            var data = {};
            $(personForm).serializeArray().map(function (field) {
                if (field.name.includes("."))
                {
                    var split = field.name.split(".");
                    if (!data[split[0]]) data[split[0]] = {};

                    data[split[0]][split[1]] = field.value;
                } else {
                    data[field.name] = field.value;
                }
            });

            if (data.age === "") data.age = null;

            var xhr = new XMLHttpRequest();
            xhr.onreadystatechange = displayResult;
            xhr.open("POST", "api/Person", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.send(JSON.stringify(data));
        });
    });
})(window, document, $);
